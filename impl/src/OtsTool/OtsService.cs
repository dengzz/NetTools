using Aliyun.OTS;
using Aliyun.OTS.DataModel;
using Aliyun.OTS.Request;
using Aliyun.OTS.Response;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OtsTool
{
    public class OtsService
    {
        private readonly Lazy<OTSClient> m_OTSClient;

        public ILogger Logger { get; set; }

        public OtsService(string endpint, string accessKeyId, string accessKeySecret, string instanceName)
        {
            m_OTSClient = new Lazy<OTSClient>(() =>
            {
                var config = new OTSClientConfig(endpint, accessKeyId, accessKeySecret, instanceName);
                config.OTSErrorLogHandler = (log) => { Logger?.LogError(log); };
                config.OTSDebugLogHandler = (log) => { Logger?.LogDebug(log); };
                return new OTSClient(config);
            });
        }

        public void ExportTable(string tableName, IEnumerable<string> columnNames,
            Tuple<PrimaryKey, PrimaryKey> primaryKeys, Action<IList<RowDataFromGetRange>> handle)
        {
            var describeTableRequest = new DescribeTableRequest(tableName);
            var describeTableResponse = m_OTSClient.Value.DescribeTable(describeTableRequest);
            var primaryKeySchema = describeTableResponse.TableMeta.PrimaryKeySchema;
            var rangeRowQueryCriteria = new RangeRowQueryCriteria(tableName);
            rangeRowQueryCriteria.InclusiveStartPrimaryKey = CreateGetRangePrimaryKey(primaryKeySchema, ColumnValue.INF_MIN, primaryKeys?.Item1);
            rangeRowQueryCriteria.ExclusiveEndPrimaryKey = CreateGetRangePrimaryKey(primaryKeySchema, ColumnValue.INF_MAX, primaryKeys?.Item2);
            if (columnNames?.Any() ?? false)
            {
                rangeRowQueryCriteria.SetColumnsToGet(new HashSet<string>(columnNames));
            }
            GetRangeRequest getRangeRequest;
            GetRangeResponse getRangeResponse;
            while (true)
            {
                if (rangeRowQueryCriteria.InclusiveStartPrimaryKey == null)
                {
                    break;
                }
                else
                {
                    getRangeRequest = new GetRangeRequest(rangeRowQueryCriteria);
                    getRangeResponse = m_OTSClient.Value.GetRange(getRangeRequest);
                    handle.Invoke(getRangeResponse.RowDataList);
                    rangeRowQueryCriteria.InclusiveStartPrimaryKey = getRangeResponse.NextPrimaryKey;
                }
            }
        }

        private PrimaryKey CreateGetRangePrimaryKey(PrimaryKeySchema primaryKeySchema, ColumnValue value, IDictionary<string, ColumnValue> userColumnValues)
        {
            var primaryKey = new PrimaryKey();
            foreach (var item in primaryKeySchema)
            {
                if (userColumnValues?.ContainsKey(item.Item1) ?? false)
                {
                    primaryKey.Add(item.Item1, userColumnValues[item.Item1]);
                }
                else
                {
                    primaryKey.Add(item.Item1, value);
                }
            }
            return primaryKey;
        }
    }
}

using Aliyun.OTS.DataModel;
using NetTool;
using System;
using System.Collections.Generic;

namespace OtsTool
{
    public class ExportManager
    {
        private readonly OtsService m_OtsService;

        public ExportManager(OtsService otsService)
        {
            m_OtsService = otsService;
        }

        public InvokedResult ExportTable(string fileName, string tableName, string columnNames, string primaryKeys)
        {
            var convertedColumnNames = ConvertColumnNames(columnNames);
            var convertedPrimaryKeys = ConvertPrimaryKeys(primaryKeys);
            m_OtsService.ExportTable(tableName,
                convertedColumnNames?.Keys,
                convertedPrimaryKeys,
                data =>
                {
                    ExcelService.Append(fileName, tableName, data, convertedColumnNames);
                }
            );
            return InvokedResult.Ok();
        }

        private Tuple<PrimaryKey, PrimaryKey> ConvertPrimaryKeys(string primaryKeys)
        {
            if (string.IsNullOrEmpty(primaryKeys))
                return null;
            var splitedPrimaryKeys = primaryKeys.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            var pk1 = new PrimaryKey();
            var pk2 = new PrimaryKey();
            foreach (var primaryKey in splitedPrimaryKeys)
            {
                var splitedPrimaryKey = primaryKey.Split(':');
                if (splitedPrimaryKey.Length > 2)
                {
                    pk1.Add(splitedPrimaryKey[0], new ColumnValue(splitedPrimaryKey[1]));
                    pk2.Add(splitedPrimaryKey[0], new ColumnValue(splitedPrimaryKey[2]));
                }
                else if (splitedPrimaryKey.Length > 1)
                {
                    pk1.Add(splitedPrimaryKey[0], new ColumnValue(splitedPrimaryKey[1]));
                    pk2.Add(splitedPrimaryKey[0], new ColumnValue(splitedPrimaryKey[1]));
                }
                else
                {
                    pk1.Add(splitedPrimaryKey[0], ColumnValue.INF_MIN);
                    pk2.Add(splitedPrimaryKey[0], ColumnValue.INF_MAX);
                }
            }
            return new Tuple<PrimaryKey, PrimaryKey>(pk1, pk2);
        }

        private IDictionary<string, ColumnType> ConvertColumnNames(string columnNames)
        {
            if (string.IsNullOrEmpty(columnNames))
                return null;

            var splitedColumnNames = columnNames.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            var result = new Dictionary<string, ColumnType>();
            var seperator = ':';
            string[] splitedColumnName;
            ColumnType parsedColumnType;
            foreach (var columnName in splitedColumnNames)
            {
                if (columnName.IndexOf(seperator) >= 0)
                {
                    splitedColumnName = columnName.Split(seperator);
                    if (Enum.TryParse(splitedColumnName[1], out parsedColumnType))
                    {
                        result.Add(splitedColumnName[0], parsedColumnType);
                        continue;
                    }
                }
                result.Add(columnName, ColumnType.Auto);
            }
            return result;
        }
    }
}

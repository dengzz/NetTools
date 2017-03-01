using NetTool;
using StackExchange.Redis;
using System;

namespace RedisTool
{
    public class RedisService
    {
        private static readonly string _DeleteKeysScript = string.Concat("local keys = redis.call('keys', ARGV[1])",
    " for i=1,#keys,5000 do redis.call('del', unpack(keys, i, math.min(i+4999, #keys))) end ",
    "return keys");
        private ConnectionMultiplexer m_Connection;
        private IDatabase m_Database;
        private string m_Configuration;

        public RedisService(string configuration)
        {
            m_Configuration = configuration;
        }

        public InvokedResult<int> DeleteKeys(string pattern)
        {
            if (string.IsNullOrEmpty(pattern))
                throw new ArgumentNullException(nameof(pattern));

            Connection();
            int total = 0;
            RedisResult redisResult;
            string[] result;
            int count;
            while (true)
            {
                redisResult = m_Database.ScriptEvaluate(_DeleteKeysScript, null, new RedisValue[] { pattern });
                result = redisResult.ToString().Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                if (result.Length == 0)
                    break;
                count = 0;
                if (int.TryParse(result[0], out count) && count > 0)
                {
                    total += count;
                }
                else
                {
                    break;
                }
            }
            return InvokedResult.Ok(total, null);
        }

        public void Connection()
        {
            if (m_Connection == null)
            {
                m_Connection = ConnectionMultiplexer.Connect(m_Configuration);
                m_Database = m_Connection.GetDatabase();
            }
        }
    }
}

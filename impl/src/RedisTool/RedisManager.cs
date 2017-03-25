using NetTool;
using System;
using System.Text;

namespace RedisTool
{
    public class RedisManager
    {
        private string[] m_Configurations;

        public RedisManager(string configurations)
        {
            if (string.IsNullOrEmpty(configurations))
                throw new ArgumentNullException(configurations, nameof(configurations));

            m_Configurations = configurations.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
        }

        public InvokedResult DeleteKeys(string pattern)
        {
            if (string.IsNullOrEmpty(pattern))
                throw new ArgumentNullException(pattern, nameof(pattern));

            var stringBuilder = new StringBuilder();
            foreach (var configuration in m_Configurations)
            {
                var deleteService = new RedisService(configuration);
                var result = deleteService.DeleteKeys(pattern);
                if (result.Succeeded)
                    stringBuilder.AppendLine($"删除数据[实例{configuration}-模式{pattern}]成功");
                else
                    stringBuilder.AppendLine(result.Message);
            }

            return InvokedResult.Ok(stringBuilder.ToString());
        }
    }
}

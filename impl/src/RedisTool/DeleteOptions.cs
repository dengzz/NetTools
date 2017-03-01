using CommandLine;

namespace RedisTool
{
    [Verb("Delete", HelpText = "删除数据")]
    public class DeleteOptions
    {
        [Option('s', "Script", Required = true, HelpText = "要执行的脚本")]
        public string Script { get; set; }

        [Option('h', "Configurations", HelpText = "要连接的配置信息，为空时从配置中读取")]
        public string Configurations { get; set; }
    }
}

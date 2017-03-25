using CommandLine;

namespace OtsTool
{
    [Verb("Export", HelpText = "导出数据")]
    class ExportOptions
    {
        [Option('t', "Table Name", Required = true, HelpText = "要执行的表格名")]
        public string TableName { get; set; }

        [Option('f', "File Name", HelpText = "要保存的文件路径，包含文件名")]
        public string FileName { get; set; }

        [Option('c', "Column Names", HelpText = "要导出的列名，为空时导出所有列，格式：列名1[:列类型1],列名2,列名3[,...],列名n")]
        public string ColumnNames { get; set; }

        [Option('i', "Instance Name", HelpText = "要执行的实例")]
        public string InstanceName { get; set; }

        [Option('p', "Primay Keys", HelpText = "要过滤的主键匹配条件，为空时不进行任何过滤，格式：主键1:最小值1[:最大值2],主键2:固定值2")]
        public string PrimayKeys { get; set; }

        [Option('e', "Endpoint", HelpText = "要执行的连接地址")]
        public string Endpoint { get; set; }

        [Option('k', "Access Key Id", HelpText = "要执行的access key id")]
        public string AccessKeyId { get; set; }

        [Option('s', "Access Key Secret", HelpText = "要执行的access key secret")]
        public string AccessKeySecret { get; set; }
    }
}

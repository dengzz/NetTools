using CommandLine;
using NetTool;
using System;
using System.Configuration;

namespace OtsTool
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var endpoint = ConfigurationManager.AppSettings["TableStore:Endpoint"];
                var accessKeyId = ConfigurationManager.AppSettings["TableStore:AccessKeyId"];
                var accessKeySecret = ConfigurationManager.AppSettings["TableStore:AccessKeySecret"];
                var instanceName = ConfigurationManager.AppSettings["TableStore:InstanceName"];
                var result = Parser.Default.ParseArguments<ExportOptions>(args)
                    .MapResult(
                        oo =>
                        {
                            var otsService = new OtsService(oo.Endpoint ?? endpoint,
                               oo.AccessKeyId ?? accessKeyId,
                               oo.AccessKeySecret ?? accessKeySecret,
                               oo.InstanceName ?? instanceName);
                            var manager = new ExportManager(otsService);
                            return manager.ExportTable(oo.FileName, oo.TableName, oo.ColumnNames, oo.PrimayKeys);
                        }
                        , errors => InvokedResult.Fail("参数错误"));
                if (result.Succeeded)
                {
                    Console.WriteLine(result.Message);
                }
                else
                {
                    Console.Error.WriteLine("操作失败：" + result.Message);
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.ToString());
            }
        }
    }
}

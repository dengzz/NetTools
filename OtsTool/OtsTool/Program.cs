using CommandLine;
using NetTool;
using System;

namespace OtsTool
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var result = Parser.Default.ParseArguments<ExportOptions>(args)
                    .MapResult(
                        oo =>
                        {
                            var otsService = new OtsService(oo.Endpoint ?? "http://search-log.cn-shenzhen.ots.aliyuncs.com",
                               oo.AccessKeyId ?? "LTAIQ6FDHFYtgej5",
                               oo.AccessKeySecret ?? "bfnWaGgANozsEy1dp3QkDN5iISdg2T",
                               oo.InstanceName ?? "search-log");
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

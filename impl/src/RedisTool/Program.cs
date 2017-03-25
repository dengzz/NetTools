using CommandLine;
using NetTool;
using System;
using System.Configuration;

namespace RedisTool
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var result = Parser.Default.ParseArguments<DeleteOptions>(args)
                    .MapResult(
                        oo =>
                        {
                            var configurations = string.IsNullOrEmpty(oo.Configurations) ?
                                ConfigurationManager.AppSettings[Constants.ConfigKeys.RedisConfigurations] : oo.Configurations;
                            var manager = new RedisManager(configurations);
                            return manager.DeleteKeys(oo.Script);
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

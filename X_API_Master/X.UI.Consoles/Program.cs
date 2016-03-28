using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using X.Util.Core;
using X.Util.Entities;

namespace X.UI.Consoles
{
    class Program
    {
        static void Main()
        {
            var config = new WCfConfig()
            {
                ProxyList = new List<WCfProxy>()
                {
                    new WCfProxy()
                    {
                        Address = "http://222.73.55.27:8002/DateService/?wsdl", 
                        Mode = MetadataExchangeClientMode.HttpGet, 
                        ProxyFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data\\IDateService.cs")
                    }
                },
                ConfigPathPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data\\testConfig.xml")
            };
            ConfigurationHelper.GenerateWCfProxyAndConfig(config);
            //Index();
        }

        static void Index()
        {
            Console.Clear();
            Console.WriteLine("┏━━━━━━━━━━━━━━━┓");
            Console.WriteLine("┃\tAPI-Console-Test:\t┃");
            foreach (var item in (EnumApiTestItem[])Enum.GetValues(typeof(EnumApiTestItem)))
            {
                Console.WriteLine("┣━━━━━━━━━━━━━━━┫");
                Console.WriteLine("┃" + (item.GetHashCode() + "." + item).PadRight(30) + "┃");           
            }
            Console.WriteLine("┗━━━━━━━━━━━━━━━┛");
            Next();
        }

        static void Next()
        {
            var input = Console.ReadLine();
            foreach (var item in ((EnumApiTestItem[])Enum.GetValues(typeof(EnumApiTestItem))).Where(item => Equals(item.GetHashCode().ToString(), input)))
            {
                Console.Clear();
                typeof(ApiTestMethods).GetMethod(item.ToString()).Invoke(null, null);
                Index();
                break;
            }
        }
    }
}

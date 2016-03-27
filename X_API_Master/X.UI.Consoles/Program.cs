using System;
using System.IO;
using System.Linq;
using X.Util.Core;

namespace X.UI.Consoles
{
    class Program
    {
        static void Main()
        {
            //Index();
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data\\test.cs");
            string config = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data\\testconfig.xml");
            ConfigurationHelper.GenerateWCfProxyAndConfig("http://222.73.55.27:8002/DateService/?wsdl", MetadataExchangeClientMode.HttpGet, path, config);
            Console.ReadKey();
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

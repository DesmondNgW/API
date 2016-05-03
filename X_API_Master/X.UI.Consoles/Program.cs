using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using X.Util.Core;
using X.Util.Entities;

namespace X.UI.Consoles
{
    public class Run
    {
        public static Run Client
        {
            get
            {
                Console.WriteLine("Run");
                return new Run();
            }
        }

        public void Test()
        {
            Console.WriteLine("Test");
        }
    }


    class Program
    {
        static void tRun(Action action)
        {
            int i = 3;
            while ((i--) > 0)
            {
                action();
            }
        }

        static void Main()
        {
            //Index();
            tRun(Run.Client.Test);
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

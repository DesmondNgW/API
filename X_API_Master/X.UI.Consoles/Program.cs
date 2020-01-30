using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text.RegularExpressions;
using X.UI.Helper;
using X.Util.Core;
using X.Util.Extend;
using X.Util.Core.Kernel;
using X.Util.Other;
using X.Util.Extend.Mongo;
using X.Util.Entities;
using X.Util.Provider;
using X.Util.Entities.Enums;
using MongoDB.Driver.Builders;
using System.Globalization;
using X.UI.Entities;
using System.IO;

namespace X.UI.Consoles
{
    class Program
    {
        static void Main()
        {
            Index();
            Console.ReadKey();
        }

        static void Index()
        {
            Console.Clear();
            Console.WriteLine(@"┏" + "━".PadRight(30, '━') + "┓");
            Console.WriteLine(@"┃API-Console-Test:" + string.Empty.PadRight(13) + "┃");
            foreach (var item in (EnumApiTestItem[])Enum.GetValues(typeof(EnumApiTestItem)))
            {
                Console.WriteLine(@"┣" + "━".PadRight(30, '━') + "┫");
                Console.WriteLine(@"┃{0}.{1}┃", item.GetHashCode(), item.ToString().PadRight(28));
            }
            Console.WriteLine(@"┗" + "━".PadRight(30, '━') + "┛");
            Next();
        }

        static void Next()
        {
            var input = Console.ReadLine();
            foreach (var item in ((EnumApiTestItem[])Enum.GetValues(typeof(EnumApiTestItem))).Where(item => Equals(item.GetHashCode().ToString(), input)))
            {
                Console.Clear();
                var methodInfo = typeof(ApiTestMethods).GetMethod(item.ToString());
                if (methodInfo != null)
                    methodInfo.Invoke(null, null);
                Index();
                break;
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using X.UI.Entities;
using X.UI.Helper;
using X.Util.Core;
using X.Util.Other;

namespace X.UI.Consoles
{
    class Program
    {
        static void Main()
        {
            StockDealHelper.Program();
            Index();
            Console.ReadKey();
        }

        static string Center(string begin, string end, string content, string pad, int total)
        {
            var len = total - begin.Length - end.Length - content.Length;
            int leftLen = len / 2 + 1,
                retLength = total - content.Length;
            var ret = new string[retLength + 1];
            ret[0] = begin;
            ret[retLength] = end;
            ret[leftLen] = content;
            return string.Join(pad, ret);
        }

        static void Index(int total = 32)
        {
            Console.Clear();
            Console.WriteLine(Center("┏", "┓", "━", "━", total));
            Console.WriteLine(Center("┃", "┃", "API-Console-Test:", "*", total));
            foreach (var item in (EnumApiTestItem[])Enum.GetValues(typeof(EnumApiTestItem)))
            {
                Console.WriteLine(Center("┣", "┫", "━", "━", total));
                Console.WriteLine(Center("┃", "┃", string.Format("{0}.{1}", item.GetHashCode(), item.ToString()), "*", total));
            }
            Console.WriteLine(Center("┗", "┛", "━", "━", total));
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
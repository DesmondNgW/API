using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using X.UI.Entities;
using X.UI.Helper;
using X.Util.Core;
using X.Util.Core.Common;
using X.Util.Other;

namespace X.UI.Consoles
{
    class Program
    {
        static void Main()
        {
            //RandGame.TestRand2();
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

    class RandGame
    {
        public static double Rand(double u, double d)
        {
            double u1, u2, z, x;
            if (d <= 0)
            {

                return u;
            }
            u1 = StringConvert.SysRandom.NextDouble();
            u2 = StringConvert.SysRandom.NextDouble();
            z = Math.Sqrt(-2 * Math.Log(u1)) * Math.Sin(2 * Math.PI * u2);
            x = u + d * z;
            return x;
        }

        public static bool TestRand()
        {
            Func<int> f = () =>
            {
                var r = Rand(20, 4);
                if (r < 0) return 0;
                if (r > 32) return 32;
                return (int)r;
            };

            var a1 = f();
            var a2 = f();
            var a3 = f();
            var a4 = f();
            var a5 = f();
            var a = (a1 + a2 + a3 + a4 + a5) / 5;
            Console.WriteLine("a1:{0}", a1);
            Console.WriteLine("a2:{0}", a2);
            Console.WriteLine("a3:{0}", a3);
            Console.WriteLine("a4:{0}", a4);
            Console.WriteLine("a5:{0}", a5);
            if (a >= 28)
            {
                Console.WriteLine("鬼!");
            }
            if (a >= 29)
            {
                Console.WriteLine("神!");
            }
            if (a >= 30)
            {
                Console.WriteLine("人!");
            }
            if (a >= 31)
            {
                Console.WriteLine("地!");
            }
            if (a >= 32)
            {
                Console.WriteLine("天!");
            }
            if (a >= 28) return true;
            return false;
        }

        public static void TestRand2()
        {
            int count = 0, total = 0;
            while (total < 10000)
            {
                if (TestRand())
                {
                    count++;
                }
                total++;
                Thread.Sleep(1000);
            }
            Console.WriteLine("{0}/{1}", count, total);
        }



    }
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using X.Util.Core;
using X.Util.Entities;

namespace X.UI.Consoles
{
    public class Channel
    {
        public void Test()
        {
            Console.WriteLine("Test");
        }
    }

    public class Run<T> where T : new()
    {
        public T Client
        {
            get
            {
                Console.WriteLine("Run");
                return new T();
            }
        }
    }


    class Program
    {
        static void tRun(Action action, Run<Channel> c)
        {
            Action acion = Delegate.CreateDelegate(typeof(Action), c.Client, action.Method) as Action;
            int i = 3;
            while ((i--) > 0)
            {
                action();
                acion();
            }
        }

        static void Main()
        {
            //Index();
            var c = new Run<Channel>();
            tRun(c.Client.Test, c);
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

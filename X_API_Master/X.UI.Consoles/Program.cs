using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web.UI.HtmlControls;
using System.Xml;
using MongoDB.Bson;
using X.DataBase.Helper;
using X.UI.Consoles.Stock;
using X.Util.Core.Xml;
using X.Util.Entities.Enums;
using X.Util.Other;

namespace X.UI.Consoles
{
    class Program
    {
        static void Main()
        {
            StockTestHelper.Test("300666", stock =>
            {
                return (stock.IncLength > 1 || stock.PositionLength > 2 || stock.StrongLength > 2) && stock.Atr > 0;
            });
            //var ret = new List<List<string>>();
            //var doc = XmlHelper.GetXmlDocCache(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "rarecharacter.xml"));
            //var nodes = doc.SelectNodes("/RareCharacters/characters/words");
            //if (nodes != null)
            //{
            //    ret.AddRange(from XmlNode node in nodes
            //                 let @group = XmlHelper.GetXmlAttributeValue(node, "group", string.Empty)
            //                 from XmlNode sub in node.ChildNodes
            //                 select new List<string>
            //        {
            //            @group, XmlHelper.GetXmlAttributeValue(sub, "pinyin", string.Empty), XmlHelper.GetXmlAttributeValue(sub, "text", string.Empty),
            //        });
            //}
            //Console.WriteLine(ret.ToJson());
            //TestCacheClient(1000);
            //StockPerformanceHelper.Compute(StockPerformanceHelper.Init(new DateTime(2017, 9, 25)));
            //MongoDbBase<MongoTest1>.Default.SaveMongo(new MongoTest1 { Dt = DateTime.Now, Value = 1, Key = "test" }, "Test", "test");
            //MongoDbBase<MongoTest2>.Default.SaveMongo(new MongoTest2 { Dt = DateTime.Now, Value = "15", Key = "test" }, "Test", "test");
            //var s = MongoDbBase<MongoTest>.Default.FindBsonDocument("Test", "test", Query.Null);
            //var f = MongoDbBase<MongoTest>.ToEntity(s);
            //Console.WriteLine(ThirdPartyTest.CouchBaseTest("1234567890", "1234567890"));
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
                var methodInfo = typeof(ApiTestMethods).GetMethod(item.ToString());
                if (methodInfo != null)
                    methodInfo.Invoke(null, null);
                Index();
                break;
            }
        }
    }
}
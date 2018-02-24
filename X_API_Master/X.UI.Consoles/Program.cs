using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
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
    //public class CaiLianNews
    //{
    //    public decimal error { get; set; }

    //    public Tmp data { get; set; }
    //}

    //public class Tmp
    //{
    //    public List<RollData> roll_data { get; set; }
    //}

    //public class RollData
    //{
    //    public int is_top { get; set; }

    //    public long modified_time { get; set; }

    //    public string share_img { get; set; }

    //    public string img { get; set; }

    //    public int jpush { get; set; }

    //    public string level { get; set; }

    //    public string author_extends { get; set; }

    //    public int bold { get; set; }

    //    public int comment_num { get; set; }

    //    public int deny_comment { get; set; }

    //    public int user_id { get; set; }

    //    //public string[] imgs { get; set; }

    //    public string shareurl { get; set; }

    //    public long ctime { get; set; }

    //    public long sort_score { get; set; }

    //    public int status { get; set; }

    //    public int reading_num { get; set; }

    //    public int in_roll { get; set; }

    //    public string title { get; set; }

    //    public string[] images { get; set; }

    //    public string[] tags { get; set; }

    //    public string content { get; set; }

    //    public string brief { get; set; }

    //    public int type { get; set; }

    //    //public string[] sub_titles { get; set; }

    //    public int id { get; set; }

    //    public int recommend { get; set; }

    //    public string depth_extends { get; set; }

    //    public int explain_num { get; set; }

    //    public int has_img { get; set; }

    //    public string category { get; set; }

    //    public int collection { get; set; }

    //    public int confirmed { get; set; }
    //}


    class Program
    {
        static void Main()
        {
            //StockTestHelper.Test("300666", stock =>
            //{
            //    return stock.StrongLength > 5;
            //});

            //TestCacheClient(1000);

            //MongoDbBase<MongoTest1>.Default.SaveMongo(new MongoTest1 { Dt = DateTime.Now, Value = 1, Key = "test" }, "Test", "test");
            //MongoDbBase<MongoTest2>.Default.SaveMongo(new MongoTest2 { Dt = DateTime.Now, Value = "15", Key = "test" }, "Test", "test");
            //var s = MongoDbBase<MongoTest>.Default.FindBsonDocument("Test", "test", Query.Null);
            //var f = MongoDbBase<MongoTest>.ToEntity(s);

            //var urlFormat = "https://www.cailianpress.com/nodeapi/telegraphs?last_time={0}&refresh_type=1&rn=20";
            //var ret = ApiData.Get<CaiLianNews>(urlFormat, null, null);
            //Console.ReadKey();
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
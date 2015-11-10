using System;
using X.Util.Entities;
using X.Util.Extend.Cache;
using X.Util.Extend.Mongo;

namespace X.UI.Consoles
{
    public class ThirdPartyTest
    {
        public static string CouchBaseTest()
        {
            CouchCache.Default.Set("test", "CouchBaseTest", DateTime.Now.AddMinutes(1));
            return CouchCache.Default.Get<string>("test");
        }

        public static string RedisTest()
        {
            RedisCache.Default.Set("test", "RedisTest", DateTime.Now.AddMinutes(1));
            return RedisCache.Default.Get<string>("test");
        }

        public static void MongoTest()
        {
            MongoDbBase.Default.AddMongo(new MongoTest { Dt = DateTime.Now, Value = "MongoTest", Key = "test" }, "Test", "test");
        }
    }

    public class MongoTest : MongoBaseModel
    {
        public DateTime Dt { get; set; }
        public object Value { get; set; }
        public string Key { get; set; }
    }
}

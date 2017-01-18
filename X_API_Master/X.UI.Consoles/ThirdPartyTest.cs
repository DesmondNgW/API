using System;
using System.Threading;
using X.Util.Core.Common;
using X.Util.Entities;
using X.Util.Extend.Cache;
using X.Util.Extend.Mongo;

namespace X.UI.Consoles
{
    public class ThirdPartyTest
    {
        public static string CouchBaseTest(string key, string value)
        {
            CouchCache.Default.Set(key, value, DateTime.Now.AddMinutes(1));
            return CouchCache.Default.Get<string>(key);
        }

        public static void HighFrequencyTest(int sleep, Func<string, string, string> loader, int keyMax = 600)
        {
            var times = (double) 1000/sleep;
            int total = 0, suc = 0;
            while (true)
            {
                total++;
                var index = StringConvert.SysRandom.Next(keyMax);
                var key = "HighFrequencyKey" + index;
                var value = "HighFrequencyValue" + index;
                var result = loader(key, value);
                if (result == value) suc++;
                Console.WriteLine("key:{0}, value:{1}", key, result);
                var fail = total - suc;
                var rate = (double) suc/total;
                Console.WriteLine("当前运行次数：{0}, 成功获取数据次数：{1}, 获取数据失败次数：{2}, 成功率：{4}, 程序运行频率：{3} 次/秒。", total, suc, fail, times, rate);
                Thread.Sleep(sleep);
            }
        }

        public static string RedisTest(string key, string value)
        {
            RedisCache.Default.Set(key, value, DateTime.Now.AddMinutes(1));
            return RedisCache.Default.Get<string>(key);
        }

        public static void MongoTest()
        {
            MongoDbBase<MongoTest>.Default.SaveMongo(new MongoTest { Dt = DateTime.Now, Value = "MongoTest", Key = "test" }, "Test", "test");
        }
    }

    public class MongoTest : MongoBaseModel
    {
        public DateTime Dt { get; set; }
        public object Value { get; set; }
        public string Key { get; set; }
    }
}

using System;
using System.Diagnostics;
using X.Cache.Service;

namespace X.UI.Consoles
{
    public class CacheClientTest
    {
        public static void Test(int number)
        {
            if (number <= 1) number = 1;
            var client = new CacheClient("127.0.0.1", 12234, 1000);
            var c1 = 0;
            var c2 = 0;
            var total = new Stopwatch();
            total.Start();
            for (var i = 0; i < number; i++)
            {
                var key = Guid.NewGuid().ToString("N");
                var sw = new Stopwatch();

                sw.Start();
                var ret1 = client.Get(key);
                sw.Stop();
                Console.WriteLine("Get key {0},result {1}, ElapsedMilliseconds {2}", key, ret1, sw.ElapsedMilliseconds);
                if (sw.ElapsedMilliseconds > 0) c1++;
                c2++;

                sw.Restart();
                var ret2 = client.Set(key, key, DateTime.Now.AddMinutes(2));
                sw.Stop();
                Console.WriteLine("Set DateTime key {0},result {1}, ElapsedMilliseconds {2}", key, ret2, sw.ElapsedMilliseconds);
                if (sw.ElapsedMilliseconds > 0) c1++;
                c2++;

                sw.Restart();
                var ret3 = client.Get(key);
                sw.Stop();
                Console.WriteLine("Get key {0},result {1}, ElapsedMilliseconds {2}", key, ret3, sw.ElapsedMilliseconds);
                if (sw.ElapsedMilliseconds > 0) c1++;
                c2++;

                sw.Restart();
                var ret4 = client.Set(key, key, new TimeSpan(0, 2, 0));
                sw.Stop();
                Console.WriteLine("Set TimeSpan key {0},result {1}, ElapsedMilliseconds {2}", key, ret4, sw.ElapsedMilliseconds);
                if (sw.ElapsedMilliseconds > 0) c1++;
                c2++;
                c2++;
                sw.Restart();
                var ret5 = client.Get(key);
                sw.Stop();
                Console.WriteLine("Get key {0},result {1}, ElapsedMilliseconds {2}", key, ret5, sw.ElapsedMilliseconds);
                if (sw.ElapsedMilliseconds > 0) c1++;
                c2++;

                sw.Restart();
                var ret6 = client.Remove(key);
                sw.Stop();
                Console.WriteLine("Remove key {0},result {1}, ElapsedMilliseconds {2}", key, ret6, sw.ElapsedMilliseconds);
                if (sw.ElapsedMilliseconds > 0) c1++;
                c2++;

                sw.Restart();
                var ret7 = client.Get(key);
                sw.Stop();
                Console.WriteLine("Get key {0},result {1}, ElapsedMilliseconds {2}", key, ret7, sw.ElapsedMilliseconds);
                if (sw.ElapsedMilliseconds > 0) c1++;
                c2++;
            }
            total.Stop();
            Console.WriteLine("above zoro {0}, total {1}", c1, c2);
            Console.WriteLine("Total ElapsedMilliseconds {0}", total.ElapsedMilliseconds);
        }
    }
}

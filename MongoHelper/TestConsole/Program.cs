using System;
using System.Threading;
using MongoDbHelper;

namespace TestConsole
{
    class Program
    {
        static void Main()
        {
            MongoDbConfig.Config("mongodb://127.0.0.1:27017");
            const int threadCount = 2;
            var pools = new Thread[threadCount];
            for (var i = 0; i < threadCount; i++)
            {
                pools[i] = new Thread(RunTest) { IsBackground = true };
                pools[i].Start(i);
            }
            Console.WriteLine(@"all threads created.");
            Console.ReadLine();
        }

        static void RunTest(object obj)
        {
            var cursor = Convert.ToInt32(obj);
            for (var i = 0; i < 1; i++)
            {
                object o = new
                {
                    _id = cursor + "_" + i,
                    value = "this is a text string for test"
                };
                using (var mc = new MongoDbProvider("Route", "Data"))
                {
                    mc.Collection.Save(o);
                }
            }
            MongoDbConfig.ShowStatus();
            Console.WriteLine(string.Format(@"current thread threadId : {0} end.", Thread.CurrentThread.ManagedThreadId));
        }

        private class Task
        {
            //SharedState sharedState;
            private readonly int _num;
            public Task(int num)
            {
                _num = num;
            }

            public void DoTheTask()
            {
                var type = _num % 2;
                const int totle = 2000;
                for (var i = 0; i < totle; i++)
                {
                    //var obj = new ModelBase {Id = i.ToString()};
                    switch (type)
                    {
                        case 0:
                            using (var mc = new MongoDbProvider("Test5", "Mongo27016"))
                            {
                                var obj1 = new Model1
                                {
                                    Id = i + "-" + _num,
                                    Model = "model1-" + i
                                };
                                mc.Collection.Save(obj1);
                            }
                            break;
                        case 1:
                            using (var mc = new MongoDbProvider("Test2", "Mongo27017"))
                            {
                                var obj2 = new Model2
                                {
                                    Id = i + "-" + _num,
                                    Model = "model2-" + i
                                };
                                mc.Collection.Save(obj2);
                            }
                            break;
                    }
                    Console.WriteLine(_num + @" --> " + i);
                }
            }
        }

        private class SharedState
        {
            public int State { get; set; }
        }

        public class ModelBase
        {
            public string Id { get; set; }
        }


        public class Model1 : ModelBase
        {
            public string Model { get; set; }
            public string Value1 { get; set; }
            public string Value2 { get; set; }
        }

        public class Model2 : ModelBase
        {
            public string Model { get; set; }
        }

        public class Model3 : ModelBase
        {
            public string Model { get; set; }
        }
    }

    enum Colors { Red, Green, Blue, Yellow };
}

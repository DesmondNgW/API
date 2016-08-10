using System;
using System.Collections.Generic;
using System.Threading;
using MongoDbHelper;

namespace TestConsole
{
    public class TestModel
    {
        public string Id { get; set; }

        public string Text { get; set; }
    }


    class Program
    {
        static void Main()
        {
            MongoDbConfig.Config("mongodb://172.16.86.101:40041?username=mongodbadmin&password=q1w2e3r4");
            const int threadCount = 1;
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
            //var list = new List<TestModel>();
            for (var i = 0; i < 1; i++)
            {
                //var o = new BsonDocument();
                //o.Add("_id", cursor + "_" + i);
                //o.Add("Text", "this is a text string for test");
                var o = new TestModel
                {
                    Id = cursor + "_" + i,
                    Text = "this is a text string for test www  2222"
                };
                using (var mc = new MongoDbProvider("newdatabase", "Data"))
                {
                    //mc.ICollection.InsertOne(o);
                    //mc.ICollection.ReplaceOne(m => true, o, new UpdateOptions() {IsUpsert = true});
                    //var a = mc.ICollection.Find(m => m.Id == cursor + "_" + i).ToList();
                }
            }

            MongoDbConfig.ShowStatus();
            Console.WriteLine(@"current thread threadId : {0} end.", Thread.CurrentThread.ManagedThreadId);
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
                List<Model1> list1 = new List<Model1>();
                List<Model2> list2 = new List<Model2>();
                for (var i = 0; i < totle; i++)
                {
                    //var obj = new ModelBase {Id = i.ToString()};
                    switch (type)
                    {
                        case 0:
                            list1.Add(new Model1
                            {
                                Id = i + "-" + _num,
                                Model = "model1-" + i
                            });
                            break;
                        case 1:
                            list2.Add(new Model2
                            {
                                Id = i + "-" + _num,
                                Model = "model2-" + i
                            });
                            break;
                    }
                    Console.WriteLine(_num + @" --> " + i);
                }
                using (var mc = new MongoDbProvider("Test5", "Mongo27016"))
                {
                    mc.Collection.InsertBatch(list1);
                }

                using (var mc = new MongoDbProvider("Test2", "Mongo27017"))
                {
                    mc.Collection.InsertBatch(list2);
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

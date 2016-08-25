using System;
using X.Util.Entities;

namespace X.Util.Extend.Mongo
{
    /// <summary>
    /// 日志监控
    /// </summary>
    public class LogMonitor
    {
        public static void Info<T>(T t, LogMonitorDomain domain) where T : MongoBaseModel
        {
            MongoDbBase<T>.Default.SaveMongo(t, "LogMonitor-Info", null, string.Format("{0}.{1}", domain, DateTime.Now.ToString("yyyy.MM")));
        }

        public static void Error<T>(T t, LogMonitorDomain domain) where T : MongoBaseModel
        {
            MongoDbBase<T>.Default.SaveMongo(t, "LogMonitor-Error", null, string.Format("{0}.{1}", domain, DateTime.Now.ToString("yyyy.MM")));
        }

        public static void Debug<T>(T t, LogMonitorDomain domain) where T : MongoBaseModel
        {
            MongoDbBase<T>.Default.SaveMongo(t, "LogMonitor-Debug", null, string.Format("{0}.{1}", domain, DateTime.Now.ToString("yyyy.MM")));
        }
    }
}

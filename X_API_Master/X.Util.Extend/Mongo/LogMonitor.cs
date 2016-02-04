using System;
using X.Util.Core;
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
            MongoDbBase.Default.SaveMongo(t, "LogMonitor-Info", string.Format("{0}.{1}", domain, DateTime.Now.ToString("yyyy.MM")));
        }

        public static void Error<T>(T t, LogMonitorDomain domain) where T : MongoBaseModel
        {
            MongoDbBase.Default.SaveMongo(t, "LogMonitor-Error", string.Format("{0}.{1}", domain, DateTime.Now.ToString("yyyy.MM")));
        }

        public static void Debug<T>(T t, LogMonitorDomain domain) where T : MongoBaseModel
        {
            MongoDbBase.Default.SaveMongo(t, "LogMonitor-Debug", string.Format("{0}.{1}", domain, DateTime.Now.ToString("yyyy.MM")));
        }
    }
}

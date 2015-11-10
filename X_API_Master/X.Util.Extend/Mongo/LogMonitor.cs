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
            MongoDbBase.Default.AddMongo(t, "LogMonitor-Info", domain.ToString());
        }

        public static void Error<T>(T t, LogMonitorDomain domain) where T : MongoBaseModel
        {
            MongoDbBase.Default.AddMongo(t, "LogMonitor-Error", domain.ToString());
        }

        public static void Debug<T>(T t, LogMonitorDomain domain) where T : MongoBaseModel
        {
            MongoDbBase.Default.AddMongo(t, "LogMonitor-Debug", domain.ToString());
        }
    }
}

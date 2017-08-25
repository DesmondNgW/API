using System.ServiceProcess;
using System.Web;
using X.Util.Core;
using X.Util.Core.Cache;
using X.Util.Core.Configuration;
using X.Util.Core.Kernel;

namespace X.Cache.Service
{
    public partial class CacheService : ServiceBase
    {
        public CacheService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            var remoteIpAddress = ConfigurationHelper.GetAppSetting("IpAddress");
            var remotePort = ConfigurationHelper.GetAppSetting("Port").Convert2Int32(12234);
            NetworkCommsHelper.StartListening(remoteIpAddress, remotePort);

            NetworkCommsHelper.Reply<string, object>("Get", (p, c, t) => HttpRuntime.Cache.Get(t));

            NetworkCommsHelper.Reply<SendModel, bool>("Set.DateTime", (p, c, t) =>
            {
                HttpRuntime.Cache.Insert(t.Key, t.Value, null, t.ExpireAt, System.Web.Caching.Cache.NoSlidingExpiration);
                return true;
            });

            NetworkCommsHelper.Reply<SendModel, bool>("Set.TimeSpan", (p, c, t) =>
            {
                HttpRuntime.Cache.Insert(t.Key, t.Value, null, System.Web.Caching.Cache.NoAbsoluteExpiration, t.Expire);
                return true;
            });

            NetworkCommsHelper.Reply<string, bool>("Remove", (p, c, t) =>
            {
                HttpRuntime.Cache.Remove(t);
                return true;
            });
        }

        protected override void OnStop()
        {
            var remoteIpAddress = ConfigurationHelper.GetAppSetting("IpAddress");
            var remotePort = ConfigurationHelper.GetAppSetting("Port").Convert2Int32(12234);
            NetworkCommsHelper.StopListening(remoteIpAddress, remotePort);
        }
    }
}

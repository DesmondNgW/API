using System.ServiceProcess;
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

            NetworkCommsHelper.Reply<string, object>("Get", (p, c, t) => LocalCache.Get(t));

            NetworkCommsHelper.Reply<SendModel, bool>("Set.DateTime", (p, c, t) =>
            {
                LocalCache.Set(t.Key, t.Value, t.ExpireAt);
                return true;
            });

            NetworkCommsHelper.Reply<SendModel, bool>("Set.TimeSpan", (p, c, t) =>
            {
                LocalCache.Set(t.Key, t.Value, t.Expire);
                return true;
            });

            NetworkCommsHelper.Reply<string, bool>("Set.TimeSpan", (p, c, t) =>
            {
                LocalCache.Remove(t);
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

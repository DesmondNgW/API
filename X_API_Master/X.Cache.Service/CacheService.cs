using System.ServiceProcess;
using X.Util.Core;
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
            var server = new CoreTcpServer<ICache>(remoteIpAddress, remotePort, null);
            server.StartListening();
            server.RegisterAllRequestHandler(new CacheManager());
        }

        protected override void OnStop()
        {
            var remoteIpAddress = ConfigurationHelper.GetAppSetting("IpAddress");
            var remotePort = ConfigurationHelper.GetAppSetting("Port").Convert2Int32(12234);
            var server = new CoreTcpServer<ICache>(remoteIpAddress, remotePort, null);
            server.StopListening();
        }
    }
}

using System.Net.NetworkInformation;

namespace X.UI.Consoles
{
    internal class MethodHelper
    {
        public static void GetTcpConnections(string ip, int port)
        {
            var properties = IPGlobalProperties.GetIPGlobalProperties();
            var connections = properties.GetActiveTcpConnections();
            foreach (var t in connections)
            {
                if (!string.IsNullOrEmpty(ip) && !t.RemoteEndPoint.Address.ToString().Contains(ip)) continue;
                if (port > 0 && t.RemoteEndPoint.Port != port) continue;
                Console.Write("Local endpoint: {0} ", t.LocalEndPoint.ToString());
                Console.Write("Remote endpoint: {0} ", t.RemoteEndPoint.ToString());
                Console.WriteLine("{0}", t.State);
            }
        }

        public static void MonitorTcpConnection()
        {
            while (true)
            {
                Console.Clear();
                GetTcpConnections(string.Empty, default);
                Thread.Sleep(60000);
            }
        }
    }
}

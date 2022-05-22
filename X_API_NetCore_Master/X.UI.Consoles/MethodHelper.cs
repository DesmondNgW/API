using X.UI.Util.Helper;

namespace X.UI.Consoles
{
    internal class MethodHelper
    {
        public static void GetTcpConnections(string ip, int port)
        {
            var list = MonitorHelper.GetTcpConnection(ip, port);
            foreach (var t in list)
            {
                Console.WriteLine("Local endpoint: {0},Remote endpoint: {1}, {2}", t.Local, t.Remote, t.TcpState);
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

using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using X.UI.Util.Model;
using X.Util.Core.Kernel;

namespace X.UI.Util.Helper
{
    public class MonitorHelper
    {
        public static List<TcpConnectionInfo> GetTcpConnection(string ip, int port)
        {
            var ret = new List<TcpConnectionInfo>();
            var connections = IPGlobalProperties.GetIPGlobalProperties()?.GetActiveTcpConnections();
            if (connections != null)
            {
                foreach (var t in connections)
                {
                    if (IpBase.IpValid(ip) && !t.RemoteEndPoint.Address.ToString().Contains(ip)) continue;
                    if (port > 0 && t.RemoteEndPoint.Port != port) continue;
                    ret.Add(new TcpConnectionInfo
                    {
                        Local = t.LocalEndPoint.ToString(),
                        Remote = t.RemoteEndPoint.ToString(),
                        TcpState = t.State.ToString()
                    });
                }
            }
            return ret;
        }
    }
}

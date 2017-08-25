using System;
using System.Net;
using System.Reflection;
using NetworkCommsDotNet;
using NetworkCommsDotNet.Connections;
using NetworkCommsDotNet.Connections.TCP;
using X.Util.Core.Log;
using X.Util.Entities.Enums;

namespace X.Util.Core.Kernel
{
    public class NetworkCommsHelper
    {
        /// <summary>
        /// 监听Tcp链接
        /// </summary>
        /// <param name="remoteIpAddress"></param>
        /// <param name="remotePort"></param>
        public static void StartListening(string remoteIpAddress, int remotePort)
        {
            Connection.StartListening(ConnectionType.TCP, new IPEndPoint(IPAddress.Parse(remoteIpAddress), remotePort));
        }

        /// <summary>
        /// 关闭Tcp监听
        /// </summary>
        /// <param name="remoteIpAddress"></param>
        /// <param name="remotePort"></param>
        public static void StopListening(string remoteIpAddress, int remotePort)
        {
            Connection.StopListening(ConnectionType.TCP, new IPEndPoint(IPAddress.Parse(remoteIpAddress), remotePort));
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="remoteIpAddress"></param>
        /// <param name="remotePort"></param>
        /// <param name="methodName"></param>
        /// <param name="obj"></param>
        /// <param name="timeOutMilliSeconds"></param>
        /// <returns></returns>
        public static TReply Send<TRequest, TReply>(string remoteIpAddress, int remotePort, string methodName, TRequest obj, int timeOutMilliSeconds)
        {
            try
            {
                var connectionInfo = new ConnectionInfo(remoteIpAddress, remotePort);
                var serverConnection = TCPConnection.GetConnection(connectionInfo);
                return serverConnection.SendReceiveObject<string, string>(methodName, methodName + typeof(TReply).FullName, timeOutMilliSeconds, obj.ToJson()).FromJson<TReply>();
            }
            catch (Exception e)
            {
                Logger.Client.Error(Logger.Client.GetMethodInfo(MethodBase.GetCurrentMethod(), new object[] { remoteIpAddress, remotePort, methodName, obj, timeOutMilliSeconds }), e, LogDomain.Util);
            }
            return default(TReply);
        }

        /// <summary>
        /// 注册消息处理器
        /// </summary>
        /// <param name="methodName"></param>
        /// <param name="func"></param>
        public static void Reply<TRequest, TReply>(string methodName, Func<PacketHeader, Connection, TRequest, TReply> func)
        {
            NetworkComms.AppendGlobalIncomingPacketHandler<string>(methodName, (packetHeader, connection, request) =>
            {
                try
                {
                    var ret = func(packetHeader, connection, request.FromJson<TRequest>());
                    connection.SendObject(methodName + typeof(TReply).FullName, ret.ToJson());
                }
                catch(Exception e)
                {
                    Logger.Client.Error(Logger.Client.GetMethodInfo(MethodBase.GetCurrentMethod(), new object[] { methodName }), e, LogDomain.Util);
                }
            });
        }
    }
}

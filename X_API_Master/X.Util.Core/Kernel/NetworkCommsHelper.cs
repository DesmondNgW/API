using System;
using System.Linq;
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
        /// GetRequestId
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        public static string GetRequestId<T>(MethodBase method)
        {
            var methodFullName = typeof(T).FullName + "." + method.Name;
            return method.GetParameters().OrderBy(p => p.Position).Aggregate(methodFullName, (current, p) => current + ("_" + p.Name));
        }

        /// <summary>
        /// GetIpEndPoint
        /// </summary>
        /// <param name="remoteIpAddress"></param>
        /// <param name="remotePort"></param>
        /// <returns></returns>
        public static IPEndPoint GetIpEndPoint(string remoteIpAddress, int remotePort)
        {
            IPAddress address;
            return IPAddress.TryParse(remoteIpAddress, out address) ? new IPEndPoint(address, remotePort) : default(IPEndPoint);
        }

        /// <summary>
        /// 监听Tcp链接
        /// </summary>
        /// <param name="connectionType"></param>
        /// <param name="iPEndPoint"></param>
        public static void StartListening(ConnectionType connectionType, IPEndPoint iPEndPoint)
        {
            Connection.StartListening(connectionType, iPEndPoint);
        }

        /// <summary>
        /// 关闭Tcp监听
        /// </summary>
        /// <param name="connectionType"></param>
        /// <param name="iPEndPoint"></param>
        public static void StopListening(ConnectionType connectionType, IPEndPoint iPEndPoint)
        {
            Connection.StopListening(connectionType, iPEndPoint);
        }

        #region SendRequest
        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="remoteIpAddress"></param>
        /// <param name="remotePort"></param>
        /// <param name="requestId"></param>
        /// <param name="obj"></param>
        /// <param name="timeOutMilliSeconds"></param>
        /// <returns></returns>
        public static TReply SendRequest<TRequest, TReply>(string remoteIpAddress, int remotePort, string requestId, TRequest obj, int timeOutMilliSeconds)
        {
            try
            {
                var connectionInfo = new ConnectionInfo(remoteIpAddress, remotePort);
                var serverConnection = TCPConnection.GetConnection(connectionInfo);
                return serverConnection.SendReceiveObject<string, string>(requestId, requestId + typeof(TReply).FullName, timeOutMilliSeconds, obj.ToJson()).FromJson<TReply>();
            }
            catch (Exception e)
            {
                Logger.Client.Error(Logger.Client.GetMethodInfo(MethodBase.GetCurrentMethod(), new object[] { remoteIpAddress, remotePort, requestId, obj, timeOutMilliSeconds }), e, LogDomain.Util);
            }
            return default(TReply);
        }

        /// <summary>
        /// 发送消息无参数
        /// </summary>
        /// <typeparam name="TReply"></typeparam>
        /// <param name="remoteIpAddress"></param>
        /// <param name="remotePort"></param>
        /// <param name="requestId"></param>
        /// <param name="timeOutMilliSeconds"></param>
        /// <returns></returns>
        public static TReply SendRequest<TReply>(string remoteIpAddress, int remotePort, string requestId, int timeOutMilliSeconds)
        {
            try
            {
                var connectionInfo = new ConnectionInfo(remoteIpAddress, remotePort);
                var serverConnection = TCPConnection.GetConnection(connectionInfo);
                return serverConnection.SendReceiveObject<string>(requestId, requestId + typeof(TReply).FullName, timeOutMilliSeconds).FromJson<TReply>();
            }
            catch (Exception e)
            {
                Logger.Client.Error(Logger.Client.GetMethodInfo(MethodBase.GetCurrentMethod(), new object[] { remoteIpAddress, remotePort, requestId, timeOutMilliSeconds }), e, LogDomain.Util);
            }
            return default(TReply);
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="remoteIpAddress"></param>
        /// <param name="remotePort"></param>
        /// <param name="requestId"></param>
        /// <param name="obj"></param>
        /// <param name="timeOutMilliSeconds"></param>
        /// <param name="returnType"></param>
        /// <returns></returns>
        public static object SendRequest(string remoteIpAddress, int remotePort, string requestId, object obj, int timeOutMilliSeconds, Type returnType)
        {
            if (returnType == typeof(void))
            {
                SendRequest(remoteIpAddress, remotePort, requestId, obj);
                return null;
            }
            try
            {
                var connectionInfo = new ConnectionInfo(remoteIpAddress, remotePort);
                var serverConnection = TCPConnection.GetConnection(connectionInfo);
                return serverConnection.SendReceiveObject<string, string>(requestId, requestId + returnType.FullName, timeOutMilliSeconds, obj.ToJson()).FromJson(returnType);
            }
            catch (Exception e)
            {
                Logger.Client.Error(Logger.Client.GetMethodInfo(MethodBase.GetCurrentMethod(), new[] { remoteIpAddress, remotePort, requestId, obj, timeOutMilliSeconds, returnType }), e, LogDomain.Util);
            }
            return null;
        }

        /// <summary>
        /// 发送消息 无参数
        /// </summary>
        /// <param name="remoteIpAddress"></param>
        /// <param name="remotePort"></param>
        /// <param name="requestId"></param>
        /// <param name="timeOutMilliSeconds"></param>
        /// <param name="returnType"></param>
        /// <returns></returns>
        public static object SendRequest(string remoteIpAddress, int remotePort, string requestId, int timeOutMilliSeconds, Type returnType)
        {
            if (returnType == typeof(void))
            {
                SendRequest(remoteIpAddress, remotePort, requestId, timeOutMilliSeconds);
                return null;
            }
            try
            {
                var connectionInfo = new ConnectionInfo(remoteIpAddress, remotePort);
                var serverConnection = TCPConnection.GetConnection(connectionInfo);
                return serverConnection.SendReceiveObject<string>(requestId, requestId + returnType.FullName, timeOutMilliSeconds).FromJson(returnType);
            }
            catch (Exception e)
            {
                Logger.Client.Error(Logger.Client.GetMethodInfo(MethodBase.GetCurrentMethod(), new object[] { remoteIpAddress, remotePort, requestId, timeOutMilliSeconds, returnType }), e, LogDomain.Util);
            }
            return null;
        }

        /// <summary>
        /// 发送消息无返回值
        /// </summary>
        /// <typeparam name="TRequest"></typeparam>
        /// <param name="remoteIpAddress"></param>
        /// <param name="remotePort"></param>
        /// <param name="requestId"></param>
        /// <param name="obj"></param>
        public static void SendRequest<TRequest>(string remoteIpAddress, int remotePort, string requestId, TRequest obj)
        {
            try
            {
                var connectionInfo = new ConnectionInfo(remoteIpAddress, remotePort);
                var serverConnection = TCPConnection.GetConnection(connectionInfo);
                serverConnection.SendObject(requestId, obj.ToJson());
            }
            catch (Exception e)
            {
                Logger.Client.Error(Logger.Client.GetMethodInfo(MethodBase.GetCurrentMethod(), new object[] { remoteIpAddress, remotePort, requestId, obj }), e, LogDomain.Util);
            }
        }

        /// <summary>
        /// 无参数,无返回
        /// </summary>
        /// <param name="remoteIpAddress"></param>
        /// <param name="remotePort"></param>
        /// <param name="requestId"></param>
        public static void SendRequest(string remoteIpAddress, int remotePort, string requestId)
        {
            try
            {
                var connectionInfo = new ConnectionInfo(remoteIpAddress, remotePort);
                var serverConnection = TCPConnection.GetConnection(connectionInfo);
                serverConnection.SendObject(requestId);
            }
            catch (Exception e)
            {
                Logger.Client.Error(Logger.Client.GetMethodInfo(MethodBase.GetCurrentMethod(), new object[] { remoteIpAddress, remotePort, requestId }), e, LogDomain.Util);
            }
        }

        /// <summary>
        /// 发送消息无返回值
        /// </summary>
        /// <param name="remoteIpAddress"></param>
        /// <param name="remotePort"></param>
        /// <param name="requestId"></param>
        /// <param name="obj"></param>
        public static void SendRequest(string remoteIpAddress, int remotePort, string requestId, object obj)
        {
            try
            {
                var connectionInfo = new ConnectionInfo(remoteIpAddress, remotePort);
                var serverConnection = TCPConnection.GetConnection(connectionInfo);
                serverConnection.SendObject(requestId, obj.ToJson());
            }
            catch (Exception e)
            {
                Logger.Client.Error(Logger.Client.GetMethodInfo(MethodBase.GetCurrentMethod(), new[] { remoteIpAddress, remotePort, requestId, obj }), e, LogDomain.Util);
            }
        }
        
        #endregion

        #region RegisterRequestHandler
		/// <summary>
        /// 注册消息处理器
        /// </summary>
        /// <param name="requestId"></param>
        /// <param name="packet"></param>
        /// <param name="func"></param>
        public static void RegisterRequestHandler<TRequest, TReply>(string requestId, Func<PacketHeader, Connection, bool> packet, Func<TRequest, TReply> func)
        {
            NetworkComms.AppendGlobalIncomingPacketHandler<string>(requestId, (packetHeader, connection, request) =>
            {
                try
                {
                    if (packet(packetHeader, connection))
                    {
                        var ret = func(request.FromJson<TRequest>());
                        connection.SendObject(requestId + typeof (TReply).FullName, ret.ToJson());
                    }
                    else
                    {
                        connection.SendObject(requestId + typeof(TReply).FullName, default(TReply));
                        Logger.Client.Error(Logger.Client.GetMethodInfo(MethodBase.GetCurrentMethod(), new object[] { packetHeader, connection }), new Exception("Tcp status or packetHeader Error."), LogDomain.Util);
                    }
                }
                catch(Exception e)
                {
                    Logger.Client.Error(Logger.Client.GetMethodInfo(MethodBase.GetCurrentMethod(), new object[] { requestId, packetHeader, connection, request }), e, LogDomain.Util);
                }
            });
        }

        /// <summary>
        /// 注册消息处理器 无参数
        /// </summary>
        /// <typeparam name="TReply"></typeparam>
        /// <param name="requestId"></param>
        /// <param name="packet"></param>
        /// <param name="func"></param>
        public static void RegisterRequestHandler<TReply>(string requestId, Func<PacketHeader, Connection, bool> packet, Func<TReply> func)
        {
            NetworkComms.AppendGlobalIncomingPacketHandler<string>(requestId, (packetHeader, connection, request) =>
            {
                try
                {
                    if (packet(packetHeader, connection))
                    {
                        var ret = func();
                        connection.SendObject(requestId + typeof(TReply).FullName, ret.ToJson());
                    }
                    else
                    {
                        connection.SendObject(requestId + typeof(TReply).FullName, default(TReply));
                        Logger.Client.Error(Logger.Client.GetMethodInfo(MethodBase.GetCurrentMethod(), new object[] { packetHeader, connection }), new Exception("Tcp status or packetHeader Error."), LogDomain.Util);
                    }
                }
                catch (Exception e)
                {
                    Logger.Client.Error(Logger.Client.GetMethodInfo(MethodBase.GetCurrentMethod(), new object[] { requestId, packetHeader, connection }), e, LogDomain.Util);
                }
            });
        }

        /// <summary>
        /// 注册消息处理器
        /// </summary>
        /// <param name="requestId"></param>
        /// <param name="packet"></param>
        /// <param name="method"></param>
        /// <param name="target"></param>
        /// <param name="incomeType"></param>
        /// <param name="returnType"></param>
        public static void RegisterRequestHandler(string requestId, Func<PacketHeader, Connection, bool> packet, MethodBase method, object target, Type incomeType, Type returnType)
        {
            if (returnType == typeof (void))
            {
                RegisterRequestHandlerVoid(requestId, packet, method, target, incomeType);
                return;
            }
            if (incomeType == null)
            {
                RegisterRequestHandlerReturn(requestId, packet, method, target, returnType);
                return;
            }
            NetworkComms.AppendGlobalIncomingPacketHandler<string>(requestId, (packetHeader, connection, request) =>
            {
                try
                {
                    if (packet(packetHeader, connection))
                    {
                        var ret = method.Invoke(target, new[] {request.FromJson(incomeType)});
                        connection.SendObject(requestId + returnType.FullName, ret.ToJson());
                    }
                    else
                    {
                        connection.SendObject(requestId + returnType.FullName, string.Empty);
                        Logger.Client.Error(Logger.Client.GetMethodInfo(MethodBase.GetCurrentMethod(), new object[] { packetHeader, connection }), new Exception("Tcp status or packetHeader Error."), LogDomain.Util);
                    }
                }
                catch (Exception e)
                {
                    Logger.Client.Error(Logger.Client.GetMethodInfo(MethodBase.GetCurrentMethod(), new object[] { requestId, packetHeader, connection, request }), e, LogDomain.Util);
                }
            });
        }

        /// <summary>
        /// 注册消息处理器 无参数
        /// </summary>
        /// <param name="requestId"></param>
        /// <param name="packet"></param>
        /// <param name="method"></param>
        /// <param name="target"></param>
        /// <param name="returnType"></param>
        public static void RegisterRequestHandlerReturn(string requestId, Func<PacketHeader, Connection, bool> packet, MethodBase method, object target, Type returnType)
        {
            if (returnType == typeof(void))
            {
                RegisterRequestHandler(requestId, packet, method, target);
                return;
            }
            NetworkComms.AppendGlobalIncomingPacketHandler<string>(requestId, (packetHeader, connection, request) =>
            {
                try
                {
                    if (packet(packetHeader, connection))
                    {
                        var ret = method.Invoke(target, null);
                        connection.SendObject(requestId + returnType.FullName, ret.ToJson());
                    }
                    else
                    {
                        connection.SendObject(requestId + returnType.FullName, string.Empty);
                        Logger.Client.Error(Logger.Client.GetMethodInfo(MethodBase.GetCurrentMethod(), new object[] { packetHeader, connection }), new Exception("Tcp status or packetHeader Error."), LogDomain.Util);
                    }
                }
                catch (Exception e)
                {
                    Logger.Client.Error(Logger.Client.GetMethodInfo(MethodBase.GetCurrentMethod(), new object[] { requestId, packetHeader, connection, request }), e, LogDomain.Util);
                }
            });
        }

        /// <summary>
        /// 注册消息处理器
        /// </summary>
        /// <typeparam name="TRequest"></typeparam>
        /// <param name="requestId"></param>
        /// <param name="packet"></param>
        /// <param name="func"></param>
        public static void RegisterRequestHandler<TRequest>(string requestId, Func<PacketHeader, Connection, bool> packet, Action<TRequest> func)
        {
            NetworkComms.AppendGlobalIncomingPacketHandler<string>(requestId, (packetHeader, connection, request) =>
            {
                try
                {
                    if (packet(packetHeader, connection))
                    {
                        func(request.FromJson<TRequest>());
                    }
                    else
                    {
                        Logger.Client.Error(Logger.Client.GetMethodInfo(MethodBase.GetCurrentMethod(), new object[] { packetHeader, connection }), new Exception("Tcp status or packetHeader Error."), LogDomain.Util);
                    }
                }
                catch (Exception e)
                {
                    Logger.Client.Error(Logger.Client.GetMethodInfo(MethodBase.GetCurrentMethod(), new object[] { requestId, packetHeader, connection, request }), e, LogDomain.Util);
                }
            });
        }

        /// <summary>
        /// 无参数，无返回
        /// </summary>
        /// <param name="requestId"></param>
        /// <param name="packet"></param>
        /// <param name="func"></param>
        public static void RegisterRequestHandler(string requestId, Func<PacketHeader, Connection, bool> packet, Action func)
        {
            NetworkComms.AppendGlobalIncomingPacketHandler<string>(requestId, (packetHeader, connection, request) =>
            {
                try
                {
                    if (packet(packetHeader, connection))
                    {
                        func();
                    }
                    else
                    {
                        Logger.Client.Error(Logger.Client.GetMethodInfo(MethodBase.GetCurrentMethod(), new object[] { packetHeader, connection }), new Exception("Tcp status or packetHeader Error."), LogDomain.Util);
                    }
                }
                catch (Exception e)
                {
                    Logger.Client.Error(Logger.Client.GetMethodInfo(MethodBase.GetCurrentMethod(), new object[] { requestId, packetHeader, connection }), e, LogDomain.Util);
                }
            });
        }
        
        /// <summary>
        /// 注册消息处理器
        /// </summary>
        /// <param name="requestId"></param>
        /// <param name="packet"></param>
        /// <param name="method"></param>
        /// <param name="target"></param>
        /// <param name="incomeType"></param>
        public static void RegisterRequestHandlerVoid(string requestId, Func<PacketHeader, Connection, bool> packet, MethodBase method, object target, Type incomeType)
        {
            if (incomeType == null)
            {
                RegisterRequestHandler(requestId, packet, method, target);
                return;
            }
            NetworkComms.AppendGlobalIncomingPacketHandler<string>(requestId, (packetHeader, connection, request) =>
            {
                try
                {
                    if (packet(packetHeader, connection))
                    {
                        method.Invoke(target, new[] { request.FromJson(incomeType) });
                    }
                    else
                    {
                        Logger.Client.Error(Logger.Client.GetMethodInfo(MethodBase.GetCurrentMethod(), new object[] { packetHeader, connection }), new Exception("Tcp status or packetHeader Error."), LogDomain.Util);
                    }
                }
                catch (Exception e)
                {
                    Logger.Client.Error(Logger.Client.GetMethodInfo(MethodBase.GetCurrentMethod(), new object[] { requestId, packetHeader, connection, request }), e, LogDomain.Util);
                }
            });
        }

        /// <summary>
        /// 注册消息处理器 无参数,无返回
        /// </summary>
        /// <param name="requestId"></param>
        /// <param name="packet"></param>
        /// <param name="method"></param>
        /// <param name="target"></param>
        public static void RegisterRequestHandler(string requestId, Func<PacketHeader, Connection, bool> packet, MethodBase method, object target)
        {
            NetworkComms.AppendGlobalIncomingPacketHandler<string>(requestId, (packetHeader, connection, request) =>
            {
                try
                {
                    if (packet(packetHeader, connection))
                    {
                        method.Invoke(target, null);
                    }
                    else
                    {
                        Logger.Client.Error(Logger.Client.GetMethodInfo(MethodBase.GetCurrentMethod(), new object[] { packetHeader, connection }), new Exception("Tcp status or packetHeader Error."), LogDomain.Util);
                    }
                }
                catch (Exception e)
                {
                    Logger.Client.Error(Logger.Client.GetMethodInfo(MethodBase.GetCurrentMethod(), new object[] { requestId, packetHeader, connection }), e, LogDomain.Util);
                }
            });
        }
        #endregion
    }
}

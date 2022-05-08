using NetworkCommsDotNet;
using NetworkCommsDotNet.Connections;
using System;
using System.Linq;
using System.Net;
using System.Reflection;

namespace X.Util.Core.Kernel
{
    public class CoreTcpServer<TChannel> where TChannel : class
    {
        /// <summary>
        /// 监听地址
        /// </summary>
        public IPEndPoint IpEndPoint { get; set; }

        /// <summary>
        /// 处理包头
        /// </summary>
        public Func<PacketHeader, Connection, bool> Packet { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="ipAddress"></param>
        /// <param name="port"></param>
        /// <param name="packet"></param>
        public CoreTcpServer(string ipAddress, int port, Func<PacketHeader, Connection, bool> packet)
        {
            IpEndPoint = NetworkCommsHelper.GetIpEndPoint(ipAddress, port);
            if (Equals(IpEndPoint, default(IPEndPoint))) throw new Exception("ipAddress不合法");
            if (packet == null) packet = (p, c) => true;
            Packet = packet;
        }

        /// <summary>
        /// 开始监听
        /// </summary>
        /// <returns></returns>
        public CoreTcpServer<TChannel> StartListening()
        {
            NetworkCommsHelper.StartListening(ConnectionType.TCP, IpEndPoint);
            return this;
        }

        /// <summary>
        /// 结束监听
        /// </summary>
        /// <returns></returns>
        public CoreTcpServer<TChannel> StopListening()
        {
            NetworkCommsHelper.StopListening(ConnectionType.TCP, IpEndPoint);
            return this;
        }

        /// <summary>
        /// 注册方法
        /// </summary>
        /// <typeparam name="TRequest"></typeparam>
        /// <typeparam name="TReply"></typeparam>
        /// <param name="func"></param>
        /// <returns></returns>
        public CoreTcpServer<TChannel> RegisterRequestHandler<TRequest, TReply>(Func<TRequest, TReply> func)
        {
            var requestId = NetworkCommsHelper.GetRequestId<TChannel>(func.Method);
            NetworkCommsHelper.RegisterRequestHandler(requestId, Packet, func);
            return this;
        }

        /// <summary>
        /// 注册方法
        /// </summary>
        /// <typeparam name="TReply"></typeparam>
        /// <param name="func"></param>
        /// <returns></returns>
        public CoreTcpServer<TChannel> RegisterRequestHandler<TReply>(Func<TReply> func)
        {
            var requestId = NetworkCommsHelper.GetRequestId<TChannel>(func.Method);
            NetworkCommsHelper.RegisterRequestHandler(requestId, Packet, func);
            return this;
        }

        /// <summary>
        /// 注册方法
        /// </summary>
        /// <param name="method"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public CoreTcpServer<TChannel> RegisterRequestHandler(MethodInfo method, object target)
        {
            var requestId = NetworkCommsHelper.GetRequestId<TChannel>(method);
            var p = method.GetParameters().FirstOrDefault();
            Type incomeType = null;
            if (p != null)
            {
                incomeType = p.ParameterType;
            }
            var returnType = method.ReturnType;
            NetworkCommsHelper.RegisterRequestHandler(requestId, Packet, method, target, incomeType, returnType);
            return this;
        }

        /// <summary>
        /// 注册方法
        /// </summary>
        /// <typeparam name="TRequest"></typeparam>
        /// <param name="func"></param>
        /// <returns></returns>
        public CoreTcpServer<TChannel> RegisterRequestHandler<TRequest>(Action<TRequest> func)
        {
            var requestId = NetworkCommsHelper.GetRequestId<TChannel>(func.Method);
            NetworkCommsHelper.RegisterRequestHandler(requestId, Packet, func);
            return this;
        }

        /// <summary>
        /// 注册方法
        /// </summary>
        /// <param name="func"></param>
        /// <returns></returns>
        public CoreTcpServer<TChannel> RegisterRequestHandler(Action func)
        {
            var requestId = NetworkCommsHelper.GetRequestId<TChannel>(func.Method);
            NetworkCommsHelper.RegisterRequestHandler(requestId, Packet, func);
            return this;
        }

        /// <summary>
        /// RegisterAllRequestHandler
        /// </summary>
        /// <param name="target"></param>
        public void RegisterAllRequestHandler(object target)
        {
            var methods = typeof(TChannel).GetMethods();
            foreach (var method in methods)
            {
                RegisterRequestHandler(method, target);
            }
        }
    }
}

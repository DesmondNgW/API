//using System.Reflection;

//namespace X.Util.Core.Kernel
//{
//    public class CoreTcpClient<TChannel>
//    {
//        /// <summary>
//        /// ip地址
//        /// </summary>
//        public string IpAddress { get; set; }

//        /// <summary>
//        /// 端口
//        /// </summary>
//        public int Port { get; set; }

//        /// <summary>
//        /// 超时毫秒数
//        /// </summary>
//        public int TimeOutMilliSeconds { get; set; }

//        /// <summary>
//        /// 构造函数
//        /// </summary>
//        /// <param name="ipAddress"></param>
//        /// <param name="port"></param>
//        /// <param name="timeOutMilliSeconds"></param>
//        public CoreTcpClient(string ipAddress, int port, int timeOutMilliSeconds)
//        {
//            IpAddress = ipAddress;
//            Port = port;
//            TimeOutMilliSeconds = timeOutMilliSeconds;
//        }

//        /// <summary>
//        /// 调用服务端方法
//        /// </summary>
//        /// <typeparam name="TRequest"></typeparam>
//        /// <typeparam name="TReply"></typeparam>
//        /// <param name="method"></param>
//        /// <param name="obj"></param>
//        /// <returns></returns>
//        public TReply SendRequest<TRequest, TReply>(MethodBase method, TRequest obj)
//        {
//            var requestId = NetworkCommsHelper.GetRequestId<TChannel>(method);
//            return NetworkCommsHelper.SendRequest<TRequest, TReply>(IpAddress, Port, requestId, obj, TimeOutMilliSeconds);
//        }

//        /// <summary>
//        /// 调用服务端方法
//        /// </summary>
//        /// <typeparam name="TRequest"></typeparam>
//        /// <param name="method"></param>
//        /// <param name="obj"></param>
//        public void SendRequest<TRequest>(MethodBase method, TRequest obj)
//        {
//            var requestId = NetworkCommsHelper.GetRequestId<TChannel>(method);
//            NetworkCommsHelper.SendRequest(IpAddress, Port, requestId, obj);
//        }
//    }
//}

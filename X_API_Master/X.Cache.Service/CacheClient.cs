using System;
using X.Util.Core.Kernel;

namespace X.Cache.Service
{
    public class CacheClient : ICache
    {
        public string RemoteIpAddress { get; set; }

        public int RemotePort { get; set; }

        public CacheClient(string remoteIpAddress, int remotePort)
        {
            RemoteIpAddress = remoteIpAddress;
            RemotePort = remotePort;
        }

        public object Get(string key)
        {
            const string methodName = "Get";
            return NetworkCommsHelper.Send<string, object>(RemoteIpAddress, RemotePort, methodName, key, 1000);
        }

        public bool Set(string key, object value, DateTime expire)
        {
            const string methodName = "Set.DateTime";
            return NetworkCommsHelper.Send<SendModel, bool>(RemoteIpAddress, RemotePort, methodName, new SendModel
            {
                Key = key,
                Value = value,
                Expire = TimeSpan.Zero,
                ExpireAt = expire
            }, 1000);
        }

        public bool Set(string key, object value, TimeSpan expire)
        {
            const string methodName = "Set.TimeSpan";
            return NetworkCommsHelper.Send<SendModel, bool>(RemoteIpAddress, RemotePort, methodName, new SendModel
            {
                Key = key,
                Value = value,
                Expire = expire,
                ExpireAt = DateTime.MinValue
            }, 1000);
        }

        public bool Remove(string key)
        {
            const string methodName = "Remove";
            return NetworkCommsHelper.Send<string, bool>(RemoteIpAddress, RemotePort, methodName, key, 1000);
        }
    }
}

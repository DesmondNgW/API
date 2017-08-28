using System;
using System.Reflection;
using X.Util.Core.Kernel;

namespace X.Cache.Service
{
    public class CacheClient : ICache
    {
        private readonly CoreTcpClient<ICache> _client;

        public CacheClient(string ipAddress, int port, int timeOutMilliSeconds)
        {
            _client = new CoreTcpClient<ICache>(ipAddress, port, timeOutMilliSeconds);
        }

        public object Get(string key)
        {
            return _client.SendRequest<string, object>(MethodBase.GetCurrentMethod(), key);
        }

        public bool Set(SendModel send)
        {
            return _client.SendRequest<SendModel, bool>(MethodBase.GetCurrentMethod(), send);
        }

        public bool Set(string key, object value, TimeSpan expire)
        {
            return Set(new SendModel
            {
                Key = key,
                Value = value,
                Expire = expire,
                ExpireAt = DateTime.MinValue
            });
        }

        public bool Set(string key, object value, DateTime expire)
        {
            return Set(new SendModel
            {
                Key = key,
                Value = value,
                Expire = TimeSpan.Zero,
                ExpireAt = expire
            });
        }

        public bool Remove(string key)
        {
            return _client.SendRequest<string, bool>(MethodBase.GetCurrentMethod(), key);
        }
    }
}

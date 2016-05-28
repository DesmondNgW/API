using System;
using ServiceStack.Redis;
using X.Util.Core;
using X.Util.Core.Configuration;
using X.Util.Core.Kernel;
using X.Util.Entities;
using X.Util.Entities.Interface;
using X.Util.Extend.Core;
using X.Util.Provider;

namespace X.Util.Extend.Cache
{
    /// <summary>
    /// 缓存基础类
    /// </summary>
    public sealed class RedisCache : IRedisCache
    {
        #region 构造函数
        public readonly string ServerName = ConfigurationHelper.RedisDefaultServername;
        public static readonly IRedisCache Default = new RedisCache();
        private RedisCache() { }
        public RedisCache(string serverName)
        {
            ServerName = serverName;
        } 
        #endregion

        #region 对外公布的Api
        public T Get<T>(string key)
        {
            var redisProvider = new RedisProvider(ServerName);
            return CoreAccess<IRedisClient>.TryCall(redisProvider.ReadOnlyClient.Get<T>, key, redisProvider, new LogOptions<T>(CoreBase.CallSuccess));
        }

        public T GetJson<T>(string key)
        {
            var o = Get<string>(key);
            return o != null ? o.FromJson<T>() : default(T);
        }

        public void Set<T>(string key, T value)
        {
            var redisProvider = new RedisProvider(ServerName);
            CoreAccess<IRedisClient>.TryCallAsync(redisProvider.Client.Add, key, value, redisProvider, null, new LogOptions<bool>(CoreBase.CallSuccess, true, false));
        }

        public void SetJson<T>(string key, T value)
        {
            var redisProvider = new RedisProvider(ServerName);
            CoreAccess<IRedisClient>.TryCallAsync(redisProvider.Client.Add, key, value.ToJson(), redisProvider, null, new LogOptions<bool>(CoreBase.CallSuccess, true, false));
        }

        public void Set<T>(string key, T value, DateTime expire)
        {
            var redisProvider = new RedisProvider(ServerName);
            CoreAccess<IRedisClient>.TryCallAsync(redisProvider.Client.Add, key, value, expire, redisProvider, null, new LogOptions<bool>(CoreBase.CallSuccess, true, false));
        }

        public void SetJson<T>(string key, T value, DateTime expire)
        {
            var redisProvider = new RedisProvider(ServerName);
            CoreAccess<IRedisClient>.TryCallAsync(redisProvider.Client.Add, key, value.ToJson(), expire, redisProvider, null, new LogOptions<bool>(CoreBase.CallSuccess, true, false));
        }

        public void Set<T>(string key, T value, TimeSpan expire)
        {
            var redisProvider = new RedisProvider(ServerName);
            CoreAccess<IRedisClient>.TryCallAsync(redisProvider.Client.Add, key, value, expire, redisProvider, null, new LogOptions<bool>(CoreBase.CallSuccess, true, false));
        }
        public void SetJson<T>(string key, T value, TimeSpan expire)
        {
            var redisProvider = new RedisProvider(ServerName);
            CoreAccess<IRedisClient>.TryCallAsync(redisProvider.Client.Add, key, value.ToJson(), expire, redisProvider, null, new LogOptions<bool>(CoreBase.CallSuccess, true, false));
        }

        public void Remove(string key)
        {
            var redisProvider = new RedisProvider(ServerName);
            CoreAccess<IRedisClient>.TryCallAsync(redisProvider.Client.Remove, key, redisProvider, null, new LogOptions<bool>(CoreBase.CallSuccess, true, false));
        }
        #endregion
    }
}

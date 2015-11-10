using System;
using X.Util.Core;
using X.Util.Extend.Core;
using X.Util.Provider;

namespace X.Util.Extend.Cache
{
    /// <summary>
    /// 缓存基础类
    /// </summary>
    public sealed class RedisCache
    {
        #region 构造函数
        public readonly string ServerName = ConfigurationHelper.RedisDefaultServername;
        public static readonly RedisCache Default = new RedisCache();
        public const LogDomain EDomain = LogDomain.ThirdParty;
        public RedisCache() { }
        public RedisCache(string serverName)
        {
            ServerName = serverName;
        } 
        #endregion

        #region 对外公布的Api
        public T Get<T>(string key)
        {
            var redisProvider = new RedisProvider(ServerName);
            return CoreAccess.TryCall(EDomain, redisProvider.ReadOnlyClient.Get<T>, key, CoreBase.CallSuccess, redisProvider.Close, true, ServerName);
        }

        public void Set<T>(string key, T value)
        {
            var redisProvider = new RedisProvider(ServerName);
            CoreAccess.TryCallAsync(EDomain, redisProvider.Client.Add, key, value.ToJson(), CoreBase.CallSuccess, redisProvider.Close, null, false, ServerName);
        }

        public void Set<T>(string key, T value, DateTime expire)
        {
            var redisProvider = new RedisProvider(ServerName);
            CoreAccess.TryCallAsync(EDomain, redisProvider.Client.Add, key, value.ToJson(), expire, CoreBase.CallSuccess, redisProvider.Close, null, false, ServerName);
        }

        public void Set<T>(string key, T value, TimeSpan expire)
        {
            var redisProvider = new RedisProvider(ServerName);
            CoreAccess.TryCallAsync(EDomain, redisProvider.Client.Add, key, value.ToJson(), expire, CoreBase.CallSuccess, redisProvider.Close, null, false, ServerName);
        } 
        #endregion
    }
}

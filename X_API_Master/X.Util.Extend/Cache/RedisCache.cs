using System;
using X.Util.Core.Configuration;
using X.Util.Entities.Interface;
using X.Util.Extend.Redis;

namespace X.Util.Extend.Cache
{
    /// <summary>
    /// 缓存基础类
    /// </summary>
    public sealed class RedisCache : IRedisCache
    {
        #region 构造函数
        public readonly string ServerName = ConfigurationHelper.RedisDefaultServername;
        public readonly int DbNum = -1;
        public static readonly IRedisCache Default = new RedisCache();
        private RedisCache() { }
        public RedisCache(string serverName)
        {
            ServerName = serverName;
        }
        public RedisCache(int dbNum)
        {
            DbNum = dbNum;
        } 
        public RedisCache(string serverName, int dbNum)
        {
            ServerName = serverName;
            DbNum = dbNum;
        } 
        #endregion

        public T Get<T>(string key)
        {
            return RedisBase.Default.StringGet<T>(key);
        }

        public void Set<T>(string key, T value, DateTime expire)
        {
            RedisBase.Default.StringSet(key, value, expire - DateTime.Now);
            RedisBase.Default.KeyExpire(key, expire);
        }

        public void Set<T>(string key, T value, TimeSpan expire)
        {
            RedisBase.Default.StringSet(key, value, expire);
            RedisBase.Default.KeyExpire(key, expire);
        }

        public bool KeyDelete(string key)
        {
            return RedisBase.Default.KeyDelete(key);
        }
    }
}

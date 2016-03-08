using System;

namespace X.Util.Entities
{
    /// <summary>
    /// http请求结果
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class HttpCacheResult<T>
    {
        public string CacheKey { get; set; }
        public T Result { get; set; }
        public DateTime LastModified { get; set; }
    }
}

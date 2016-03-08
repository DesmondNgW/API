using System;

namespace X.Util.Entities
{
    [Serializable]
    public class StatusCacheResult<T>
    {
        public string CacheKey { get; set; }
        public T Result { get; set; }
        public DateTime ExpiryDate { get; set; }
    }
}

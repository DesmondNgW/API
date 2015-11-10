using System;

namespace X.Util.Entities
{
    [Serializable]
    public class CacheResultInfo<T>
    {
        public string CacheKey { get; set; }
        public T Result { get; set; }
        public string AppVersion { get; set; }
        public DateTime LastModified { get; set; }
        public DateTime ExpiryDate { get; set; }
    }
}

using System;

namespace X.Util.Entities
{
    /// <summary>
    /// 缓存数据封装
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class CacheResult<T>
    {
        public T Result { get; set; }
        public bool Succeed { get; set; }
        public string Message { get; set; }
        public string ErrorCode { get; set; }
        public string AppVersion { get; set; }
        public DateTime CacheTime { get; set; }
    }
}

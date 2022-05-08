using System;

namespace X.Util.Entities
{
    public class MongoCacheModel : MongoBaseModel
    {
        /// <summary>
        /// 过期时间
        /// </summary>
        public DateTime Expire { get; set; }
    }
}

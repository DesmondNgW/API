using System;

namespace X.Business.Model
{
    public class RequestStatus
    {
        /// <summary>
        /// 请求Uri
        /// </summary>
        public string Uri { get; set; }

        /// <summary>
        /// TokenId
        /// </summary>
        public string TokenId { get; set; }

        /// <summary>
        /// RequesTime
        /// </summary>
        public DateTime RequesTime { get; set; }
    }
}

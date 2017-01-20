using System;

namespace X.Business.Entities
{
    /// <summary>
    /// Token
    /// </summary>
    public class Token
    {
        /// <summary>
        /// clientId
        /// </summary>
        public string ClientId { get; set; }

        /// <summary>
        /// clientIp
        /// </summary>
        public string ClientIp { get; set; }

        /// <summary>
        /// TokenId
        /// </summary>
        public string TokenId { get; set; }

        /// <summary>
        /// LastRequesTime
        /// </summary>
        public DateTime LastRequesTime { get; set; }

        /// <summary>
        /// RequesTime
        /// </summary>
        public DateTime RequesTime { get; set; }

        /// <summary>
        /// AllowAccess
        /// </summary>
        public bool AllowAccess { get; set; }
    }
}

namespace X.Business.Util
{
    /// <summary>
    /// 定义业务常量
    /// </summary>
    public class ConstHelper
    {
        /// <summary>
        /// HmacKey
        /// </summary>
        public const string GenerateHmacKey = "X.Business.Helper.ConstHelper.GenerateHmacKey";

        /// <summary>
        /// LoginKeyPrefix
        /// </summary>
        public const string LoginKeyPrefix = "X.Business.Helper.ConstHelper.LoginKeyPrefix";

        /// <summary>
        /// LoginExpireMinutes
        /// </summary>
        public const int LoginExpireMinutes = 30;

        /// <summary>
        /// SubLoginExpireMinutes
        /// </summary>
        public const int SubLoginExpireMinutes = 10;

        /// <summary>
        /// RequestExpireMinutes
        /// </summary>
        public const int RequestExpireMinutes = 30;

        /// <summary>
        /// RequestInterval
        /// </summary>
        public const int RequestInterval = 100;
    }
}

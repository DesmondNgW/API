using X.Util.Entities;

namespace X.Util.Extend.Core
{
    public abstract class CoreBase
    {
        public const string CoreDefaultMesssage = "系统错误";

        public static bool CallSuccess<TResult>(TResult iresult)
        {
            return !Equals(iresult, default(TResult));
        }

        public static bool CallSuccess<TResult>(CacheResult<TResult> iresult)
        {
            return iresult != null && iresult.Result != null && iresult.Succeed;
        }
    }
}

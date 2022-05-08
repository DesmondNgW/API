using X.Util.Entities;

namespace X.Util.Extend.Core
{
    public abstract class CoreBase
    {
        public const string CoreDataMesssage = "网络不给力！";
        public const string CoreCacheMesssage = "网络不给力！";

        public static bool CallSuccess<TResult>(TResult iresult)
        {
            return !Equals(iresult, default(TResult));
        }

        public static bool CallSuccess<TResult>(CacheResult<TResult> iresult)
        {
            return iresult != null && iresult.Result != null && iresult.Succeed;
        }

        public static bool CallSuccess<TResult>(ResultInfo<TResult> iresult)
        {
            return iresult != null && iresult.Result != null && iresult.Succeed;
        }
    }
}

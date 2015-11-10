using Em.Entities;

namespace X.Util.Extend.Core
{
    public abstract class CoreBase
    {
        public const string CoreDefaultMesssage = "中台服务器错误";

        public static bool CallSuccess<TResult>(TResult iresult)
        {
            return !Equals(iresult, default(TResult));
        }

        public static bool CallSuccess<TResult>(ResultInfo<TResult> iresult)
        {
            return iresult != null && iresult.Result != null && iresult.Succeed;
        }
    }
}

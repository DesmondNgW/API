using X.Interface.Dto;

namespace X.Interface.Core
{
    public class CoreService
    {
        public static bool CallSuccess<TResult>(ApiResult<TResult> iresult)
        {
            return iresult != null && iresult.Data != null && iresult.Success;
        }
    }
}

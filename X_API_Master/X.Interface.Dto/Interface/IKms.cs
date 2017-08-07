using Em.FundTrade.EncryptHelper;
using X.Interface.Dto.HttpResponse;

namespace X.Interface.Dto.Interface
{
    /// <summary>
    /// IKms
    /// </summary>
    public interface IKms
    {
        /// <summary>
        /// MobileEncrypt
        /// </summary>
        /// <param name="mobile"></param>
        /// <returns></returns>
        EncryptResult MobileEncrypt(string mobile);

        /// <summary>
        /// Now
        /// </summary>
        /// <returns></returns>
        ApiResult<DateTimeDto> Now();
    }
}

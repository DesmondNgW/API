using System.Net.Http;

namespace X.Interface.Dto.Interface
{
    /// <summary>
    /// 图片接口
    /// </summary>
    public interface IImageManager
    {
        /// <summary>
        /// 验证码图片
        /// </summary>
        /// <returns></returns>
        HttpResponseMessage VerifyCode();

        /// <summary>
        /// 1*1像素位图
        /// </summary>
        /// <returns></returns>
        HttpResponseMessage BitMap();

        /// <summary>
        ///  文本转图像
        /// </summary>
        /// <param name="value">图片文本</param>
        /// <returns></returns>
        HttpResponseMessage TextImage(string value);

        /// <summary>
        /// 二维码
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        HttpResponseMessage QrCode(string value);
    }
}

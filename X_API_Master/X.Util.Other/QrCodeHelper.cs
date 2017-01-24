using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;
using System.Text;
using ThoughtWorks.QRCode.Codec;
using ThoughtWorks.QRCode.Codec.Data;
using X.Util.Core.Log;
using X.Util.Entities.Enums;

namespace X.Util.Other
{
    public class QrCodeHelper
    {
        /// <summary>
        /// Encoder
        /// </summary>
        /// <param name="qrCodeEncoder"></param>
        /// <param name="content"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static byte[] Encoder(QRCodeEncoder qrCodeEncoder, string content, Encoding encoding)
        {
            try
            {
                var bmp = qrCodeEncoder.Encode(content, encoding);
                byte[] result;
                var ms = new MemoryStream();
                try
                {
                    bmp.Save(ms, ImageFormat.Bmp);
                    result = ms.ToArray();
                }
                finally
                {
                    bmp.Dispose();
                    ms.Close();
                    ms.Dispose();
                }
                return result;
            }
            catch (Exception e)
            {
                Logger.Client.Error(Logger.Client.GetMethodInfo(MethodBase.GetCurrentMethod(), new object[] { content, encoding }), e, LogDomain.Util);
            }
            return null;
        }

        /// <summary>
        /// Decoder
        /// </summary>
        /// <param name="bitmap"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static string Decoder(Bitmap bitmap, Encoding encoding)
        {
            try
            {
                return new QRCodeDecoder().decode(new QRCodeBitmapImage(bitmap));
            }
            catch (Exception e)
            {
                Logger.Client.Error(Logger.Client.GetMethodInfo(MethodBase.GetCurrentMethod(), new object[] { bitmap, encoding }), e, LogDomain.Util);
            }
            return string.Empty;
        }
    }
}

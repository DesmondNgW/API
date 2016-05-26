using System.IO;
using System.Security.Cryptography;
using System.Text;
using X.Util.Core;
using X.Util.Core.Kernel;

namespace X.Util.Extend.Cryption
{
    public sealed class CryptionExtend
    {
        /// <summary>
        /// 加密成16进制字符串
        /// </summary>
        /// <param name="plainStr"></param>
        /// <param name="key"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string Encrypt(string plainStr, string key, CryptionType type)
        {
            var byteArray = plainStr.ToUtf8Bytes();
            var mStream = new MemoryStream();
            var cStream = new CryptoStream(mStream, BaseCryption.CreateEncryptor(key, type), CryptoStreamMode.Write);
            cStream.Write(byteArray, 0, byteArray.Length);
            cStream.FlushFinalBlock();
            var encrypt = mStream.ToArray().Bytes2Hex();
            cStream.Close();
            return encrypt;
        }

        /// <summary>
        /// 加密成base64字符串
        /// </summary>
        /// <param name="plainStr"></param>
        /// <param name="key"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string Encrypt2Base64(string plainStr, string key, CryptionType type)
        {
            var byteArray = plainStr.ToUtf8Bytes();
            var mStream = new MemoryStream();
            var cStream = new CryptoStream(mStream, BaseCryption.CreateEncryptor(key, type), CryptoStreamMode.Write);
            cStream.Write(byteArray, 0, byteArray.Length);
            cStream.FlushFinalBlock();
            var encrypt = mStream.ToArray().Bytes2Base64();
            cStream.Close();
            return encrypt;
        }

        /// <summary>
        /// 从16进制字符串解密
        /// </summary>
        /// <param name="encryptStr"></param>
        /// <param name="key"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string Decrypt(string encryptStr, string key, CryptionType type)
        {
            var byteArray = encryptStr.Hex2Bytes();
            var mStream = new MemoryStream();
            var cStream = new CryptoStream(mStream, BaseCryption.CreateDecryptor(key, type), CryptoStreamMode.Write);
            cStream.Write(byteArray, 0, byteArray.Length);
            cStream.FlushFinalBlock();
            var decrypt = Encoding.UTF8.GetString(mStream.ToArray());
            cStream.Close();
            return decrypt;
        }

        /// <summary>
        /// 从nase64字符串解密
        /// </summary>
        /// <param name="base64"></param>
        /// <param name="key"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string DecryptFromBase64(string base64, string key, CryptionType type)
        {
            var byteArray = base64.Base64ToBytes();
            var mStream = new MemoryStream();
            var cStream = new CryptoStream(mStream, BaseCryption.CreateDecryptor(key, type), CryptoStreamMode.Write);
            cStream.Write(byteArray, 0, byteArray.Length);
            cStream.FlushFinalBlock();
            var decrypt = Encoding.UTF8.GetString(mStream.ToArray());
            cStream.Close();
            return decrypt;
        }
    }
}

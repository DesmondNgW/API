using System.IO;
using System.Security.Cryptography;
using System.Text;
using X.Util.Core;

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
            var byteArray = Encoding.UTF8.GetBytes(plainStr);
            var mStream = new MemoryStream();
            var cStream = new CryptoStream(mStream, BaseCryption.CreateEncryptor(key, type), CryptoStreamMode.Write);
            cStream.Write(byteArray, 0, byteArray.Length);
            cStream.FlushFinalBlock();
            var encrypt = StringConvert.Bytes2Hex(mStream.ToArray());
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
            var byteArray = Encoding.UTF8.GetBytes(plainStr);
            var mStream = new MemoryStream();
            var cStream = new CryptoStream(mStream, BaseCryption.CreateEncryptor(key, type), CryptoStreamMode.Write);
            cStream.Write(byteArray, 0, byteArray.Length);
            cStream.FlushFinalBlock();
            var encrypt = StringConvert.Bytes2Base64(mStream.ToArray());
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
            var byteArray = StringConvert.Hex2Bytes(encryptStr);
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
            var byteArray = StringConvert.Base64ToBytes(base64);
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

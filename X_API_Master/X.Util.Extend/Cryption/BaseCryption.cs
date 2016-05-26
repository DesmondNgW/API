using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using X.Util.Core;
using X.Util.Core.Kernel;

namespace X.Util.Extend.Cryption
{
    public enum CryptionType { Des, Aes, TripleDes }

    public enum HmacType { Md5, Ripemd160, Sha1, Sha256, Sha384, Sha512 }

    public sealed class BaseCryption
    {
        /// <summary>
        /// 16位MD5
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string Md5Bit16(string s)
        {
            var md5 = new MD5CryptoServiceProvider();
            var t2 = BitConverter.ToString(md5.ComputeHash(s.ToDefaultBytes()), 4, 8);
            t2 = t2.Replace("-", "").ToLower();
            return t2;
        }

        /// <summary>
        /// 32位MD5
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string Md5Bit32(string s)
        {
            var md5 = new MD5CryptoServiceProvider();
            var encryptedBytes = md5.ComputeHash(s.ToAsciiBytes());
            var sb = new StringBuilder();
            foreach (var t in encryptedBytes) sb.AppendFormat("{0:x2}", t);
            return sb.ToString().ToLower();
        }

        /// <summary>
        /// sha-1
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string Sha1(string s)
        {
            SHA1 sha1 = new SHA1CryptoServiceProvider();
            var encryptedBytes = sha1.ComputeHash(s.ToAsciiBytes());
            var sb = new StringBuilder();
            foreach (var t in encryptedBytes) sb.AppendFormat("{0:x2}", t);
            return sb.ToString().ToLower();
        }

        /// <summary>
        /// sha-256
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string Sha256(string s)
        {
            SHA256 sha1 = new SHA256CryptoServiceProvider();
            var encryptedBytes = sha1.ComputeHash(s.ToAsciiBytes());
            var sb = new StringBuilder();
            foreach (var t in encryptedBytes) sb.AppendFormat("{0:x2}", t);
            return sb.ToString().ToLower();
        }

        /// <summary>
        /// sha-384
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string Sha384(string s)
        {
            SHA384 sha1 = new SHA384CryptoServiceProvider();
            var encryptedBytes = sha1.ComputeHash(s.ToAsciiBytes());
            var sb = new StringBuilder();
            foreach (var t in encryptedBytes) sb.AppendFormat("{0:x2}", t);
            return sb.ToString().ToLower();
        }

        /// <summary>
        /// sha-512
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string Sha512(string s)
        {
            SHA512 sha1 = new SHA512CryptoServiceProvider();
            var encryptedBytes = sha1.ComputeHash(s.ToAsciiBytes());
            var sb = new StringBuilder();
            foreach (var t in encryptedBytes) sb.AppendFormat("{0:x2}", t);
            return sb.ToString().ToLower();
        }

        private static HMAC GetHmac(string key, HmacType hmacType)
        {
            var bytes = key.ToDefaultBytes();
            switch (hmacType)
            {
                case HmacType.Ripemd160:
                    return new HMACRIPEMD160(bytes);
                case HmacType.Sha1:
                    return new HMACSHA1(bytes);
                case HmacType.Sha256:
                    return new HMACSHA256(bytes);
                case HmacType.Sha384:
                    return new HMACSHA384(bytes);
                case HmacType.Sha512:
                    return new HMACSHA512(bytes);
                default:
                    return new HMACMD5(bytes);
            }
        }

        /// <summary>
        /// HMAC SignData
        /// </summary>
        /// <param name="key"></param>
        /// <param name="content"></param>
        /// <param name="hmacType"></param>
        /// <returns></returns>
        public static string SignData(string key, string content, HmacType hmacType)
        {
            var data = content.ToDefaultBytes();
            var alg = GetHmac(key, hmacType);
            return alg.ComputeHash(data).Concat(data).ToArray().Bytes2Base64();
        }

        /// <summary>
        /// HMAC VerityData
        /// </summary>
        /// <param name="key"></param>
        /// <param name="content"></param>
        /// <param name="hmacType"></param>
        /// <returns></returns>
        public static bool VerifyData(string key, string content, HmacType hmacType)
        {
            var data = content.Base64ToBytes();
            var alg = GetHmac(key, hmacType);
            return data.Take(alg.HashSize >> 3).SequenceEqual(alg.ComputeHash(data.Skip(alg.HashSize >> 3).ToArray()));
        }

        public static ICryptoTransform CreateEncryptor(string key, CryptionType type)
        {
            ICryptoTransform transform;
            SHA512 sha512 = new SHA512CryptoServiceProvider();
            var bytes = sha512.ComputeHash(Sha1(key).ToAsciiBytes());
            switch (type)
            {
                case CryptionType.Aes:
                    var aes = Rijndael.Create();
                    aes.Mode = CipherMode.CBC;
                    transform = aes.CreateEncryptor(bytes.Skip(17).Take(32).ToArray(), bytes.Skip(17).Take(16).ToArray());
                    aes.Clear();
                    break;
                case CryptionType.Des:
                    var des = new DESCryptoServiceProvider {Mode = CipherMode.CBC};
                    transform = des.CreateEncryptor(bytes.Skip(17).Take(8).ToArray(), bytes.Skip(17).Take(16).ToArray());
                    des.Clear();
                    break;
                default:
                    var tripleDes = new TripleDESCryptoServiceProvider {Mode = CipherMode.CBC};
                    transform = tripleDes.CreateEncryptor(bytes.Skip(17).Take(24).ToArray(), bytes.Skip(17).Take(16).ToArray());
                    tripleDes.Clear();
                    break;
            }
            return transform;
        }

        public static ICryptoTransform CreateDecryptor(string key, CryptionType type)
        {
            ICryptoTransform transform;
            SHA512 sha512 = new SHA512CryptoServiceProvider();
            var bytes = sha512.ComputeHash(Sha1(key).ToAsciiBytes());
            switch (type)
            {
                case CryptionType.Aes:
                    var aes = Rijndael.Create();
                    aes.Mode = CipherMode.CBC;
                    transform = aes.CreateDecryptor(bytes.Skip(17).Take(32).ToArray(), bytes.Skip(17).Take(16).ToArray());
                    aes.Clear();
                    break;
                case CryptionType.Des:
                    var des = new DESCryptoServiceProvider {Mode = CipherMode.CBC};
                    transform = des.CreateDecryptor(bytes.Skip(17).Take(8).ToArray(), bytes.Skip(17).Take(16).ToArray());
                    des.Clear();
                    break;
                default:
                    var tripleDes = new TripleDESCryptoServiceProvider {Mode = CipherMode.CBC};
                    transform = tripleDes.CreateDecryptor(bytes.Skip(17).Take(24).ToArray(), bytes.Skip(17).Take(16).ToArray());
                    tripleDes.Clear();
                    break;
            }
            return transform;
        }
    }
}

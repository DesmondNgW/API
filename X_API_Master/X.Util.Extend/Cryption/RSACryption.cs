using MongoDB.Driver;
using System;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using X.Util.Core;
using X.Util.Entities;
using X.Util.Extend.Mongo;

namespace X.Util.Extend.Cryption
{
    public sealed class RsaCryption
    {
        private static RsaKey GetRsaKey(string id)
        {
            var rsaKey = default(RsaKey);
            var query = new QueryDocument {{"_id", id }};
            var list = MongoDbBase.ToEntity<RsaKey>(MongoDbBase.Default.ReadMongo("RSA", "RSA", query));
            if (list != null && list.Count > 0)
            {
                rsaKey = list[0];
            }
            return rsaKey;
        }

        private static void RemoveRsaKey(string id)
        {
            var query = new QueryDocument {{"_id", id}};
            MongoDbBase.Default.RemoveMongo("RSA", "RSA", query, RemoveFlags.Single);
        }

        private static RSAParameters GetPublicKey(string id, bool tmpKey, Func<RSAParameters> loader)
        {
            var rsaKey = GetRsaKey(id);
            if (!Equals(rsaKey, default(RsaKey)))
                return new RSAParameters
                {
                    Modulus = StringConvert.Hex2Bytes(rsaKey.Modulus),
                    Exponent = StringConvert.Hex2Bytes(rsaKey.Exponent)
                };
            var privateKey = loader();
            rsaKey = new RsaKey
            {
                id = id,
                TmpKey = tmpKey,
                Modulus = StringConvert.Bytes2Hex(privateKey.Modulus),
                Exponent = StringConvert.Bytes2Hex(privateKey.Exponent),
                D = StringConvert.Bytes2Hex(privateKey.D),
                Dp = StringConvert.Bytes2Hex(privateKey.DP),
                Dq = StringConvert.Bytes2Hex(privateKey.DQ),
                InverseQ = StringConvert.Bytes2Hex(privateKey.InverseQ),
                P = StringConvert.Bytes2Hex(privateKey.P),
                Q = StringConvert.Bytes2Hex(privateKey.Q)
            };
            MongoDbBase.Default.AddMongo(rsaKey, "RSA", "RSA");
            return new RSAParameters
            {
                Modulus = StringConvert.Hex2Bytes(rsaKey.Modulus),
                Exponent = StringConvert.Hex2Bytes(rsaKey.Exponent)
            };
        }

        public static RSAParameters GetPublicKey(string key, int size, string nonce, bool tmpKey)
        {
            return GetPublicKey($"{key}_{size}_{nonce}", tmpKey, () => new RSACryptoServiceProvider(size).ExportParameters(true));
        }

        private static RSAParameters GetPrivateKey(RsaKey rsaKey)
        {
            return new RSAParameters
            {
                Modulus = StringConvert.Hex2Bytes(rsaKey.Modulus),
                Exponent = StringConvert.Hex2Bytes(rsaKey.Exponent),
                D = StringConvert.Hex2Bytes(rsaKey.D),
                DP = StringConvert.Hex2Bytes(rsaKey.Dp),
                DQ = StringConvert.Hex2Bytes(rsaKey.Dq),
                InverseQ = StringConvert.Hex2Bytes(rsaKey.InverseQ),
                P = StringConvert.Hex2Bytes(rsaKey.P),
                Q = StringConvert.Hex2Bytes(rsaKey.Q)
            };
        }

        /// <summary>
        /// 加密成16进制数
        /// </summary>
        /// <param name="key"></param>
        /// <param name="content"></param>
        /// <param name="nonce"></param>
        /// <param name="tmpKey"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public static string Encrypt(string key, string content, string nonce, bool tmpKey = true, int size = 1024)
        {
            var rsa = new RSACryptoServiceProvider(size);
            var publicKey = GetPublicKey(key, size, nonce, tmpKey);
            rsa.ImportParameters(publicKey);
            var cipherbytes = rsa.Encrypt(Encoding.UTF8.GetBytes(content), false);
            return StringConvert.Bytes2Hex(cipherbytes);
        }

        /// <summary>
        /// 加密成base64字符串
        /// </summary>
        /// <param name="key"></param>
        /// <param name="content"></param>
        /// <param name="nonce"></param>
        /// <param name="tmpKey"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public static string Encrypt2Base64(string key, string content, string nonce, bool tmpKey = true, int size = 1024)
        {
            var rsa = new RSACryptoServiceProvider(size);
            var publicKey = GetPublicKey(key, size, nonce, tmpKey);
            rsa.ImportParameters(publicKey);
            var cipherbytes = rsa.Encrypt(Encoding.UTF8.GetBytes(content), false);
            return StringConvert.Bytes2Base64(cipherbytes);
        }

        /// <summary>
        /// 从16进制数解密
        /// </summary>
        /// <param name="key"></param>
        /// <param name="content"></param>
        /// <param name="nonce"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public static string Decrypt(string key, string content, string nonce, int size = 1024)
        {
            var rsa = new RSACryptoServiceProvider(size);
            var rsaKey = GetRsaKey($"{key}_{size}_{nonce}");
            if (!Equals(rsaKey, default(RsaKey)))
            {
                var privateKey = GetPrivateKey(rsaKey);
                rsa.ImportParameters(privateKey);
                var cipherbytes = rsa.Decrypt(StringConvert.Hex2Bytes(content), false);
                if (rsaKey.TmpKey) RemoveRsaKey(rsaKey.id);
                return Encoding.UTF8.GetString(cipherbytes);
            }
            Logger.Error(MethodBase.GetCurrentMethod(), LogDomain.Util, null, "RSA Private Key Missing.");
            return string.Empty;
        }

        /// <summary>
        /// 从base64字符串解密
        /// </summary>
        /// <param name="key"></param>
        /// <param name="content"></param>
        /// <param name="nonce"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public static string DecryptFromBase64(string key, string content, string nonce, int size = 1024)
        {
            var rsa = new RSACryptoServiceProvider(size);
            var rsaKey = GetRsaKey($"{key}_{size}_{nonce}");
            if (!Equals(rsaKey, default(RsaKey)))
            {
                var privateKey = GetPrivateKey(rsaKey);
                rsa.ImportParameters(privateKey);
                var cipherbytes = rsa.Decrypt(StringConvert.Base64ToBytes(content), false);
                if (rsaKey.TmpKey) RemoveRsaKey(rsaKey.id);
                return Encoding.UTF8.GetString(cipherbytes);
            }
            Logger.Error(MethodBase.GetCurrentMethod(), LogDomain.Util, null, "RSA Private Key Missing.");
            return string.Empty;
        }
    }
}

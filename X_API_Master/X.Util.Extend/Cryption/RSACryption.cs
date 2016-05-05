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
            var query = new QueryDocument { { "_id", id } };
            var list = MongoDbBase.ToEntity<RsaKey>(MongoDbBase.Default.ReadMongo("RSA", "RSA", query));
            if (list != null && list.Count > 0)
            {
                rsaKey = list[0];
            }
            return rsaKey;
        }

        private static void RemoveRsaKey(string id)
        {
            var query = new QueryDocument { { "_id", id } };
            MongoDbBase.Default.RemoveMongo("RSA", "RSA", query, RemoveFlags.Single);
        }

        private static RSAParameters GetPublicKey(string id, bool tmpKey, Func<RSAParameters> loader)
        {
            var rsaKey = GetRsaKey(id);
            if (!Equals(rsaKey, default(RsaKey)))
                return new RSAParameters
                {
                    Modulus = rsaKey.Modulus.Hex2Bytes(),
                    Exponent = rsaKey.Exponent.Hex2Bytes()
                };
            var privateKey = loader();
            rsaKey = new RsaKey
            {
                Id = id,
                TmpKey = tmpKey,
                Modulus = privateKey.Modulus.Bytes2Hex(),
                Exponent = privateKey.Exponent.Bytes2Hex(),
                D = privateKey.D.Bytes2Hex(),
                Dp = privateKey.DP.Bytes2Hex(),
                Dq = privateKey.DQ.Bytes2Hex(),
                InverseQ = privateKey.InverseQ.Bytes2Hex(),
                P = privateKey.P.Bytes2Hex(),
                Q = privateKey.Q.Bytes2Hex()
            };
            MongoDbBase.Default.SaveMongo(rsaKey, "RSA", "RSA");
            return new RSAParameters
            {
                Modulus = rsaKey.Modulus.Hex2Bytes(),
                Exponent = rsaKey.Exponent.Hex2Bytes()
            };
        }

        public static RSAParameters GetPublicKey(string key, int size, string nonce, bool tmpKey)
        {
            return GetPublicKey(string.Format("{0}_{1}_{2}", key, size, nonce), tmpKey, () => new RSACryptoServiceProvider(size).ExportParameters(true));
        }

        private static RSAParameters GetPrivateKey(RsaKey rsaKey)
        {
            return new RSAParameters
            {
                Modulus = rsaKey.Modulus.Hex2Bytes(),
                Exponent = rsaKey.Exponent.Hex2Bytes(),
                D = rsaKey.D.Hex2Bytes(),
                DP = rsaKey.Dp.Hex2Bytes(),
                DQ = rsaKey.Dq.Hex2Bytes(),
                InverseQ = rsaKey.InverseQ.Hex2Bytes(),
                P = rsaKey.P.Hex2Bytes(),
                Q = rsaKey.Q.Hex2Bytes()
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
            var cipherbytes = rsa.Encrypt(content.ToUtf8Bytes(), false);
            return cipherbytes.Bytes2Hex();
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
            var cipherbytes = rsa.Encrypt(content.ToUtf8Bytes(), false);
            return cipherbytes.Bytes2Base64();
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
            var rsaKey = GetRsaKey(string.Format("{0}_{1}_{2}", key, size, nonce));
            if (!Equals(rsaKey, default(RsaKey)))
            {
                var privateKey = GetPrivateKey(rsaKey);
                rsa.ImportParameters(privateKey);
                var cipherbytes = rsa.Decrypt(content.Hex2Bytes(), false);
                if (rsaKey.TmpKey) RemoveRsaKey(rsaKey.Id);
                return Encoding.UTF8.GetString(cipherbytes);
            }
            Logger.Client.Error(MethodBase.GetCurrentMethod(), LogDomain.Util, null, string.Empty, "RSA Private Key Missing.");
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
            var rsaKey = GetRsaKey(string.Format("{0}_{1}_{2}", key, size, nonce));
            if (!Equals(rsaKey, default(RsaKey)))
            {
                var privateKey = GetPrivateKey(rsaKey);
                rsa.ImportParameters(privateKey);
                var cipherbytes = rsa.Decrypt(content.Base64ToBytes(), false);
                if (rsaKey.TmpKey) RemoveRsaKey(rsaKey.Id);
                return Encoding.UTF8.GetString(cipherbytes);
            }
            Logger.Client.Error(MethodBase.GetCurrentMethod(), LogDomain.Util, null, string.Empty, "RSA Private Key Missing.");
            return string.Empty;
        }
    }
}

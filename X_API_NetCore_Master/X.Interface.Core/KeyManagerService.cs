using System;
using System.Globalization;
using X.Interface.Dto;
using X.Interface.Dto.HttpResponse;
using X.Interface.Dto.Interface;
using X.Util.Core;
using X.Util.Extend.Cryption;

namespace X.Interface.Core
{
    public class KeyManagerService : IKeyManager
    {
        /// <summary>
        /// GetPublicKey
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        public ApiResult<PublicKeyDto> GetPublicKey(int size)
        {
            size = Math.Max(1024, size);
            var key = Guid.NewGuid().ToString("N");
            var nonce = (DateTime.Now - new DateTime(1970, 1, 1).ToLocalTime()).TotalMilliseconds.ToString(CultureInfo.InvariantCulture);
            var publicKey = RsaCryption.GetPublicKey(key, size, nonce, true);
            var dto = new PublicKeyDto
            {
                Exponent = publicKey.Exponent.Bytes2Hex(),
                Modulus = publicKey.Modulus.Bytes2Hex(),
                RsaKey = key,
                Size = size,
                Nonce = nonce
            };
            return new ApiResult<PublicKeyDto> { Success = true, Data = dto };
        }

        public ApiResult<string> Encrypt(string key, string content, string nonce, int size = 1024)
        {
            var ret = RsaCryption.Encrypt(key, content, nonce, true, size);
            return new ApiResult<string> { Success = true, Data = ret };
        }

        public ApiResult<string> Decrypt(string key, string content, string nonce, int size = 1024)
        {
            var ret = RsaCryption.Decrypt(key, content, nonce, size);
            return new ApiResult<string> { Success = true, Data = ret };
        }
    }
}

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
        public ApiResult<PublicKeyDto> GetPublicKey(int size)
        {
            size = Math.Max(1024, size);
            var key = Guid.NewGuid().ToString("N");
            var nonce = (DateTime.Now - new DateTime(1970, 1, 1)).TotalMilliseconds.ToString(CultureInfo.InvariantCulture);
            var publicKey = RsaCryption.GetPublicKey(key, size, nonce, true);
            var dto = new PublicKeyDto
            {
                Exponent = StringConvert.Bytes2Hex(publicKey.Exponent),
                Modulus = StringConvert.Bytes2Hex(publicKey.Modulus),
                RsaKey = key,
                Size = size,
                Nonce = nonce
            };
            return new ApiResult<PublicKeyDto> {Success = true, Data = dto};
        }

        public ApiResult<string> GetToken()
        {
            return new ApiResult<string> {Success = true, Data = ServiceHelper.GenerateToken() };
        }
    }
}

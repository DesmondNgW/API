"use strict";
import MD5 from "crypto-js/md5";
import SHA1 from "crypto-js/sha1";
import SHA256 from "crypto-js/sha256";
import SHA384 from "crypto-js/sha384";
import SHA512 from "crypto-js/sha512";

import HmacMD5 from "crypto-js/hmac-md5";
import HmacSHA1 from "crypto-js/hmac-sha1";
import HmacSHA256 from "crypto-js/hmac-sha256";
import HmacSHA384 from "crypto-js/hmac-sha384";
import HmacSHA512 from "crypto-js/hmac-sha512";

function BaseCrypto(name) {
    this.name = name;
}

BaseCrypto.prototype.MD5 = MD5;
BaseCrypto.prototype.SHA1 = SHA1;
BaseCrypto.prototype.SHA256 = SHA256;
BaseCrypto.prototype.SHA384 = SHA384;
BaseCrypto.prototype.SHA512 = SHA512;

BaseCrypto.prototype.HmacMD5 = HmacMD5;
BaseCrypto.prototype.HmacSHA1 = HmacSHA1;
BaseCrypto.prototype.HmacSHA256 = HmacSHA256;
BaseCrypto.prototype.HmacSHA384 = HmacSHA384;
BaseCrypto.prototype.HmacSHA512 = HmacSHA512;

export const baseCrypto = new BaseCrypto("util.extend.baseCrypto")
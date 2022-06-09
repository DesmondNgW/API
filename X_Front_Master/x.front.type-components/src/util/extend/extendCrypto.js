'use strict';
import AES from "crypto-js/aes";
import TripleDES from "crypto-js/tripledes";

function ExtendCrypto(name) {
    this.name = name;
}

ExtendCrypto.prototype.AESEncrypt = AES.encrypt;
ExtendCrypto.prototype.AESDecrypt = AES.decrypt;
ExtendCrypto.prototype.TripleDESEncrypt = TripleDES.encrypt;
ExtendCrypto.prototype.TripleDESDecrypt = TripleDES.decrypt;

export const extendCrypto = new ExtendCrypto("util.extend.extendCrypto")


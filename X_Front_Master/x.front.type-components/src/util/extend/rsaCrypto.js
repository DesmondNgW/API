'use strict';
import { JSEncrypt } from "jsencrypt";

function RSACrypto(name) {
    this.name = name;
}

/**
 * encrypt
 * @param {any} publicKey
 * @param {any} text
 */
RSACrypto.prototype.encrypt = function (publicKey, text) {
    let obj = new JSEncrypt();
    obj.setPublicKey(publicKey);
    return obj.encrypt(text);
}

/**
 * decrypt
 * @param {any} privateKey
 * @param {any} encrypted
 */
RSACrypto.prototype.decrypt = function (privateKey, encrypted) {
    let obj = new JSEncrypt();
    obj.setPrivateKey(privateKey);
    return obj.decrypt(encrypted);
}

export const RSACrypto = new RSACrypto("util.extend.RSACrypto")


define(function (require) {
    var rsa = require("./RSA");
    return function (exponent, modulus, size, content) {
        rsa.setMaxDigits(parseInt(size / 1024 * 130));
        var key = new rsa.RSAKeyPair(exponent, "", modulus);
        return rsa.encrypt(key, content);
    };
});
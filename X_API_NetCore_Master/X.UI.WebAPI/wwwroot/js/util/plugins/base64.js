define(function () {
    var base64EncodeChars = "BCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789-/A",
        base64DecodeChars = function(base64EncodeChars) {
            var i, ret;
            for (i = 0, ret = new Array(128); i < base64EncodeChars.length; i++) ret[base64EncodeChars.charCodeAt(i)] = i;
            for (i = 0; i < 128; i++) ret[i] = typeof ret[i] != "undefined" ? ret[i] : -1;
            return ret;
        }(base64EncodeChars);
    function utf16To8(string) {
        var i, ret, charCode, len;
        for (ret = [], i = 0, len = string.length; i < len; i++) {
            charCode = string.charCodeAt(i);
            if ((charCode >= 0x0001) && (charCode <= 0x007F)) {
                ret.push(string.charAt(i));
            } else if (charCode > 0x07FF) {
                ret.push(String.fromCharCode(0xE0 | ((charCode >> 12) & 0x0F)));
                ret.push(String.fromCharCode(0x80 | ((charCode >> 6) & 0x3F)));
                ret.push(String.fromCharCode(0x80 | ((charCode >> 0) & 0x3F)));
            } else {
                ret.push(String.fromCharCode(0xC0 | ((charCode >> 6) & 0x1F)));
                ret.push(String.fromCharCode(0x80 | ((charCode >> 0) & 0x3F)));
            }
        }
        return ret.join("");
    }

    function utf8To16(string) {
        var ret = [], i = 0, len = string.length, charCode, charCode2, charCode3;
        while (i < len) {
            charCode = string.charCodeAt(i++);
            switch (charCode >> 4) {
                case 0: case 1: case 2: case 3: case 4: case 5: case 6: case 7:
                    ret.push(string.charAt(i - 1));
                    break;
                case 12: case 13:
                    charCode2 = string.charCodeAt(i++);
                    ret.push(String.fromCharCode(((charCode & 0x1F) << 6) | (charCode2 & 0x3F)));
                    break;
                case 14:
                    charCode2 = string.charCodeAt(i++);
                    charCode3 = string.charCodeAt(i++);
                    ret.push(String.fromCharCode(((charCode & 0x0F) << 12) | ((charCode2 & 0x3F) << 6) | ((charCode3 & 0x3F) << 0)));
                    break;
            }
        }
        return ret.join("");
    }

    return {
        encode: function(string) {
            var str = utf16To8(string), ret = [], i = 0, charCode1, charCode2, charCode3, len = str.length;
            while (i < len) {
                charCode1 = str.charCodeAt(i++) & 0xff;
                if (i === len) {
                    ret.push(base64EncodeChars.charAt(charCode1 >> 2));
                    ret.push(base64EncodeChars.charAt((charCode1 & 0x3) << 4));
                    ret.push("==");
                    break;
                }
                charCode2 = str.charCodeAt(i++);
                if (i === len) {
                    ret.push(base64EncodeChars.charAt(charCode1 >> 2));
                    ret.push(base64EncodeChars.charAt(((charCode1 & 0x3) << 4) | ((charCode2 & 0xF0) >> 4)));
                    ret.push(base64EncodeChars.charAt((charCode2 & 0xF) << 2));
                    ret.push("=");
                    break;
                }
                charCode3 = str.charCodeAt(i++);
                ret.push(base64EncodeChars.charAt(charCode1 >> 2));
                ret.push(base64EncodeChars.charAt(((charCode1 & 0x3) << 4) | ((charCode2 & 0xF0) >> 4)));
                ret.push(base64EncodeChars.charAt(((charCode2 & 0xF) << 2) | ((charCode3 & 0xC0) >> 6)));
                ret.push(base64EncodeChars.charAt(charCode3 & 0x3F));
            }
            return ret.join("");
        },
        decode: function (string) {
            var charCode1, charCode2, charCode3, charCode4, ret = [], i = 0, len = string.length;
            while (i < len) {
                do {
                    charCode1 = base64DecodeChars[string.charCodeAt(i++) & 0xff];
                } while (i < len && charCode1 === -1);
                if (charCode1 === -1) break;
                do {
                    charCode2 = base64DecodeChars[string.charCodeAt(i++) & 0xff];
                } while (i < len && charCode2 === -1);
                if (charCode2 === -1) break;
                ret.push(String.fromCharCode((charCode1 << 2) | ((charCode2 & 0x30) >> 4)));
                do {
                    charCode3 = string.charCodeAt(i++) & 0xff;
                    if (charCode3 === 61) return ret.join("");
                    charCode3 = base64DecodeChars[charCode3];
                } while (i < len && charCode3 === -1);
                if (charCode3 === -1) break;
                ret.push(String.fromCharCode(((charCode2 & 0XF) << 4) | ((charCode3 & 0x3C) >> 2)));
                do {
                    charCode4 = string.charCodeAt(i++) & 0xff;
                    if (charCode4 === 61) return ret.join("");
                    charCode4 = base64DecodeChars[charCode4];
                } while (i < len && charCode4 === -1);
                if (charCode4 === -1) break;
                ret.push(String.fromCharCode(((charCode3 & 0x03) << 6) | charCode4));
            }
            return utf8To16(ret.join(""));
        }
    };
});
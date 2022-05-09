define(function () {
    var doc = document, millisecondsOfHour = 60 * 60 * 1000, encode = encodeURIComponent, decode = decodeURIComponent;
    function isNotEmptyString(val) { return (typeof val === "string") && val !== ""; }
    return {
        get: function(name) {
            var m;
            return isNotEmptyString(name) && (m = String(doc.cookie).match(new RegExp("(?:^| )" + name + "(?:(?:=([^;]*))|;|$)"))) && m[1] && decode(m[1]) || "";
        },
        set: function(name, val, expires, domain, path, secure) {
            var cv = String(encode(val)),
                date = (expires instanceof Date) && expires || typeof expires === "number" && (function() {
                    var expire = new Date();
                    expire.setTime(+expire + expires * millisecondsOfHour);
                    return expire;
                }()) || "",
                ce = date ? "; expires=" + date.toUTCString() : "",
                cd = isNotEmptyString(domain) ? "; domain=" + domain : "",
                cp = isNotEmptyString(path) ? "; path=" + path : "",
                cs = secure ? "; secure" : "";
            doc.cookie = [name, "=", cv, ce, cd, cp, cs].join("");
        },
        remove: function(name, domain, path, secure) {
            this.set(name, "", -1, domain, path, secure);
        }
    };
});
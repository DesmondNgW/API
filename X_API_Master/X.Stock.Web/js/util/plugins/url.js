define(function () {
    var isType = function() {
            return function(obj) { return {}.toString.call(obj) === "[object " + type + "]"; };
        },
        isString = isType("String"),
        isObject = isType("Object"),
        parseReg = /([^&]+?)=(.+?)(?:&|$)/g,
        hrefReg = /([A-Za-z:]+)(\/\/)([A-Za-z0-9\.:]+)([^\?#]+)(\?[^#]+)?(#.+)?/,
        hashReg = /^#([^\?#]+)(\?[^#]+)?/,
        dotRe = /\/\.\//g,
        doubleDotRe = /\/[^/]+\/\.\.\//,
        multiSlashRe = /([^:/])\/+\//g,
        extend = function(a, b) {
            a = a || {};
            for (var n in b) if (b.hasOwnProperty(n)) a[n] = b[n];
            return a;
        },
        parse = function(search) {
            var obj = {}, arr, input = search && search.substring(1);
            while ((arr = parseReg.exec(input))) obj[arr[1]] = unescape(arr[2]);
            return obj;
        },
        realpath = function(path) {
            path = path.replace(dotRe, "/").replace(multiSlashRe, "$1/");
            while (path.match(doubleDotRe)) path = path.replace(doubleDotRe, "/");
            return path;
        };
    function url(href) {
        if (!(this instanceof url)) return new url(href);
        var arr = [location.protocol, "//", location.host, location.pathname, location.search, location.hash], hashArr = ["", ""];
        href && (arr = href.match(hrefReg)) && (arr = [arr[1], arr[2], arr[3], arr[4] || "/", arr[5] || "", arr[6] || ""]);
        arr[5] && (hashArr = arr[5].match(hashReg)) && (hashArr = [hashArr[1], hashArr[2] || ""]);
        arr[4] = realpath(arr[4]);
        return extend(this, {
            path: arr,
            params: parse(arr[4]),
            hashPath: realpath(hashArr[0]),
            hashQuery: parse(hashArr[1])
        }), this;
    }

    extend(url.prototype, {
        toString: function() { return this.path.join(""); },
        search: function (prop, value) {
            var obj = this.params, arr = [];
            if (isString(prop)) {
                if (!value) return obj[prop];
                obj[prop] = value;
            } else if (isObject(prop)) {
                for (var key in prop) if (prop.hasOwnProperty(key)) obj[key] = prop[key];
            }
            for (var k in obj) if (obj.hasOwnProperty(k)) arr.push(k + "=" + obj[k]);
            return arr.length && (this.path[3] = "?" + arr.join("&")), this;
        },
        hash: function (prop, value) {
            var obj = this.hashQuery, arr = [];
            if (isString(prop)) {
                if (!value) return obj[prop];
                obj[prop] = value;
            } else if (isObject(prop)) {
                for (var key in prop) if (prop.hasOwnProperty(key)) obj[key] = prop[key];
            }
            for (var k in obj) if (obj.hasOwnProperty(k)) arr.push(k + "=" + obj[k]);
            return this.path[5] = [this.hashPath, arr.length ? "?" + arr.join("&") : ""].join(""), this;
        }
    });
    return url;
});
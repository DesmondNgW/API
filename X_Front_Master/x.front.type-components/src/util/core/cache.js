'use strict';
function Cache(name) {
    this.name = name;
    this.CacheStore = {};
    this.prefixKey = "util.core.cache.prefixKey";
}

Cache.prototype.get = function (key) {
    var symbol = Symbol.for(this.prefixKey + key);
    return this.CacheStore[symbol];
}

Cache.prototype.set = function (key, value) {
    var symbol = Symbol.for(this.prefixKey + key);
    this.CacheStore[symbol] = value;
}

Cache.prototype.remove = function (key) {
    var symbol = Symbol.for(this.prefixKey + key);
    this.CacheStore[symbol] = null;
    delete this.CacheStore[symbol];
}

export const cache = new Cache("util.core.cache");
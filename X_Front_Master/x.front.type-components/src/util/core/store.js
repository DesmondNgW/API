'use strict';
function Store(name) {
    this.name = name;
    this.prefixKey = "util.core.cache.store";
}

Store.prototype.getStore = function (type) {
    return type == "local" ? localStorage : sessionStorage;
}

Store.prototype.get = function (key, type) {
    let storeObj = this.getStore(type);
    return storeObj.getItem[this.prefixKey + key];
}

Store.prototype.set = function (key, value, type) {
    let storeObj = this.getStore(type);
    storeObj.setItem(this.prefixKey + key, value);
}

Store.prototype.remove = function (key, type) {
    let storeObj = this.getStore(type);
    storeObj.removeItem(this.prefixKey + key);
}

export const store = new Store("util.core.store");
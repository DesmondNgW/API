'use strict';
import { cache } from "../core/cache.js";

function CacheData(name) {
    this.name = name;
}

/**
 * getCacheData
 * @param {any} key:cache-key
 * @param {any} version:cache-version
 * @param {any} dt:cache-expire{Type:long|Date}
 * @param {any} loader:loaderdata{Type:function;ReturnType:Object{Result=;Succeed=;}}
 * @param {any} loadArgs
 * @param {any} bindThis
 */
CacheData.prototype.getCacheData = function (key, version, dt, loader, loadArgs, bindThis) {
    let setting = cache.get(key);
    if (setting && setting.Result && setting.Succeed && setting.AppVersion == version && (+new Date() - setting.CacheTime) < 0) return setting;
    let iresult = loader.apply(bindThis, loadArgs);
    setting = iresult;
    if (!setting) return null;
    setting.AppVersion = version;
    setting.CacheTime = +dt;
    setting.Succeed = iresult && iresult.Result && iresult.Succeed;
    if (setting.Succeed) cache.set(key, setting);
    return setting;
}

/**
 * remove
 * @param {any} key
 */
CacheData.prototype.remove = function (key) {
    cache.remove(key);
}


export const cacheData = new CacheData("util.extend.cacheData");
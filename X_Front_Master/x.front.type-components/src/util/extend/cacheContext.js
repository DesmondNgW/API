'use strict';
import { cacheData } from "./cacheData.js";

/**
 * CacheContext
 * @param {any} Provider
 * @param {any} Options:{Type:Object{Key=;Version=;CallSuccess=function{returnType bool;};Date=;}}
 */
function CacheContext(Provider, Options) {
    this.Provider = Provider;
    this.Options = Options;
}

/**
 * Calling
 * @param {any} context
 * @param {any} caller
 */
CacheContext.prototype.Calling = function (context, caller) {
    let self = this;
    let options = this.Options;
    let CallSuccess = options.CallSuccess;
    let box = function (caller) {
        return () => {
            let iresult = caller();
            return {
                Succeed: CallSuccess(iresult),
                Result: iresult
            };
        };
    };
    let unbox = function (caller) {
        return () => {
            var iresult = caller();
            return iresult ? iresult.Result : null;
        };
    }
    return unbox(() => {
        return cacheData.getCacheData(options.Key, options.Version, options.Date, box(caller), null, self);
    });
}

/**
 * Called
 * @param {any} context
 */
CacheContext.prototype.Called = function (context) {

}

/**
 * OnException
 * @param {any} context
 */
CacheContext.prototype.OnException = function (context) {
    
}

export const CacheContext;

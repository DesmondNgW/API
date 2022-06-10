"use strict";
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
 * @param {any} context:Context.getActionContext
 * @param {any} caller
 */
CacheContext.prototype.Calling = function(context, caller) {
    let options = this.Options;
    let CallSuccess = options.CallSuccess;
    let args = context.ActionArguments;
    let bindThis = context.ActionBindThis;
    let box = function(caller) {
        return () => {
            let iresult = caller.apply(bindThis, args);
            return {
                Succeed: CallSuccess(iresult),
                Result: iresult
            };
        };
    };
    let unbox = function(caller) {
        return () => {
            var iresult = caller();
            return iresult ? iresult.Result : null;
        };
    }
    return unbox(() => {
        return cacheData.getCacheData(options.Key, options.Version, options.Date, box(caller), args, bindThis);
    });
}

/**
 * Called
 * @param {any} context:Context.getActionContext
 */
CacheContext.prototype.Called = function(context) {
    console.debug("debug:CacheContext-Called;ignore", context);
}

/**
 * OnException
 * @param {any} context:Context.getActionContext
 */
CacheContext.prototype.OnException = function(context) {
    console.debug("debug:CacheContext-OnException;ignore", context);
}

export default CacheContext;
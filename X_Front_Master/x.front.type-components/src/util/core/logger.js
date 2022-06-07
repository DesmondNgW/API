'use strict';

/**function getExceptionStack() {
    try {
        throw new Error("getExceptionStack")
    } catch (error) {
        console.error(error)
        console.trace();
        return error;
    }
}
getExceptionStack();
 * */

function Logger() { }

/**
 * getClientOptions
 * */
Logger.prototype.getClientOptions = function () {
    let nav = navigator || {};
    let doc = document || {};
    let screen = (window || {}).screen || {};
    let timing = ((window || {}).performance || {}).timing || {
        domContentLoadedEventEnd: 0,
        navigationStart: 0
    };
    return {
        userAgent: nav.userAgent,
        lang: nav.language,
        platform: nav.platform,
        domain: doc.domain,
        title: doc.title,
        referrer: doc.referrer,
        screen: {
            width: screen.width,
            height: screen.height
        },
        timing: timing,
        url: {
            href: location.href,
            search: location.search,
            hash: location.hash
        }
    };
}

/**
 * getRequestOptions
 * @param {any} paramList
 */
Logger.prototype.getRequestOptions = function (paramList) {
    return {
        now: new Date(),
        caller: arguments.callee.caller,
        paramList: paramList
    }
}

/**
 * getResponseOptions
 * @param {any} paramList
 * @param {any} error
 * @param {any} result
 * @param {any} elapsed
 */
Logger.prototype.getResponseOptions = function (paramList, error, result, elapsed) {
    return {
        now: new Date(),
        caller: arguments.callee.caller,
        paramList: paramList,
        error: error,
        result: result,
        elapsed: elapsed
    }
}

/**
 * logRequest
 * @param {any} paramList
 */
Logger.prototype.logRequest = function (paramList) {
    let client = this.getClientOptions();
    let request = this.getRequestOptions(paramList);
    console.info("%s request userAgent: %s, url: %s, search: %s, hash: %s.",
        request.now.format("yyyy-MM-dd HH:mm:ss.f"),
        client.userAgent,
        client.url.href,
        client.url.search,
        client.url.hash);
    if (request.caller) {
        console.trace("method info: %s, paramList: %s,", request.caller, request.paramList)
    }
}



export default {};

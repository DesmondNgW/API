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

function getMainOptions() {
    let nav = navigator || {};
    let doc = document || {};
    let screen = (window || {}).screen || {};
    let timing = ((window || {}).performance || {}).timing || {
        domContentLoadedEventEnd: 0,
        navigationStart: 0
    };
    return {
        now: new Date(),
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

function getSubOptions(paramList, error, result, elapsed) {
    return {
        caller: arguments.callee.caller,
        paramList: paramList,
        error: error,
        result: result,
        elapsed: elapsed
    }
}

function logRequest(paramList, error, result, elapsed) {
    let main = getMainOptions();
    let sub = getSubOptions(paramList, error, result, elapsed);
    console.info("%s url:%s,search:%s,hash:%s.", main.now.format("yyyy-MM-dd HH:mm:ss.f"), main.url.href, main.url.search, main.url.hash);
    if (sub.caller) {
        console.info("%s ")
    }



}



export default {};

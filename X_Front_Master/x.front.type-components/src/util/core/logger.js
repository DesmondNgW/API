"use strict";
import "./extend/date.js";
import { cache } from "./cache.js";
import { store } from "./store.js";

function Logger(name) {
    this.name = name;
}

/**
 * getClientOptions
 * */
Logger.prototype.getClientOptions = function() {
    let nav = navigator || {};
    let doc = document || {};
    let screen = (window || {}).screen || {};
    let timing = ((window || {}).performance || {}).timing || {
        domContentLoadedEventEnd: 0,
        navigationStart: 0
    };
    let result = {
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
    const key = "util.core.logger.logClient";
    if (!store.get(key, "local")) {
        store.set(key, result, "local");
    }
    if (cache.get(key)) {
        cache.set(key, result);
    }
    return result;
}

/**
 * getRequestOptions
 * @param {any} paramList
 * @param {any} channel
 * @param {any} func
 */
Logger.prototype.getRequestOptions = function(paramList, channel, func) {
    return {
        now: new Date(),
        channel: channel,
        func: func,
        paramList: paramList
    }
}

/**
 * getResponseOptions
 * @param {any} paramList
 * @param {any} error
 * @param {any} result
 * @param {any} elapsed
 * @param {any} channel
 * @param {any} func
 */
Logger.prototype.getResponseOptions = function(paramList, error, result, elapsed, channel, func) {
    return {
        now: new Date(),
        channel: channel,
        func: func,
        paramList: paramList,
        error: error,
        result: result,
        elapsed: elapsed
    }
}

/**
 * logRequest
 * @param {any} paramList
 * @param {any} caller
 */
Logger.prototype.logRequest = function(paramList, channel, func) {
    let client = this.getClientOptions();
    let request = this.getRequestOptions(paramList, channel, func);
    console.info("%s LOGREQUEST: request userAgent: %s, url: %s, search: %s, hash: %s.",
        request.now.format("yyyy-MM-dd HH:mm:ss.f"),
        client.userAgent,
        client.url.href,
        client.url.search,
        client.url.hash);
    if (request.channel) {
        console.trace("method info: %s.%s, paramList: %s,", request.channel.Client.constructor, request.func, request.paramList)
    } else {
        console.info("paramList: %s.", request.paramList);
    }
}

/**
 * logResponse
 * @param {any} paramList
 * @param {any} error
 * @param {any} result
 * @param {any} elapsed
 * @param {any} caller
 */
Logger.prototype.logResponse = function(paramList, error, result, elapsed, channel, func) {
    let client = this.getClientOptions();
    let response = this.getResponseOptions(paramList, error, result, elapsed, channel, func);
    console.info("%s LOGRESPONSE: request userAgent: %s, url: %s, search: %s, hash: %s.",
        response.now.format("yyyy-MM-dd HH:mm:ss.f"),
        client.userAgent,
        client.url.href,
        client.url.search,
        client.url.hash);
    if (response.channel) {
        console.trace("method info: %s.%s, paramList: %s,", response.channel.Client.constructor, response.func, response.paramList)
    } else {
        console.info("paramList: %s.", response.paramList);
    }
    if (response.error) {
        console.error("error: %s.", response.error);
    } else {
        if (response.result) {
            console.info("result: %s.", response.result);
        }
        console.info("response elapsed: %s ms.", response.elapsed);
    }
}

/**
 * logClient
 * */
Logger.prototype.logClient = function() {
    console.debug(this.getClientOptions());
}

/**
 * logError
 * @param {any} log
 */
Logger.prototype.logError = function(log) {
    console.error("error: %s.", log);
}

/**
 * logWarn
 * @param {any} log
 */
Logger.prototype.logWarn = function(log) {
    console.warn("warn: %s.", log);
}

/**
 * logDebug
 * @param {any} log
 */
Logger.prototype.logDebug = function(log) {
    console.debug("debug: %s.", log);
}
export const logger = new Logger("util.core.logger");
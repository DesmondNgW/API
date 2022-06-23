'use strict';

import "../util/core/extend/date.js";
import { access } from "../util/core/access.js";
import { IProvider } from "../util/core/provider.js";
import { CacheContextAttribute } from "../util/extend/cacheContextAttribute.js";

import { logger } from "../util/core/logger.js";

import { target } from "./target.js";

let channel = new IProvider.Provider(target);
let contextList = [CacheContextAttribute("test.test.key", "0.0.0.1", new Date().add(30, 'n'), (iresult) => {
    return iresult && iresult.Result && iresult.Succeed;
})];

logger.logClient();

export const test = function() {
    var arr = [];
    arr.push(access.tryCall(target.add, [1, 2], this, channel, contextList));
    arr.push(access.tryCall(target.pow, [3, 2], this, channel, contextList));
    arr.push(access.tryCall(target.pow, [2, 3], this, channel, contextList));
    arr.push(access.tryCall(target.sqrt, [2], this, channel, contextList));
    arr.push(access.tryCall(target.sqrt, [3], this, channel, contextList));
}
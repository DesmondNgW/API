'use strict';
import { context } from "./context.js";


function Access(name) {
    this.name = name;
}

Access.prototype.tryCall = function (func, args, bindThis, channel, contextList, maxRetryCounts = 0) {
    let contextAction = context.GetActionContext(args);
    let list = context.getContext(channel, contextList);
    let result;
    try {
        var iresult = context.getCaller(list, contextAction, func).apply(bindThis, args);
        result = iresult;
        context.addResultToActionContext(contextAction, iresult, null);
        for (let i = 0, len = list.length; i < len; i++) {
            list[i].Called(contextAction);
        }
    } catch (error) {
        context.addResultToActionContext(contextAction, result, error);
        for (let i = 0, len = list.length; i < len; i++) {
            list[i].OnException(contextAction);
        }
        if (maxRetryCounts > 0) return tryCall(func, args, bindThis, channel, contextList, maxRetryCounts - 1);
    }
    return result;
}

export const access = new Access("util.core.access");
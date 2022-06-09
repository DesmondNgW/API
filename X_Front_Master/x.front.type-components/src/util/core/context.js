import ChannelContext from "./channelContext.js";
import LoggerContext from "./loggerContext.js";

function Context(name) {
    this.name = name;
}

/**
 * getCaller
 * @param {any} list
 * @param {any} context:Context.getActionContext
 * @param {any} caller
 */
Context.prototype.getCaller = function(list, context, caller) {
    return list.reduce((current, item) => { return item.Calling(context, current); }, caller);
}

/**
 * getContext
 * @param {any} Provider
 * @param {any} contextList:{Array[]{Type:Attribute}}
 */
Context.prototype.getContext = function(Provider, contextList) {
    var attr = [];
    attr.push(new ChannelContext(Provider));
    attr.push(new LoggerContext(Provider));
    if (contextList && contextList.length) {
        for (let i = 0, len = contextList.length, item = contextList[i]; i < len; i++) {
            if (item) {
                attr.push(item(Provider));
            }
        }
    }
    return attr;
}

/**
 * getActionContext
 * @param {any} values:paramsList
 */
Context.prototype.getActionContext = function(values) {
    return {
        ActionArguments: values
    };
}

/**
 * addResultToActionContext
 * @param {any} context
 * @param {any} result
 * @param {any} error
 */
Context.prototype.addResultToActionContext = function(context, result, error) {
    context.Result = result;
    context.Error = error;
    return context;
}
export const context = new Context("util.core.context");
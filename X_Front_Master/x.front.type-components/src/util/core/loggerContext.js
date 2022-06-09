import { logger } from "./logger.js";

function LoggerContext(Provider) {
    this.Provider = Provider;
}

/**
 * Calling
 * @param {any} context:Context.getActionContext
 * @param {any} caller
 */
LoggerContext.prototype.Calling = function(context, caller) {
    logger.logRequest(context.ActionArguments);
    let now = +new Date();
    context.stopElapsed = function() {
        return +new Date() - now;
    }
    return caller;
}

/**
 * Called
 * @param {any} context:Context.getActionContext
 */
LoggerContext.prototype.Called = function(context) {
    let elapsed = -1;
    if (context && context.stopElapsed) {
        elapsed = context.stopElapsed();
    }
    logger.logResponse(context.ActionArguments, null, context.Result, elapsed)
}

/**
 * OnException
 * @param {any} context:Context.getActionContext
 */
LoggerContext.prototype.OnException = function(context) {
    logger.logError(context.Error);
}

export default LoggerContext;
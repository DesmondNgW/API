"use strict";
import { IProvider } from "./provider.js";

function ChannelContext(Provider) {
    this.Provider = Provider;
}

/**
 * Calling
 * @param {any} context:Context.getActionContext
 * @param {any} caller
 */
ChannelContext.prototype.Calling = function(context, caller) {
    return caller;
}

/**
 * Called
 * @param {any} context:Context.getActionContext
 */
ChannelContext.prototype.Called = function(context) {
    console.debug("debug:ChannelContext-Called;ignore", context);
    if (this.Provider instanceof IProvider.ClientProvider) {
        this.Provider.ReleaseClient();
    }
}

/**
 * OnException
 * @param {any} context:Context.getActionContext
 */
ChannelContext.prototype.OnException = function(context) {
    console.debug("debug:ChannelContext-OnException;ignore", context);
    if (this.Provider instanceof IProvider.ClientProvider) {
        this.Provider.Dispose();
    }
}

export default ChannelContext;
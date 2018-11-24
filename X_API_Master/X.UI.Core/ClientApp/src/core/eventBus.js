/**
 * EventBus
 */
class EventBus {
    constructor() {
        this.events = this.events || {};
    }
}

EventBus.prototype.emit = function (type) {
    let e = this.events[type] || [];
    const ret = {};
    if (Array.isArray(e)) {
        for (let i = 0; i < e.length; i++) {
            ret[e[i].id] = e[i].apply(e[i].context || this, e[i].args);
        }
    } else {
        ret[e[0].id] = e[0].apply(e[0].context || this, e[0].args);
    }
    return ret;
}

EventBus.prototype.addListener = function (type, fun) {
    let e = this.events[type] = this.events[type] || [];
    if (!fun.id) fun.id = +new Date();
    e.push(fun);
    return function () {
        if (e.length) {
            for (let i = 0; i < e.length; i++) {
                if (e[i].id === fun.id) {
                    e.splice(i, 1);
                    break;
                }
            }
        }
    }
}

EventBus.prototype.removeAllListener = function (type) {
    let e = this.events[type]
    if (e) {
        e.length = 0;
    }
}

const eventBus = new EventBus();
export default eventBus;
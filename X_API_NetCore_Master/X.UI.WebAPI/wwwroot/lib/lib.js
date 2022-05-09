(function(factory) {
    typeof define === "function" && define.cmd ? define(factory) : (typeof exports !== "undefined" && typeof module !== "undefined" && module.exports) ? factory(require, exports, module) : factory();
})(function (require, exports, module) {

    /**
     * execution code
     */
    (function(global, undefined) {
        if (global.library) return;
        function isType(type) {
            return function(obj) { return {}.toString.call(obj) === "[object " + type + "]"; };
        }
        function extend(a, b) {
            a = a || {};
            for (var n in b) if (b.hasOwnProperty(n))a[n] = b[n];
            return a;
        }

        var library = global.library = {
                verison: "0.0.1",
                data: {
                    events: {}
                },
                cache: {}
            },
            data = library.data,
            cache = library.cache,
            events = data.events;
        /**
         * Events Suports
         */

        extend(library, {
            on: function(obj, name, callback) {
                var id = obj["__id"] = obj["__id"] || +new Date() + Math.ceil(1 + Math.random() * 10000).toString(),
                    collection = events[id] || (events[id] = {}),
                    list = collection[name] || (collection[name] = []);
                list.push(callback);
                return library;
            },
            off: function(obj, name, callback) {
                var id = obj["__id"];
                if (id != undefined) {
                    if (!(name || callback)) {
                        events[id] = data.events[id] = {};
                        return library;
                    }
                    var list, collection = events[id] || {};
                    if ((list = collection[name]) && callback) {
                        for (var i = list.length - 1; i >= 0; i--) {
                            if (list[i] === callback) list.splice(i, 1);
                        }
                    } else if (list) delete events[id][name];
                }
                return library;
            },
            emit: function(obj, name, data) {
                var list, id = obj["__id"], collection = events[id] || {};
                if ((list = collection[name]) && (list = list.slice())) {
                    for (var i = 0, len = list.length; i < len; i++) list[i](data);
                }
                return library;
            }
        });

        var emit = library.emit,
            on = library.on,
            off = library.off,
            isObject = isType("Object"),
            isString = isType("String"),
            isArray = Array.isArray || isType("Array"),
            isFunction = isType("Function");





    })(window);
});


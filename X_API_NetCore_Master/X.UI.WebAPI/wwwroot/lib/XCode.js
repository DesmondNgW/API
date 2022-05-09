/* 定义全局变量 */
var UNDEFINED,
    doc = document,
    win = window,
    ieVersion = getIEVersion() || doc.documentMode || 0,
    Opt = Object.prototype.toString,
    Ap = Array.prototype,
    isIE = !!ieVersion,
    external = win.external,
    nav = win.navigator,
    ua = nav.userAgent,
    mathAbs = Math.abs,
    mathRound = Math.round,
    mathFloor = Math.floor,
    mathCeil = Math.ceil,
    mathMax = Math.max,
    mathMin = Math.min,
    mathCos = Math.cos,
    mathSin = Math.sin,
    mathPI = Math.PI,
    deg2rad = mathPI * 2 / 360,
    garbageBin,

    /* Color */

    rgbaRegEx = /rgba\(\s*([0-9]{1,3})\s*,\s*([0-9]{1,3})\s*,\s*([0-9]{1,3})\s*,\s*([0-9]?(?:\.[0-9]+)?)\s*\)/,
    hexRegEx = /#([a-fA-F0-9]{2})([a-fA-F0-9]{2})([a-fA-F0-9]{2})/,
    rgbRegEx = /rgb\(\s*([0-9]{1,3})\s*,\s*([0-9]{1,3})\s*,\s*([0-9]{1,3})\s*\)/,

    /* Selector */

    snack = /(?:[\w\-\\.#]+)+(?:\[\w+?=([\'"])?(?:\\\1|.)+?\1\])?|\*|>/ig,
    exprClassName = /^(?:[\w\-_]+)?\.([\w\-_]+)/,
    exprId = /^(?:[\w\-_]+)?#([\w\-_]+)/,
    exprNodeName = /^([\w\*\-_]+)/,
    uniqueArg = { uid: +new Date(), n: 1 },
    na = [null, null],
    resizeT = 0,

    /* animate */

    Tween = {
        linear: function (t, b, c, d) { return c * t / d + b; },
        easeIn: function (t, b, c, d) { return c * (t /= d) * t + b; },
        easeOut: function (t, b, c, d) { return -c * (t /= d) * (t - 2) + b; },
        easeInOut: function (t, b, c, d) { return ((t /= d / 2) < 1) ? (c / 2 * t * t + b) : (-c / 2 * ((--t) * (t - 2) - 1) + b); }
    },

    /* JSON */

    cx = /[\u0000\u00ad\u0600-\u0604\u070f\u17b4\u17b5\u200c-\u200f\u2028-\u202f\u2060-\u206f\ufeff\ufff0-\uffff]/g,
    escapable = /[\\\"\x00-\x1f\x7f-\x9f\u00ad\u0600-\u0604\u070f\u17b4\u17b5\u200c-\u200f\u2028-\u202f\u2060-\u206f\ufeff\ufff0-\uffff]/g,
    meta = { '\b': '\\b', '\t': '\\t', '\n': '\\n', '\f': '\\f', '\r': '\\r', '"': '\\"', '\\': '\\\\' },

    /* userAgent */
    engines = [{
        name: 'trident', versionSearch: 'trident/'
    }, {
        name: 'webkit', versionSearch: 'webkit/'
    }, {
        name: 'gecko', versionSearch: 'rv:'
    }, {
        name: 'presto', versionSearch: 'presto/'
    }],
    platforms = [{
        name: 'windows phone', versionSearch: 'windows phone os ', versionNames: [{ number: '7.5', name: 'mango' }]
    }, {
        name: 'win', slugName: 'windows', versionSearch: 'windows(?: nt)? ', versionNames: [{ number: '6.2', name: 'windows 8' }, { number: '6.1', name: 'windows 7' }, { number: '6.0', name: 'windows vista' }, { number: '5.2', name: 'windows xp' }, { number: '5.1', name: 'windows xp' }, { number: '5.0', name: 'windows 2000' }]
    }, {
        name: 'ipad', versionSearch: 'cpu os ', flag: 'ios'
    }, {
        name: 'ipod', versionSearch: 'iphone os ', flag: 'ios'
    }, {
        name: 'iphone', versionSearch: 'iphone os ', flag: 'ios'
    }, {
        name: 'mac', versionSearch: 'os x ', versionNames: [{ number: '10.8', name: 'mountainlion' }, { number: '10.7', name: 'lion' }, { number: '10.6', name: 'snowleopard' }, { number: '10.5', name: 'leopard' }, { number: '10.4', name: 'tiger' }, { number: '10.3', name: 'panther' }, { number: '10.2', name: 'jaguar' }, { number: '10.1', name: 'puma' }, { number: '10.0', name: 'cheetah' }]
    }, {
        name: 'android', versionSearch: 'android ', versionNames: [{ number: '4.1', name: 'jellybean' }, { number: '4.0', name: 'icecream sandwich' }, { number: '3.', name: 'honey comb' }, { number: '2.3', name: 'ginger bread' }, { number: '2.2', name: 'froyo' }, { number: '2.', name: 'eclair' }, { number: '1.6', name: 'donut' }, { number: '1.5', name: 'cupcake' }]
    }, {
        name: 'blackberry', versionSearch: '(?:blackberry\\d{4}[a-z]?|version)/'
    }, {
        name: 'bb', slugName: 'blackberry', versionSearch: '(?:version)/'
    }, {
        name: 'playbook', slugName: 'blackberry', versionSearch: '(?:version)/'
    }, {
        name: 'linux'
    }, {
        name: 'nokia'
    }],
    mutilcore = [{
        name: 'BIDUBrowser', slugName: '百度浏览器', versionSearch: 'BIDUBrowser.'
    }, {
        name: 'QQBrowser', slugName: 'QQ浏览器', versionSearch: 'QQBrowser.'
    }, {
        name: '360se', slugName: '360安全浏览器', versionSearch: serachVersion(null, 'twGetVersion', 'twGetSecurityID', 'twGetRunPath')
    }, {
        name: '360ss', slugName: '360极速浏览器', versionSearch: 'theworld.'//??
    }, {
        name: 'Maxthon', slugName: '遨游浏览器', versionSearch: serachVersion('Maxthon.', 'max_version', 'mxVersion')
    }, {
        name: 'theworld', slugName: '世界之窗', versionSearch: 'theworld.'//??
    }, {
        name: 'lbbrowser', slugName: '猎豹浏览器', versionSearch: serachVersion(null, 'LiebaoGetVersion')
    }, {
        name: ' Ubrowser', slugName: 'UC浏览器', versionSearch: ' Ubrowser.'
    }, {
        name: 'se .+ metasr ', slugName: '搜狗高速浏览器', versionSearch: serachVersion(null, 'SEVersion')
    }],
    browsers = [{
        name: 'iemobile', versionSearch: 'iemobile/'
    }, {
        name: 'msie', slugName: 'Microsoft Internet Explorer', versionSearch: isIE && function () { return ieVersion; }
    }, {
        name: 'firefox', versionSearch: 'firefox.'
    }, {
        name: 'opr|opera', slugName: 'opera', versionSearch: '(?:opr|opera).'
    }, {
        name: 'chrome', versionSearch: 'chrome.'
    }, {
        name: 'safari', versionSearch: '(?:browser|version).'
    }],
    MutilCore = parseUserAgent(mutilcore, ua);

/* Function & Object */

function extend(a, b) {
    a = a || {};
    for (var n in b) {
        a[n] = b[n];
    }
    return a;
}

function doCopy(copy, original) {
    copy = isObject(copy) && copy || {};
    for (var key in original) {
        if (original.hasOwnProperty(key)) {
            var value = original[key];
            copy[key] = value && isObject(value) && doCopy(copy[key] || {}, value) || !defined(copy[key]) && value || splat(copy[key]).concat(splat(value));
        }
    }
    return copy;
}

function merge() {
    for (var i = 0, ret = {}, len = arguments.length; i < len; i++) {
        ret = doCopy(ret, arguments[i]);
    }
    return ret;
}

function pick() {
    for (var i = 0, item, len = arguments.length; i < len; i++) {
        item = arguments[i];
        if (item !== UNDEFINED && item !== null)
            return item;
    }
}

function hash() {
    for (var i = 0, arr = Ap.slice.call(arguments), len = arr.length, obj = {}; i < len; i++) {
        obj[arr[i++]] = arr[i];
    }
    return obj;
}

function destroyObjectProperties(obj, except) {
    for (var n in obj) {
        obj[n] && obj[n] !== except && obj[n].destroy && obj[n].destroy();
        delete obj[n];
    }
}

function extendClass(parent, members) {
    var object = function () { };
    object.prototype = new parent();
    extend(object.prototype, members);
    return object;
}

function wrap(obj, method, func) {
    var proceed = obj[method];
    obj[method] = function () {
        var args = Ap.slice.call(arguments);
        args.reverse().push(proceed);
        args.reverse();
        return func.apply(this, args);
    };
}

function inObject(obj, item) {
    for (var key in obj) {
        if (obj.hasOwnProperty(key) && obj[key] === item) {
            return !0;
        }
    }
    return !1;
}

/* 类型判断 */

function isString(obj) {
    return Opt.call(obj) === '[object String]';
}

function isObject(obj) {
    return Opt.call(obj) === '[object Object]';
}

function isArray(obj) {
    return Array.isArray ? Array.isArray(obj) : Opt.call(obj) === '[object Array]';
}

function isCollection(obj) {
    return typeof obj === 'object';
}

function isFunction(obj) {
    return Opt.call(obj) === '[object Function]';
}

function isNumber(obj) {
    return Opt.call(obj) === '[object Number]';
}

function isBoolean(obj) {
    return Opt.call(obj) === '[object Boolean]';
}

function isEmptyObject(obj) {
    for (var i in obj) {
        if (obj.hasOwnProperty(i)) {
            return !1;
        }
    }
    return !0;
}

function defined(obj) {
    return obj !== UNDEFINED && obj !== null;
}

function isWindow(obj) {
    return defined(obj.window) && (obj.window === obj.window.window);
}

/* 处理数字&&字符串 */

function pInt(s, mag) {
    return parseInt(s, mag || 10);
}

function log2lin(num) {
    return Math.log(num) / Math.LN10;
}

function lin2log(num) {
    return Math.pow(10, num);
}

function correctFloat(num) {
    return parseFloat(num.toPrecision(14));
}

function getMagnitude(num) {//num的近似值
    return math.pow(10, mathFloor(math.log(num) / math.LN10));
}

function pad(number, length) {
    return new Array((length || 2) + 1 - String(number).length).join(0) + number;
}

function numberFormat(number, decimals, decPoint, thousandsSep) {
    var n = +number || 0, c = decimals === -1 ? (n.toString().split('.')[1] || '').length : (isNaN(decimals = mathAbs(decimals)) ? 2 : decimals), d = decPoint, t = thousandsSep, s = n < 0 ? "-" : "", i = String(pInt(n = mathAbs(n).toFixed(c))), j = i.length > 3 ? i.length % 3 : 0;
    return s + (j ? i.substr(0, j) + t : "") + i.substr(j).replace(/(\d{3})(?=\d)/g, "$1" + t) + (c ? d + mathAbs(n - i).toFixed(c).slice(2) : "");
}

function dateFormat(format, timestamp, capitalize) {
    if (!defined(timestamp) || isNaN(timestamp)) {
        return 'Invalid date';
    }
    format = pick(format, '%Y-%m-%d %H:%M:%S');
    var date = new Date(timestamp), key, hours = date[getHours](), dayOfWeek = date[getDay](), dayOfMonth = date[getDate](), month = date[getMonth](), fullYear = date[getFullYear](), replacements = { 'd': pad(dayOfMonth), 'e': dayOfMonth, 'm': pad(month + 1), 'y': fullYear.toString().substr(2, 2), 'Y': fullYear, 'H': pad(hours), 'I': pad((hours % 12) || 12), 'l': (hours % 12) || 12, 'M': pad(date[getMinutes]()), 'p': hours < 12 ? 'AM' : 'PM', 'P': hours < 12 ? 'am' : 'pm', 'S': pad(date.getSeconds()), 'L': pad(mathRound(timestamp % 1000), 3) };
    for (key in replacements) {
        while (format.indexOf('%' + key) !== -1) {
            format = format.replace('%' + key, typeof replacements[key] === 'function' ? replacements[key](timestamp) : replacements[key]);
        }
    }
    return capitalize ? format.substr(0, 1).toUpperCase() + format.substr(1) : format;
}

function trim(text) {
    return text.replace(/^\s+|\s+$/g, '');
}

/* 处理数组 */

function erase(arr, item) {
    var i = arr.length;
    while (i--) {
        if (arr[i] === item) {
            arr.splice(i, 1);
            break;
        }
    }
    return arr;
}

function arrayObjectSort(arr, sortFunction) {
    for (var i = 0, length = arr.length; i < length; i++) {
        arr[i]._index = i;
    }
    arr.sort(function (a, b) {
        var sortValue = sortFunction(a, b);
        return sortValue === 0 ? a._index - b._index : sortValue;
    });
    for (i = 0; i < length; i++) {
        delete arr[i]._index;
    }
}

function arrayExtremum(data) {
    var i = data.length, max = min = data[0];
    while (i--) {
        max = (data[i] < max) ? max : data[i];
        min = (data[i] > min) ? min : data[i];
    }
    return [max, min];
}

function unique(arr) {
    for (var i = 0, len = arr.length, tmp = {}, ret = []; i < len; i++) {
        if (isCollection(arr[i])) {
            arr[i].__index = arr[i].__index || +new Date() + mathCeil(1 + Math.random() * 10000).toString();
            !defined(tmp[arr[i].__index]) && (tmp[arr[i].__index] = 0, ret.push(arr[i]));
        } else {
            !defined(tmp[arr[i]]) && (tmp[arr[i]] = 0, ret.push(arr[i]));
        }
    }
    for (var i = 0, len = arr.length; i < len; i++) {
        isCollection(arr[i]) && delete arr[i].__index;
    }
    return ret;
}

function map(arr, fn) {
    for (var i = 0, len = arr.length, results = []; i < len; i++) {
        results[i] = fn.call(arr[i], arr[i], i, arr);
    }
    return results;
}

function each(arr, fn) {
    return Ap.forEach ? Ap.forEach.call(arr, fn) : function (arr, fn) {
        for (var i = 0, length = arr.length; i < length; i++) {
            if (fn.call(arr[i], arr[i], i, arr) === !1) {
                return i;
            }
        }
    }.call(arr, arr, fn);
}

function splat(obj) {
    return isArray(obj) ? obj : [obj];
}

function inArray(arr, item) {
    for (var i = 0, len = arr.length; i < len; i++) {
        if (arr[i] === item) {
            return !0;
        }
    }
    return !1;
}

function indexOf(arr, item, index) {
    return Ap.indexOf ? Ap.indexOf.call(arr, item, index) : function (arr, item, index) {
        for (var start = mathMax(isNumber(index) ? index : 0, 0), i = start, len = arr.length; i < len; i++) {
            if (arr[i] === item) {
                return i;
            }
        }
        return -1;
    }.call(arr, arr, item, index)
}

function lastIndexOf(arr, item, index) {
    return Ap.indexOf ? Ap.lastIndexOf.call(arr, item, index) : function (arr, item, index) {
        for (var start = mathMax(isNumber(index) ? index : 0, 0), len = arr.length, i = len - 1; i >= start ; i--) {
            if (arr[i] === item) {
                return i;
            }
        }
        return -1;
    }.call(arr, arr, item, index)
}

function filterArray(a, b) {
    var c = [], tmp = {};
    for (var i = 0, len = a.length; i < len; i++) {
        isCollection(a[i]) ? (a[i].__index = +new Date() + mathCeil(1 + Math.random() * 10000).toString(), tmp[a[i].__index] = 1) : (tmp[a[i]] = 1);
    }
    for (var i = 0, len = b.length; i < len; i++) {
        (isCollection(b[i]) && b[i].__index && tmp[b[i].__index] || !isCollection(b[i]) && tmp[b[i]]) && c.push(b[i]);
    }
    for (var i = 0, len = a.length; i < len; i++) {
        isCollection(a[i]) && delete a[i].__index;
    }
    return c;
}

function notArray(b, a) {
    var c = [], tmp = {};
    for (var i = 0, len = a.length; i < len; i++) {
        isCollection(a[i]) ? (a[i].__index = +new Date() + mathCeil(1 + Math.random() * 10000).toString(), tmp[a[i].__index] = 1) : (tmp[a[i]] = 1);
    }
    for (var i = 0, len = b.length; i < len; i++) {
        (isCollection(b[i]) && !(b[i].__index && tmp[b[i].__index]) || !isCollection(b[i]) && !tmp[b[i]]) && c.push(b[i]);
    }
    for (var i = 0, len = arr.length; i < len; i++) {
        isCollection(a[i]) && delete a[i].__index;
    }
    return c;
}

function toArray(obj) {
    return Ap.slice.call(obj);
}

function toHash(arr) {
    for (var i = 0, len = arr.length, obj = {}, tmp, tr; i < len; i++) {
        if (arr[i].indexOf(':') > -1) {
            tmp = arr[i].replace(':', ';&nbsp;').split(';&nbsp;');
            tr = trim(tmp[1]);
            obj[trim(tmp[0])] = /^[0-9]+$/.test(tr) ? Number(tr) : tr;
        }
    }
    return obj;
}

/* 处理HTML */

function contains(a, b) {
    try {
        return a.contains ? a != b && a.contains(b) : !!(a.compareDocumentPosition(b) & 16);
    } catch (e) {
    }
}

function createElement(tag, attribs, styles, parent, nopad) {
    var el = doc.createElement(tag);
    attribs && extend(el, attribs);
    nopad && css(el, { 'padding': '0', 'border': 'none', 'margin': '0' });
    styles && css(el, styles);
    parent && parent.appendChild(el);
    return el;
}

function discardElement(element) {
    garbageBin = garbageBin || createElement('div');
    element && garbageBin.appendChild(element);
    garbageBin.innerHTML = '';
}

function attr(elem, prop, value) {
    var key, setAttribute = 'setAttribute', ret, common = { 'id': 'id', 'class': 'className', 'style': 'style', 'title': 'title', 'value': 'value', 'checked': 'checked', 'disabled': 'disabled' };
    if (isString(prop)) {
        if (defined(value))
            common[prop] ? (elem[common[prop]] = value) : elem[setAttribute](prop, value);
        else if (elem)
            return common[prop] ? elem[common[prop]] : elem.getAttribute ? elem.getAttribute(prop) : '';

    } else if (defined(prop) && isObject(prop)) {
        for (key in prop) {
            common[key] ? (elem[common[key]] = prop[key]) : elem[setAttribute](key, prop[key]);
        }
    }
    return elem;
}

function next(elem) {
    while (elem = elem.nextSibling) {
        if (elem.nodeType === 1)
            break;
    }
    return elem || null;
}

function prev(elem) {
    while (elem = elem.previousSibling) {
        if (elem.nodeType === 1)
            break;
    }
    return elem || null;
}

function offset(elem) {
    if (doc = elem && (elem.ownerDocument || document)) {
        var doc, docElem = doc.documentElement, body = doc.body, clientRect = elem.getBoundingClientRect();
        return {
            left: clientRect.left + (docElem && docElem.scrollLeft || body && body.scrollLeft || 0) - (docElem.clientLeft || body.clientLeft || 0),
            top: clientRect.top + (docElem && docElem.scrollTop || body && body.scrollTop || 0) - (docElem.clientTop || body.clientTop || 0)
        }
    }
}

function Text2HTML(text) {
    return String(text).replace(/&/g, '&amp;').replace(/</g, '&lt;').replace(/>/g, '&gt;').replace(/\\/g, '&#92;').replace(/"/g, '&quot;').replace(/'/g, '&#39;');
}

function parseHTML(html) {
    var div = createElement('div'), fragment = doc.createDocumentFragment(), children, child;
    div.innerHTML = html;
    children = div.children;
    while (child = children[0]) {
        fragment.appendChild(child);
    }
    return fragment;
}

function replaceHTML(elem, html) {
    try {
        elem.innerHTML = html;
    } catch (e) {
        var children = elem.children, child;
        while (child = children[0]) {
            child.parentNode.removeChild(child)
        }
        html = trim(html);
        if (/^<tr.+?>|<tbody.+?>|<thead.+?>|<tfoot.+?>|<col.+?>|<colgroup.+?>/i.test(html)) {
            html = '<table>' + html + '</table>';
            elem.appendChild(parseHTML(html).firstChild);
        } else if (/^<td.+?>|<th.+?>/i.test(html)) {
            html = '<table><tr>' + html + '</tr></table>';
            elem.appendChild(parseHTML(html).firstChild.firstChild);
        }
    }
}

/* 处理CSS */

function isOpacity() {
    var e = doc.createElement('div');
    e.innerHTML = '<a href="/a" style="float:left;opacity:.25;">a</a>';
    return /^0.25$/.test(e.firstChild.style.opacity);
}

function Convert(str) {
    return str.replace(/(?:-)(.)/, function (a, b) { return a.toUpperCase(); }).replace('-', '');
}

function css(elem, prop, value) {
    var support = isOpacity(), key;
    if (isString(prop)) {
        prop = Convert(prop === 'opacity' && !support && 'filter' || prop);
        if (defined(value)) {
            elem.style[prop] = prop === 'filter' ? 'alpha(opacity=' + (value * 100) + ')' : value;
        } else {
            return prop === 'filter' ? parseFloat(elem.style[prop].match(/alpha\(opacity=(\d+)\)/i)[1]) / 100 : elem.style[prop];
        }
    } else if (defined(prop) && isObject(prop)) {
        !support && prop && prop.opacity !== UNDEFINED && (prop.filter = 'alpha(opacity=' + (prop.opacity * 100) + ')');
        for (key in prop) {
            elem.style[Convert(key)] = prop[key];
        }
    }
    return elem;
}

function getComputeStyle(el, name) {
    if (el.currentStyle) {
        name = Convert(name === 'opacity' && !isOpacity() && 'filter' || name);
        return name === 'filter' ? parseFloat(el.currentStyle[name].match(/alpha\(opacity=(\d+)\)/i)[1]) / 100 : el.currentStyle[name];
    } else {
        return (el.ownerDocument || document).defaultView.getComputedStyle(el, null).getPropertyValue(name);
    }
}

function addClass(el, name) {
    var nameList = name.split(' '), i = nameList.length;
    while (i--) {
        el.className = new RegExp("(\s*)" + nameList[i] + "(\s*)", "ig").test(el.className) && el.className || (el.className + " " + nameList[i]).replace(/^\s|\s$/g, '');
    }
}

function removeClass(el, name) {
    var nameList = name.split(' '), i = nameList.length;
    while (i--) {
        el.className = el.className.replace(new RegExp("(\s*)" + nameList[i] + "(\s*)", "ig"), '').replace(/^\s|\s$/g, '').replace(/\s+/g, ' ');
    }
}

function hasClass(el, name) {
    return new RegExp("(\s*)" + name + "(\s*)", "ig").test(el.className);
}

/* 处理JSON转化 */

/* 
* If the string contains no control characters, no quote characters, and no
* backslash characters, then we can safely slap some quotes around it.
* Otherwise we must also replace the offending characters with safe escape sequences.
*/

function replaceQuote(a) {
    var c = meta[a];
    return isString(c) ? c : '\\u' + pad(a.charCodeAt(0).toString(16), 4);
}

function quote(string) {
    escapable.lastIndex = 0;
    return escapable.test(string) ? '"' + string.replace(escapable, replaceQuote) + '"' : '"' + string + '"';
}

/* Produce a string from holder[key] */

function str(key, holder, rep, gap, indent) {
    var ret, mind = gap, value = holder[key];
    value && isObject(value) && isFunction(value.toJSON) && (value = value.toJSON(key));
    isFunction(rep) && (value = rep.call(holder, key, value));
    switch (typeof value) {
        case 'string':
            ret = quote(value);
            break;
        case 'number':
            ret = isFinite(value) ? String(value) : 'null';
            break;
        case 'boolean':
        case 'null':
            ret = String(value);
            break;
        case 'object':
            if (!value) {
                ret = 'null';
            } else {
                gap += indent;
                var partial = [];
                if (isArray(value)) {
                    for (var i = 0, length = value.length; i < length; i++) {
                        partial.push(str(i, value, rep, gap, indent) || 'null');
                    }
                    ret = partial.length === 0 ? '[]' : gap ? '[\n' + gap + partial.join(',\n' + gap) + '\n' + mind + ']' : '[' + partial.join(',') + ']';
                } else {
                    if (rep && isCollection(rep)) {
                        for (var i = 0, length = rep.length; i < length; i++) {
                            var k = rep[i];
                            if (isString(k)) {
                                if (ret = str(k, value, rep, gap, indent)) {
                                    partial.push(quote(k) + (gap ? ': ' : ':') + ret);
                                }
                            }
                        }
                    } else {
                        for (var k in value) {
                            if (Object.hasOwnProperty.call(value, k)) {
                                if (ret = str(k, value, rep, gap, indent)) {
                                    partial.push(quote(k) + (gap ? ': ' : ':') + ret);
                                }
                            }
                        }
                    }
                    ret = partial.length === 0 ? '{}' : gap ? '{\n' + gap + partial.join(',\n' + gap) + '\n' + mind + '}' : '{' + partial.join(',') + '}';
                }
            }
            break;
        default:
            break;
    }
    return ret;
}

/* The walk method is used to recursively walk the resulting structure so that modifications can be made. */

function walk(holder, key, reviver) {
    var ret, value = holder[key];
    if (value && isCollection(value)) {
        for (var k in value) {
            if (Object.hasOwnProperty.call(value, k)) {
                ret = walk(value, k, reviver);
                if (ret !== UNDEFINED) {
                    value[k] = ret;
                } else {
                    delete value[k];
                }
            }
        }
    }
    return reviver.call(holder, key, value);
}

/* 处理Ajax */

function createXHR() {
    return win.XMLHttpRequest ? new win.XMLHttpRequest() : function (arr, i) {
        while (i--) {
            try {
                return new win.ActiveXObject(arr[i]);
            } catch (e) { }
        }
    }.call(this, ['MSXML2.XMLHTTP', 'MSXML2.XMLHTTP.4.0', 'MSXML2.XMLHTTP.6.0'], 3);
}

function createDOMDocument(responseText) {
    return function (arr, i) {
        var dom = null;
        while (i--) {
            try {
                dom = new win.ActiveXObject(arr[i]);
                dom.loadXML(responseText);
            } catch (e) { }
        }
        return dom;
    }.call(this, ["MSXML.DOMDocument", "Microsoft.XMLDOM", "MSXML2.DOMDocument", "MSXML2.DOMDocument.3.0", "MSXML2.DOMDocument.4.0", "MSXML2.DOMDocument.5.0"], 6);
}

function getJSON(option) {
    extend(option, {
        url: option.url || null,
        charset: option.charset || "utf-8",
        dataType: option.dataType || "JSON",
        jsonpCallback: option.jsonpCallback || "callback_" + (+new Date()),
        success: option.success || function () { },
        complete: option.complete || function () { },
        timeout: option.timeout || 15000,
        error: option.error || function () { }
    });
    var head = doc.getElementsByTagName('head')[0], script = doc.createElement('script'), cb = option.jsonpCallback, isJSONP = option.dataType.toLowerCase() === 'jsonp', requestDone = !1, id;
    isJSONP && (url += (url.indexOf("?") === -1 ? "?" : "&") + "callback=" + cb);
    extend(script, {
        charset: option.charset,
        type: 'text/javascript',
        src: option.url
    });
    head.appendChild(script);
    id = setTimeout(function () {
        option.error();
        option.complete();
        isJSONP && delete win[cb];
        script && head.removeChild(script);
    }, option.timeout);
    if (!isJSONP) {
        script.onload = script.onreadystatechange = function () {
            if (!requestDone && (!script.readyState || script.readyState === 'loaded' || script.readyState === 'complete')) {
                requestDone = !0;
                clearTimeout(id);
                option.success();
                option.complete();
                head.removeChild(script);
            }
        };
    } else {
        win[cb] = function (data) {
            clearTimeout(id);
            option.success(data);
            option.complete(data);
            try {
                delete win[cb];
                head.removeChild(script);
            } catch (e) {
                win[cb] = null;
            }
        };
    }
}

function wrapAjax(option) {
    extend(option, {
        data: option.data || null,
        type: { "get": "GET", "post": "POST", "head": "HEAD" }[option.type.toLowerCase()] || "GET",
        url: option.url || null,
        contentType: option.contentType || "application/x-www-form-urlencoded",
        success: option.success || function () { },
        complete: option.complete || function () { },
        ansync: option.ansync || !0,
        timeout: option.timeout || 180000,
        error: option.error || function () { }
    });
    if (option.url) {
        if (option.type === "GET" && option.data !== null && option.data !== '') {
            option.url = option.url.split('?')[0] + '?' + option.data.replace(/[\{\}\s\'\"]/g, '').replace(/:/g, '=').replace(/,/g, '&');
        }
        var xhr = createXHR(), requestDone = !1, id, datajson;
        xhr.open(option.type, option.url, option.ansync);
        id = setTimeout(function () {
            requestDone = !0;
            xhr.abort();
        }, option.timeout);
        if (option.type === "HEAD" && option.data !== null && option.data !== '') {
            datajson = mjson.parse(option.data);
            for (var item in datajson) {
                xhr.setRequestHeader(item, datajson[item]);
            }
        } else if (option.type !== "HEAD") {
            xhr.setRequestHeader("Content-type", option.contentType);
        }
        xhr.onreadystatechange = function () {
            if (xhr.readyState === 4 && !requestDone) {
                clearTimeout(id);
                requestDone = !0;
                if ((xhr.status >= 200 && xhr.status < 300 || xhr.status === 304)) {
                    option.success(option.type === "HEAD" ? toHash(xhr.getAllResponseHeaders().split('\r\n')) : (xhr.responseXML ? xhr.responseXML.documentElement : (createDOMDocument(xhr.responseText) || xhr.responseText)));
                } else {
                    option.error(xhr, xhr.statusText);
                }
                option.complete(xhr, xhr.statusText)
                xhr = null;
            }
        }
        option.type === "POST" ? xhr.send(option.data) : xhr.send(null);
    }
}

/* 处理Event */

function wrapEvent(event) {
    if (event["wrap"]) return event;
    event.preventDefault && delete event.returnValue;
    event = extend({ originalEvent: event }, event);
    event["wrap"] = !0;
    event.preventDefault = function () {
        event.originalEvent.preventDefault && event.originalEvent.preventDefault();
        event.originalEvent.returnValue = !1;
    };
    event.stopPropagation = function () {
        event.originalEvent.stopPropagation && event.originalEvent.stopPropagation();
        event.originalEvent.cancelBubble = !0;
    };
    event.timeStamp = event.timeStamp || +new Date();
    event.target = event.target || event.srcElement || doc;
    event.target = event.target.nodeType === 3 && event.target.parentNode || event.target;
    event.relatedTarget = event.relatedTarget || event.fromElement && event.fromElement === event.target ? event.toElement : event.fromElement;
    if (event.pageX === null && event.clientX !== null) {
        var docElem = doc.documentElement, body = doc.body;
        event.pageX = event.clientX + (docElem && docElem.scrollLeft || body && body.scrollLeft || 0) - (docElem.clientLeft || 0);
        event.pageY = event.clientY + (docElem && docElem.scrollTop || body && body.scrollTop || 0) - (docElem.clientTop || 0);
    }
    event.which = event.which || event.charCode || event.keyCode || 0;
    event.metaKey = event.metaKey || event.ctrlKey || !1;
    event.button = event.button && (doc.addEventListener ? { 0: 1, 1: 2, 2: 3 } : { 1: 1, 4: 2, 2: 3 })[event.button] || (event.which ? event.which - 1 : -1);
    event.textSelected = getSelectedText(event.target);
    event.select = function (startIndex, endIndex) {
        return selectText(event.target, startIndex, endIndex);
    }
    return event;
}

function wrapHandel(el, type, fn) {
    if (el.addEventListener) {
        if (!!{ 'mouseleave': !0, 'mouseenter': !0 }[type]) {
            return function (e) {
                var wrap = wrapEvent(e);
                !contains(wrap.currentTarget, wrap.relatedTarget) && wrap.currentTarget != wrap.relatedTarget && fn.call(wrap.currentTarget, wrap);
            }
        } else {
            return function (e) { fn.call(el, wrapEvent(e)); };
        }
    } else if (el.attachEvent) {
        return function () { fn.call(el, wrapEvent(win.event)); };
    } else {
        return function () { };
    }
}

function add(el, type, fn) {
    if (el.addEventListener) {
        if (type === 'mouseenter') {
            el.addEventListener('mouseover', fn, !1);
        } else if (type === 'mouseleave') {
            el.addEventListener('mouseout', fn, !1);
        } else {
            el.addEventListener(type, fn, !1);
        }
    } else if (el.attachEvent) {
        if (type === 'resize') {
            el.attachEvent('on' + type, function (e) { if (resizeOnce()) { fn.apply(this, [e]) } });
        } else {
            el.attachEvent('on' + type, fn);
        }
    } else {
        return !1;
    }
}

function remove(el, type, fn) {
    if (el.removeEventListener) {
        el.removeEventListener(type, fn, !1);
    } else if (el.detachEvent) {
        el.detachEvent('on' + type, fn);
    } else {
        return !1;
    }
}

function fireEvent(el, type) {
    var doc = el.ownerDocument || document, evt = doc.createEvent && doc.createEvent('MouseEvents') || doc.createEventObject();
    doc.createEvent && evt.initMouseEvent(type, !0, !0, doc.defaultView, 0, 0, 0, 0, 0, !1, !1, !1, !1, 0, null);
    return el.dispatchEvent && el.dispatchEvent(evt) || !el.dispatchEvent && el.fireEvent('on' + type, evt);
}

/* 处理DomReady */

function DOMReady() { }

function DOMContentLoaded() {
    if (doc.removeEventListener) {
        doc.removeEventListener("DOMContentLoaded", DOMContentLoaded, !1);
        DOMReady();
    } else if (doc.detachEvent && doc.documentElement.doScroll) {
        try {
            doc.documentElement.doScroll("left");
            doc.detachEvent("onreadystatechange", DOMContentLoaded)
            DOMReady();
        } catch (error) { }
    }
}

function bindDOMReady() {
    var toplevel = null;
    if (doc.addEventListener) {
        doc.addEventListener("DOMContentLoaded", DOMContentLoaded, !1);
        win.addEventListener("load", DOMContentLoaded, !1);
    } else if (doc.attachEvent) {
        doc.attachEvent("onreadystatechange", DOMContentLoaded);
        win.attachEvent("onload", DOMContentLoaded);
        try {
            toplevel = win.frameElement === null;
        } catch (e) { }
        doc.documentElement.doScroll && toplevel && doScrollCheck();
    }
}

function doScrollCheck() {
    try {
        doc.documentElement.doScroll("left");
        DOMReady();
    } catch (error) {
        setTimeout(doScrollCheck, 1);
        return;
    }
}

/*
* "mini" Selector Engine
* Copyright (c) 2009 James Padolsey
* -------------------------------------------------------
* Dual licensed under the MIT and GPL licenses.
*	- http://www.opensource.org/licenses/mit-license.php
*	- http://www.gnu.org/copyleft/gpl.html
* -------------------------------------------------------
* Version: 0.01 (BETA)
*/

/* Transforms a node collection into a real array */

function realArray(c) {
    try {
        return Ap.slice.call(c);
    } catch (e) {
        var ret = [];
        for (var i = 0, len = c.length; i < len; ++i) {
            ret[i] = c[i];
        }
        return ret;
    }
}

/* Filters a collection by an attribute. */

function filterByAttr(collection, attr, regex) {
    var i = -1, node, r = -1, ret = [];
    while ((node = collection[++i])) {
        if (regex.test(node[attr])) {
            ret[++r] = node;
        }
    }
    return ret;
}

/* This is what you call via x() Starts everything off... */

function Selector(selector, context) {
    context = context || doc;
    var simple = /^[\w\-_#]+$/.test(selector);
    if (!simple && context.querySelectorAll) {
        return realArray(context.querySelectorAll(selector));
    }
    if (selector.indexOf(',') > -1) {
        var split = selector.split(/,/g), ret = [];
        for (var sIndex = 0, len = split.length; sIndex < len; ++sIndex) {
            ret = ret.concat(Selector(split[sIndex], context));
        }
        return SelectorUnique(ret);
    }
    var parts = selector.match(snack), part = parts.pop(), id = (part.match(exprId) || na)[1], className = !id && (part.match(exprClassName) || na)[1], nodeName = !id && (part.match(exprNodeName) || na)[1], collection;
    if (className && !nodeName && context.getElementsByClassName) {
        collection = realArray(context.getElementsByClassName(className));
    } else {
        collection = !id && realArray(context.getElementsByTagName(nodeName || '*'));
        if (className) {
            collection = filterByAttr(collection, 'className', RegExp('(^|\\s)' + className + '(\\s|$)'));
        }
        if (id) {
            var byId = doc.getElementById(id);
            return byId ? [byId] : [];
        }
    }
    return parts[0] && collection[0] ? filterParents(parts, collection) : collection;
}

/* This is where the magic happens. Parents are stepped through (upwards) to see if they comply with the selector. */

function filterParents(selectorParts, collection, direct) {
    var parentSelector = selectorParts.pop();
    if (parentSelector === '>') {
        return filterParents(selectorParts, collection, !0);
    }
    var ret = [], r = -1, id = (parentSelector.match(exprId) || na)[1], className = !id && (parentSelector.match(exprClassName) || na)[1], nodeName = !id && (parentSelector.match(exprNodeName) || na)[1], cIndex = -1, node, parent, matches;
    nodeName = nodeName && nodeName.toLowerCase();
    while ((node = collection[++cIndex])) {
        parent = node.parentNode;
        do {
            matches = !nodeName || nodeName === '*' || nodeName === parent.nodeName.toLowerCase();
            matches = matches && (!id || parent.id === id);
            matches = matches && (!className || RegExp('(^|\\s)' + className + '(\\s|$)').test(parent.className));
            if (direct || matches) {
                break;
            }
        } while ((parent = parent.parentNode));
        if (matches) {
            ret[++r] = node;
        }
    }
    return selectorParts[0] && ret[0] ? filterParents(selectorParts, ret) : ret;
}

function miniData(elem) {
    var cacheIndex = elem[uniqueArg.uid], nextCacheIndex = uniqueArg.n++;
    if (!cacheIndex) {
        elem[uniqueArg.uid] = nextCacheIndex;
        return !0;
    }
    return !1;
}

/* Returns a unique array */

function SelectorUnique(arr) {
    var ret = [], r = -1
    for (var i = 0, length = arr.length, item; i < length; ++i) {
        item = arr[i];
        if (miniData(item)) {
            ret[++r] = item;
        }
    }
    uniqueArg.uid += 1;
    return ret;
}

/* 处理杂项 */

function imageCache() {
    ieVersion < 7 && ieVersion > 0 && doc.execCommand('BackgroundImageCache', !1, !0);
}

function getIEVersion() {
    var v = 3, p = doc.createElement('p');
    do {
        p.innerHTML = '<!--[if gt IE ' + (++v) + ']><i></i><![endif]-->'
    } while (p.getElementsByTagName('i').length > 0);
    return v > 4 ? v : 0;
}

function getSelectedText(el) {
    var d = el.ownerDocument || doc;
    return d.selection ? d.selection.createRange().text : (el.tagName == 'INPUT' && el.type.toLowerCase() == 'text' || el.tagName == 'TEXTAREA' ? el.value.substring(el.selectionStart, el.selectionEnd) : null);
}

function selectText(el, startIndex, stopIndex) {
    if (el.setSelectionRange) {
        el.setSelectionRange(startIndex, stopIndex);
    } else if (el.createTextRange) {
        var range = el.createTextRange();
        range.collapse(!0);
        range.moveStart("character", startIndex);
        range.moveEnd("character", stopIndex - startIndex);
        range.select();
    }
    el.focus();
}

function resizeOnce() {
    var ret = !1, now = +(new Date());
    (Number(now) - resizeT > 300) && (ret = !0, resizeT = Number(now));
    return ret;
}

function parseUserAgent(rules, ua) {
    for (var i = 0, key, item = { name: '', version: '' }, len = rules.length, rule; i < len; i++) {
        var rule = rules[i], name = rule.name, search = rule.versionSearch, flag = rule.flag, verNames = rule.versionNames;
        if (new RegExp(name, "i").test(ua) || isFunction(search)) {
            item.name = rule.slugName && rule.slugName || name.replace(/\s/g, '');
            item.version = isFunction(search) ? search() : String(ua.match(new RegExp(search + '([\\d\\._]+)*', 'i'))[1] || 0).replace(/_/g, '.');
            item.fullName = verNames && function (version, arr) {
                for (var i = 0, len = arr.length; i < len; i++) {
                    var item = arr[i];
                    if (version.indexOf(item.number) === 0) {
                        return item.name;
                    }
                }
            }.call(this, item.version, verNames);
            item.flagName = flag;
            switch (rules) {
                case browsers:
                    break;
                case engines:
                    break;
                case mutilcore:
                    item.mode = ieVersion && 'IE ' + ieVersion + ' Mode' || 'Fast Mode';
                    break;
                case platforms:
                    item.category = (/tablet/.test(ua) || /ipad/i.test(item.name) || (/android/i.test(item.name) && !/mobile/.test(ua))) && "Tablet" || (/mobile|phone/.test(ua) || /blackberry/i.test(item.name)) && "Mobile" || "Desktop";
                    item.os = nav.platform.toLowerCase();
                    item.fullName = /ios/i.test(item.flagName) ? 'ios' + parseInt(item.version, 10) : item.fullName;
                    break;
            }
            break;
        }
    }
    for (key in item) {
        !defined(item[key]) && delete item[key]
    }
    return item;
}

function serachVersion() {
    var i, item, len, flags = [], f = arguments[0] || '';
    if (external) {
        for (i = 1, len = arguments.length; i < len; i++) {
            item = arguments[i];
            (item in external) && flags.push(item);
        }
    }
    return flags.length ? function () {
        var i = 0, item;
        while (i < flags.length && !(item = isFunction(external[flags[i]]) && external[flags[i]]() || !isFunction(external[flags[i]]) && external[flags[i]])) { i++; }
        return !isEmptyObject(item) && item || String(ua.match(new RegExp(f + '([\\d\\._]+)*', 'i'))[1] || 0).replace(/_/g, '.');
    } : false;
}

/* Wrap Object */

function Color(input) {
    this.rgba = [], this.stops;
    var colorObject = this;
    (function (input) {
        if (input && input.stops) {
            this.stops = map(input.stops, function (stop) {
                return new Color(stop[1]);
            });
        } else {
            var result;
            this.rgba = (result = rgbaRegEx.exec(input)) && [pInt([1]), pInt(result[2]), pInt(result[3]), parseFloat(result[4], 10)] || (result = hexRegEx.exec(input)) && [pInt(result[1], 16), pInt(result[2], 16), pInt(result[3], 16), 1] || (result = rgbRegEx.exec(input)) && [pInt(result[1]), pInt(result[2]), pInt(result[3]), 1] || this.rgba;
        }
    }).call(this, input);

    !isFunction(Color.prototype.get) && extend(Color.prototype, {
        get: function (format) {
            var ret;
            if (colorObject.stops) {
                ret = merge(input);
                ret.stops = [].concat(ret.stops);
                each(colorObject.stops, function (stop, i) {
                    ret.stops[i] = [ret.stops[i][0], stop.get(format)];
                });
            } else if (colorObject.rgba && !isNaN(colorObject.rgba[0])) {
                ret = format === 'rgb' && 'rgb(' + colorObject.rgba[0] + ',' + colorObject.rgba[1] + ',' + colorObject.rgba[2] + ')' || format === 'a' && colorObject.rgba[3] || 'rgba(' + colorObject.rgba.join(',') + ')';
            } else {
                ret = input;
            }
            return ret;
        },
        brighten: function (alpha) {
            if (colorObject.stops) {
                each(colorObject.stops, function (stop) {
                    stop.brighten(alpha);
                });
            } else if (isNumber(alpha) && alpha !== 0) {
                for (var i = 0; i < 3; i++) {
                    colorObject.rgba[i] += pInt(alpha * 255);
                    colorObject.rgba[i] = mathMax(mathMin(255, colorObject.rgba[i]), 0);
                }
            }
            return colorObject;
        },
        setOpacity: function (alpha) {
            if (colorObject.stops) {
                each(colorObject.stops, function (stop) {
                    stop.setOpacity(alpha);
                });
            } else {
                colorObject.rgba[3] = alpha;
            }
            return colorObject;
        }
    });
}

function Event() {
    !isFunction(Event.prototype.bind) && extend(Event.prototype, {
        fix: {},
        bind: function (el, type, fn) {
            var handel = wrapHandel(el, type, fn), eid = el.guid = el.guid || +new Date() + mathCeil(1 + Math.random() * 10000).toString(), fid = +new Date() + mathCeil(1 + Math.random() * 10000).toString(), EventStore, FnStore;
            (((EventStore = {})[eid] = {})[type] = {})[fid] = handel;
            fn.guid = fn.guid || {};
            (fn.guid[el.guid + type] = fn.guid[el.guid + type] || []).push(fid);
            handel.original = fn;
            this.fix = merge(this.fix, EventStore);
            add(el, type, handel);
        },
        unbind: function (el, type, fn) {
            if (type === UNDEFINED) {
                if (el.guid) {
                    var tmp = this.fix[el.guid];
                    for (var type in tmp) {
                        for (var guid in tmp[type]) {
                            remove(el, type, tmp[type][guid]);
                            var t = tmp[type][guid]['original']['guid'];
                            delete t[el.guid + type];
                            isEmptyObject(t) && delete t;
                        }
                    }
                    delete tmp;
                }
            } else if (fn === UNDEFINED) {
                if (el.guid) {
                    var tmp = this.fix[el.guid][type];
                    for (var guid in tmp) {
                        remove(el, type, tmp[guid]);
                        var t = tmp[guid]['original']['guid'];
                        delete t[el.guid + type];
                        isEmptyObject(t) && delete t;
                    }
                    delete tmp;
                }
            } else if (el.guid) {
                var guid = isFunction(fn) && fn['guid'][el.guid + type][0] || isString(fn) && fn || null;
                if (guid && (tmp = this.fix[el.guid][type]) && tmp[guid]) {
                    remove(el, type, tmp[guid]);
                    var t = tmp[guid]['original']['guid'];
                    t[el.guid + type].pop();
                    t[el.guid + type].length === 0 && delete t[el.guid + type];
                    isEmptyObject(t) && delete t;
                    delete tmp[guid];
                }
            }
        },
        fireEvent: function (el, type) {
            fireEvent(el, type);
        }
    });
}

function Ajax() {
    !isFunction(Ajax.prototype.post) && extend(Ajax.prototype, {
        post: function (url, option) {
            option.url = url;
            option.type = "POST";
            wrapAjax(option);
        },
        get: function (url, option) {
            option.url = url;
            option.type = "GET";
            wrapAjax(option);
        },
        ajax: function (option) {
            wrapAjax(option);
        },
        getJSON: function (option) {
            getJSON(option);
        }
    });
}

function Ready(callback) {
    if (Ready.prototype.callBackQueue === UNDEFINED) {
        extend(Ready.prototype, {
            readyBound: !1,
            isReady: !1,
            callBackQueue: [],
            DOMReady: function () {
                if (!Ready.prototype.isReady) {
                    Ready.prototype.isReady = !0;
                    var cb = null;
                    while (cb = Ready.prototype.callBackQueue.shift()) {
                        cb.call(cb);
                    }
                }
            }
        });
        isFunction(callback) && Ready.prototype.callBackQueue.push(callback);
        DOMReady = Ready.prototype.DOMReady;
        if (!Ready.prototype.readyBound) {
            bindDOMReady();
            Ready.prototype.readyBound = !0;
        }
    } else
        isFunction(callback) && Ready.prototype.callBackQueue.push(callback);
}

function MyJSON() {
    if (!isFunction(Date.prototype.toJSON)) {
        Date.prototype.toJSON = function (key) {
            return isFinite(this.valueOf()) ? this.getUTCFullYear() + '-' + pad(this.getUTCMonth() + 1) + '-' + pad(this.getUTCDate()) + 'T' + pad(this.getUTCHours()) + ':' + pad(this.getUTCMinutes()) + ':' + pad(this.getUTCSeconds()) + 'Z' : null;
        };
        String.prototype.toJSON = Number.prototype.toJSON = Boolean.prototype.toJSON = function (key) {
            return this.valueOf();
        };
    }
    if (!isFunction(MyJSON.prototype.stringify)) {
        MyJSON.prototype.stringify = win.JSON && isFunction(win.JSON.stringify) && win.JSON.stringify || function (value, replacer, space) {
            var indent = isNumber(space) && space > 0 && new Array(space + 1).join(' ') || isString(space) && space || '';
            if (replacer && !isFunction(replacer) && (typeof replacer !== 'object' || !isNumber(replacer.length))) {
                throw new Error('JSON.stringify');
            }
            return str('', { '': value }, replacer, '', indent);
        }
        MyJSON.prototype.parse = win.JSON && isFunction(win.JSON.parse) && win.JSON.parse || function (text, reviver) {
            text = String(text);
            cx.lastIndex = 0;
            text = !cx.test(text) && text || text.replace(cx, function (a) { return '\\u' + pad(a.charCodeAt(0).toString(16), 4); });
            if (/^[\],:{}\s]*$/.test(text.replace(/\\(?:["'\\\/bfnrt]|u[0-9a-fA-F]{4})/g, '@').replace(/"[^"\\\n\r]*"|'[^"\\\n\r]*'|true|false|null|-?\d+(?:\.\d*)?(?:[eE][+\-]?\d+)?/g, ']').replace(/(?:^|:|,)(?:\s*\[)+/g, ''))) {
                var j = eval('(' + text + ')');
                return isFunction(reviver) ? walk({ '': j }, '', reviver) : j;
            }
            throw new SyntaxError('JSON.parse');
        }
    }
}

function _animate(el, styles, options) {
    var end = function (el, changes, styles) {
        for (var i = 0, attr; i < changes.length; i++) {
            attr = changes[i].attr;
            css(el, attr, styles[attr]);
        }
    },
    d = options.speed, dt, key, b = 0, c = 0, changes = [], callback = options.cb, easing = isFunction(options.easing) && options.easing || isString(options.easing) && Tween[options.easing] || Tween.easeOut, noAnimate = defined(options.noAnimate) && isBoolean(options.noAnimate) && options.noAnimate === !0;
    d = d && { 'fast': 300, 'normal': 700, 'slow': 3000 }[d] || isNumber(d) && d || 700;
    for (var key in styles) {
        b = parseFloat(getComputeStyle(el, key)) || 0;
        c = parseFloat(styles[key]) - b;
        changes.push({ attr: key, b: b, c: c });
    }
    if (el.stop === !0) {
        end(el, changes, styles);
        isFunction(callback) && callback.call(el);
    } else if (!noAnimate) {
        dt = +new Date();
        var cid = el.animateId = setInterval(function () {
            var t = +new Date() - dt, b, c, attr, val;
            for (var i = 0; i < changes.length; i++) {
                attr = changes[i].attr;
                val = easing(t, changes[i].b, changes[i].c, d);
                if (String(styles[attr]).lastIndexOf("px") !== -1) {
                    val = val + "px";
                }
                css(el, attr, val);
            }
            if (d <= t) {
                end(el, changes, styles);
                clearInterval(cid);
                delete el.animateId;
                isFunction(callback) && callback.call(el);
            }
        }, 1000 / 60);
    } else {
        end(el, changes, styles);
        isFunction(callback) && callback.call(el);
    }
}

function Animate() {
    !isFunction(Animate.prototype.Queue) && extend(Animate.prototype, {
        Queue: {},
        addQueue: function (el, fn, args) {
            var eid = el.guid = el.guid || +new Date() + mathCeil(1 + Math.random() * 10000).toString();
            this.Queue[eid] = this.Queue[eid] || {};
            (this.Queue[eid]['Argument'] = this.Queue[eid]['Argument'] || []).push(args);
            (this.Queue[eid]['Function'] = this.Queue[eid]['Function'] || []).push(fn);
            if (this.Queue[eid] && this.Queue[eid]['Function'].length === 1) {
                fn.call(this, el);
            }
        },
        removeQueue: function (el) {
            if (el.guid && this.Queue[el.guid]) {
                var f = this.Queue[el.guid]['Function'], a = this.Queue[el.guid]['Argument'];
                f.reverse().pop();
                a.reverse().pop();
                a.reverse();
                f.reverse();
                if (f.length > 0) {
                    var fn = f[0];
                    fn.call(this, el);
                }
            }
        },
        delay: function (el) {
            var args = this.Queue[el.guid]['Argument'][0];
            var time = isNumber(args[0]) && args[0] > 0 && args[0] || 1000;
            if (el.stop === !0) {
                Animate.prototype.removeQueue(el);
            } else {
                var cid = el.animateId = setInterval(function () {
                    Animate.prototype.removeQueue(el);
                    clearInterval(cid);
                    delete el.animateId;
                }, time);
            }
        },
        fadeIn: function (el) {
            var args = this.Queue[el.guid]['Argument'][0], speed = args[0], callback = args[1], none = getComputeStyle(el, 'display') === 'none', opacity = parseFloat(getComputeStyle(el, 'opacity'));
            none && css(el, { 'display': 'block', 'opacity': '0' });
            _animate(el, { 'opacity': opacity }, {
                speed: speed,
                easing: Tween.easeIn,
                noAnimate: !none,
                cb: function () {
                    css(el, { 'display': 'block' });
                    isFunction(callback) && callback.call(el);
                    Animate.prototype.removeQueue(el);
                }
            });
        },
        fadeOut: function (el) {
            var args = this.Queue[el.guid]['Argument'][0], speed = args[0], callback = args[1], none = getComputeStyle(el, 'display') === 'none', opacity = parseFloat(getComputeStyle(el, 'opacity'));
            _animate(el, { 'opacity': 0 }, {
                speed: speed,
                easing: Tween.easeOut,
                noAnimate: none,
                cb: function () {
                    css(el, { 'display': 'none', 'opacity': '' });
                    isFunction(callback) && callback.call(el);
                    Animate.prototype.removeQueue(el);
                }
            });
        },
        fadeTo: function (el) {
            var args = this.Queue[el.guid]['Argument'][0], speed = args[0], opacity = args[1], callback = args[2], none = getComputeStyle(el, 'display') === 'none', c = parseFloat(getComputeStyle(el, 'opacity'));
            none && css(el, { 'display': 'block', 'opacity': '0' });
            _animate(el, { 'opacity': opacity }, {
                speed: speed,
                easing: (c <= opacity) ? Tween.easeIn : Tween.easeOut,
                noAnimate: !(isNumber(opacity) && opacity <= 1 && opacity >= 0),
                cb: function () {
                    var display = (opacity === 0) ? 'none' : 'block';
                    css(el, { 'display': "'" + display + "'" });
                    isFunction(callback) && callback.call(el);
                    Animate.prototype.removeQueue(el);
                }
            });
        },
        slideDown: function (el) {
            var args = this.Queue[el.guid]['Argument'][0], speed = args[0], callback = args[1], none = getComputeStyle(el, 'display') === 'none', height = parseFloat(getComputeStyle(el, 'height'));
            none && css(el, { 'display': 'block', 'height': '0px' });
            _animate(el, { 'height': height + 'px' }, {
                speed: speed,
                easing: Tween.easeIn,
                noAnimate: !none,
                cb: function () {
                    css(el, { 'display': 'block' });
                    isFunction(callback) && callback.call(el);
                    Animate.prototype.removeQueue(el);
                }
            });
        },
        slideUp: function (el) {
            var args = this.Queue[el.guid]['Argument'][0], speed = args[0], callback = args[1], none = getComputeStyle(el, 'display') === 'none';
            _animate(el, { 'height': '0px' }, {
                speed: speed,
                easing: Tween.easeOut,
                noAnimate: none,
                cb: function () {
                    css(el, { 'height': '', 'display': 'none' });
                    isFunction(callback) && callback.call(el);
                    Animate.prototype.removeQueue(el);
                }
            });
        },
        animate: function (el) {
            var args = this.Queue[el.guid]['Argument'][0], styles = args[0], options = args[1], callback = options.callback;
            options.cb = function () {
                isFunction(callback) && callback.call(el);
                Animate.prototype.removeQueue(el);
            }
            _animate(el, styles, options);
        },
        stop: function (el, stopAll, goToEnd) {
            stopAll = defined(stopAll) && isBoolean(stopAll) && stopAll === !0, goToEnd = defined(goToEnd) && isBoolean(goToEnd) && goToEnd === !0;
            if (el.animateId) {
                clearInterval(el.animateId);
                delete el.animateId;
                if (goToEnd) {
                    el.stop = !0;
                    this.Queue[eid]['Function'][0].call(this, el);
                    delete el.stop;
                }
                if (stopAll) {
                    this.Queue[eid] = {};
                }
            }
        }
    })
}

/* $$对外API */

function $$(selector, context) {
    function dom(arr) {
        for (var i = 0, tmp = [], len = arr.length; i < len; i++) {
            arr[i] && arr[i].nodeType && tmp.push(arr[i]);
        }
        return tmp;
    }
    if (!selector) {
        return $$([], context);
    }
    if (isFunction(selector)) {
        return Ready.call(context || this, selector);
    } else if (selector instanceof $$) {
        return selector;
    } else if (!(this instanceof $$)) {
        return new $$(selector, context);
    }
    var myObj = splat(isString(selector) && Selector(selector, context) || (selector.nodeType || isWindow(selector)) && selector || isArray(selector) && SelectorUnique(dom(selector)));
    this.get = function (num) { return isNumber(num) ? (num < 0 ? myObj[myObj.length + num] : myObj[num]) : toArray(myObj); };
    for (var i = 0, len = myObj.length; i < len; i++) {
        this[i] = myObj[i];
    }
    this['length'] = myObj.length;
}

/* 初始化init */

var ajax = new Ajax();
var MyEvent = new Event();
var mjson = new MyJSON();
var animate = new Animate();
$$.ready = function (a) { return Ready(a); }
$$.color = function (input) {
    if (!(this instanceof Color)) {
        return new Color(input);
    }
}
extend($$, {
    version: '1.0.0',
    post: ajax.post,
    get: ajax.get,
    ajax: ajax.ajax,
    getJSON: ajax.getJSON,
    isFunction: function (a) { return isFunction(a) },
    isArray: function (a) { return isArray(a) },
    isObject: function (a) { return isObject(a) },
    isCollection: function (a) { return isCollection(a) },
    isEmptyObject: function (a) { return isEmptyObject(a) },
    isNumber: function (a) { return isNumber(a) },
    isBoolean: function (a) { return isBoolean(a) },
    isString: function (a) { return isString(a) },
    defined: function (a) { return defined(a) },
    pInt: function (a, b) { return pInt(a, b); },
    correctFloat: function (a) { return correctFloat(a) },
    quote: function (a) { return quote(a) },
    pad: function (a, b) { return pad(a, b); },
    trim: function (a) { return trim(a) },
    numberFormat: function (a, b, c, d) { numberFormat(a, b, c, d) },
    dateFormat: function (a, b, c) { dateFormat(a, b, c) },
    inArray: function (a, b) { return inArray(a, b); },
    indexOf: function (a, b, c) { return indexOf(a, b, c); },
    lastIndexOf: function (a, b, c) { return lastIndexOf(a, b, c); },
    arrayExtremum: function (a) { return arrayExtremum(a) },
    sort: function (a, b) { arrayObjectSort(a, b); },
    toArray: function (o) { return toArray(o); },
    erase: function (a, b) { return erase(a, b); },
    splat: function (a) { return splat(a) },
    unique: function (a) { return unique(a) },
    filterArray: function (a, b) { return filterArray(a, b); },
    notArray: function (a, b) { return notArray(a, b); },
    inObject: function (a) { return inObject(a) },
    extend: function (a, b) { return extend(a, b); },
    destroy: function (a, b) { destroyObjectProperties(a, b); },
    merge: merge,
    hash: hash,
    map: function (a, b) { return map(a, b); },
    extendClass: function (a, b) { return extendClass(a, b); },
    wrap: function (a, b, c) { wrap(a, b, c); },
    parseHTML: function (a) { return parseHTML(a) },
    contains: function (a, b) { return contains(a, b); },
    createElement: function (a, b, c, d, e) { return createElement(a, b, c, d, e); },
    JSON: {
        stringify: mjson.stringify,
        parse: mjson.parse
    },
    imageCache: function () { return imageCache(); },
    ieVersion: ieVersion,
    userAgent: {
        Platform: parseUserAgent(platforms, ua),
        Browser: MutilCore.name && MutilCore || parseUserAgent(browsers, ua),
        Engine: parseUserAgent(engines, ua)
    }
});

/* api */

$$.fn = $$.prototype = {
    constructor: $$,
    each: function (fn) {
        for (var i = 0, len = this.length; i < len ; i++) {
            if (fn.call(this[i], this[i], i, this) === !1) {
                return i;
            }
        }
        return this;
    },
    on: function (type, fn) {
        return isFunction(fn) && this.each(function () {
            for (var i = 0, types = type.split(' '), len = types.length; i < len; i++) {
                types[i] != '' && MyEvent.bind(this, types[i], fn);
            }
        });
    },
    off: function (type, fn) {
        return this.each(function () {
            if (defined(type)) {
                for (var i = 0, types = type.split(' '), len = types.length; i < len; i++) {
                    types[i] != '' && MyEvent.unbind(this, types[i], fn);
                }
            } else {
                MyEvent.unbind(this);
            }
        })
    },
    click: function (fn) {
        return defined(fn) ? this.on('click', fn) : this.each(function () { this.click(); });
    },
    blur: function (fn) {
        return defined(fn) ? this.on('blur', fn) : this.each(function () { this.blur(); });
    },
    focus: function (fn) {
        return defined(fn) ? this.on('focus', fn) : this.each(function () { this.focus(); });
    },
    submit: function (fn) {
        return defined(fn) ? this.on('submit', fn) : this.each(function () { this.submit(); });
    },
    select: function (fn) {
        return defined(fn) ? this.on('select', fn) : this.each(function () { this.select(); });
    },
    load: function (fn) {
        if (defined(fn)) {
            return this.on('load', fn);
        }
    },
    unload: function (fn) {
        if (defined(fn)) {
            return this.on('unload', fn);
        }
    },
    error: function (fn) {
        if (defined(fn)) {
            return this.on('error', fn);
        }
    },
    focusin: function (fn) {
        if (defined(fn)) {
            return this.on('focusin', fn);
        }
    },
    focusout: function (fn) {
        if (defined(fn)) {
            return this.on('focusout', fn);
        }
    },
    change: function (fn) {
        return defined(fn) ? this.on('change', fn) : this.fireEvent('change');
    },
    dblclick: function (fn) {
        return defined(fn) ? this.on('dblclick', fn) : this.fireEvent('dblclick');
    },
    resize: function (fn) {
        return defined(fn) ? this.on('resize', fn) : this.fireEvent('resize');
    },
    scroll: function (fn) {
        return defined(fn) ? this.on('scroll', fn) : this.fireEvent('scroll');
    },
    keyup: function (fn) {
        return defined(fn) ? this.on('keyup', fn) : this.fireEvent('keyup');
    },
    keydown: function (fn) {
        return defined(fn) ? this.on('keydown', fn) : this.fireEvent('keydown');
    },
    keypress: function (fn) {
        return defined(fn) ? this.on('keypress', fn) : this.fireEvent('keypress');
    },
    mouseover: function (fn) {
        return defined(fn) ? this.on('mouseover', fn) : this.fireEvent('mouseover');
    },
    mouseout: function (fn) {
        return defined(fn) ? this.on('mouseout', fn) : this.fireEvent('mouseout');
    },
    mouseleave: function (fn) {
        return defined(fn) ? this.on('mouseleave', fn) : this.fireEvent('mouseleave');
    },
    mouseenter: function (fn) {
        return defined(fn) ? this.on('mouseenter', fn) : this.fireEvent('mouseenter');
    },
    mousedown: function (fn) {
        return defined(fn) ? this.on('mousedown', fn) : this.fireEvent('mousedown');
    },
    mouseup: function (fn) {
        return defined(fn) ? this.on('mouseup', fn) : this.fireEvent('mouseup');
    },
    mousemove: function (fn) {
        return defined(fn) ? this.on('mousemove', fn) : this.fireEvent('mousemove');
    },
    hover: function (enter, leave) {
        return this.each(function () {
            MyEvent.bind(this, 'mouseenter', enter);
            MyEvent.bind(this, 'mouseleave', leave);
        })
    },
    fireEvent: function (type) {
        return this.each(function () {
            for (var i = 0, types = type.split(' '), len = types.length; i < len; i++) {
                MyEvent.fireEvent(this, types[i]);
            }
        })
    },
    addClass: function (name) {
        return this.each(function () {
            addClass(this, name);
        })
    },
    removeClass: function (name) {
        return this.each(function () {
            removeClass(this, name);
        })
    },
    hasClass: function (name) {
        if (this['length'] > 0) {
            return hasClass(this[0], name);
        }
    },
    toggleClass: function (name) {
        return this.each(function () {
            hasClass(this, name) ? removeClass(this, name) : addClass(this, name);
        });
    },
    css: function (prop, value) {
        return (isString(prop) && !defined(value) && this['length'] > 0) ? getComputeStyle(this[0], prop) : this.each(function () { css(this, prop, value); });
    },
    getComputeStyle: function (name) {
        if (this['length'] > 0) {
            return getComputeStyle(this[0], name);
        }
    },
    offset: function () {
        if (this['length'] > 0) {
            return offset(this[0]);
        }
    },
    prev: function () {
        var ret = [];
        this.each(function () {
            var obj = prev(this);
            obj && ret.push(obj);
        })
        return ret.length > 0 && $$(ret) || $$([]);
    },
    next: function () {
        var ret = [];
        this.each(function () {
            var obj = next(this);
            obj && ret.push(obj);
        })
        return ret.length > 0 && $$(ret) || $$([]);
    },
    siblings: function () {
        var ret = [];
        this.each(function () {
            var children = (this.parentNode || this.ownerDocument || doc).children, len = children.length, child;
            while (len--) {
                var child = children[len];
                if (child.nodeType === 1 && child !== this) {
                    ret.push(child);
                }
            }
        })
        return ret.length > 0 && $$(ret) || $$([]);
    },
    attr: function (prop, value) {
        return (isString(prop) && !defined(value) && this['length'] > 0) ? attr(this[0], prop) : this.each(function () { attr(this, prop, value); });
    },
    isChecked: function () {
        return this.attr('checked');
    },
    eq: function (index) {
        index = index < 0 ? index + this.length : index;
        return $$(this[index]);
    },
    gt: function (index) {
        index = index < 0 ? index + this.length : index;
        for (var len = this.length, index = mathMax(-1, index), i = len - index - 2, ret = []; i > -1; i--) {
            ret.push(this[len - i - 1]);
        }
        return $$(ret);
    },
    lt: function (index) {
        index = index < 0 ? index + this.length : index;
        for (var len = this.length, index = mathMin(len, index), i = len - index - 1, ret = []; i < len - 1 ; i++) {
            ret.push(this[i - len + index + 1]);
        }
        return $$(ret);
    },
    first: function () {
        return $$(this[0]);
    },
    last: function () {
        return $$(this[this.length - 1]);
    },
    odd: function () {
        for (var i = 1, ret = [], len = this.length; i < len; i = i + 2) {
            ret.push(this[i]);
        }
        return $$(ret);
    },
    even: function () {
        for (var i = 0, ret = [], len = this.length; i < len; i = i + 2) {
            ret.push(this[i]);
        }
        return $$(ret);
    },
    html: function (text) {
        return (!defined(text) && this['length'] > 0) ? this[0].innerHTML : this.each(function () {
            replaceHTML(this, text);
        });
    },
    val: function (value) {
        return (!defined(value) && this['length'] > 0) ? this[0].value : this.each(function () { this.value = value; });
    },
    data: function (data) {
        return (!defined(data) && this['length'] > 0) ? (this[0].data || null) : this.each(function () { this.data = data; });
    },
    clearData: function () {
        return this.each(function () {
            delete this.data;
        });
    },
    text: function (text) {
        return (!defined(text) && this['length'] > 0) ? (this[0].textContent || this[0].innerText) : this.each(function () { this.innerHTML = Text2HTML(text); });
    },
    hide: function () {
        return this.css('display', 'none');
    },
    show: function () {
        return this.css('display', 'block');
    },
    visible: function () {
        return this.css('visibility', 'visible');
    },
    hidden: function () {
        return this.css('visibility', 'hidden');
    },
    clone: function () {
        var ret = [];
        this.each(function () {
            ret.push(this.cloneNode(!0));
        })
        return $$(ret);
    },
    find: function (selector) {
        var ret = [];
        this.each(function () {
            ret = ret.concat(Selector(selector, this));
        })
        return $$(ret);
    },
    filter: function (selector) {
        var _s = Selector(selector), ret = filterArray(this, _s);
        return $$(ret);
    },
    not: function (selector) {
        var _s = Selector(selector), ret = notArray(this, _s);
        return $$(ret);
    },
    fadeIn: function (speed, callback) {
        return this.each(function () {
            animate.addQueue(this, animate.fadeIn, [speed, callback]);
        })
    },
    fadeOut: function (speed, callback) {
        return this.each(function () {
            animate.addQueue(this, animate.fadeOut, [speed, callback]);
        })
    },
    delay: function (time) {
        return this.each(function () {
            animate.addQueue(this, animate.delay, [time]);
        })
    },
    fadeToggle: function (speed, callback) {
        return this.each(function () {
            if (getComputeStyle(this, 'display') === 'none') {
                animate.addQueue(this, animate.fadeIn, [speed, callback])
            } else {
                animate.addQueue(this, animate.fadeOut, [speed, callback])
            }
        })
    },
    fadeTo: function (speed, opacity, callback) {
        return this.each(function () {
            animate.addQueue(this, animate.fadeTo, [speed, opacity, callback]);
        })
    },
    slideUp: function (speed, callback) {
        return this.each(function () {
            animate.addQueue(this, animate.slideUp, [speed, callback]);
        })
    },
    slideDown: function (speed, callback) {
        return this.each(function () {
            animate.addQueue(this, animate.slideDown, [speed, callback]);
        })
    },
    animate: function (styles, options) {
        return this.each(function () {
            animate.addQueue(this, animate.animate, [styles, options]);
        })
    },
    slideToggle: function (speed, callback) {
        return this.each(function () {
            if (getComputeStyle(this, 'display') === 'none') {
                animate.addQueue(this, animate.slideDown, [speed, callback])
            } else {
                animate.addQueue(this, animate.slideUp, [speed, callback])
            }
        })
    },
    stop: function (stopAll, goToEnd) {
        return this.each(function () {
            animate.stop(this, stopAll, goToEnd);
        });
    },
    remove: function () {
        return this.each(function () {
            discardElement(this);
        });
    },
    empty: function () {
        return this.each(function () {
            this.innerHTML = '';
        })
    },
    style: function (cssText) {
        if (defined(cssText)) {
            return this.each(function () {
                attr(this, 'style', cssText);
            });
        }
    },
    scrollTop: function () {
        if (this['length'] > 0) {
            return (this[0] === doc || this[0] === win) ? doc.documentElement && doc.documentElement.scrollTop || doc.body && doc.body.scrollTop || 0 : this[0].scrollTop;
        }
    },
    scrollLeft: function () {
        if (this['length'] > 0) {
            return (this[0] === doc || this[0] === win) ? doc.documentElement && doc.documentElement.scrollLeft || doc.body && doc.body.scrollLeft || 0 : this[0].scrollLeft;
        }
    },
    width: function () {
        if (this['length'] > 0) {
            var ret = { 'doc': doc.documentElement && doc.documentElement.clientWidth || doc.body && doc.body.clientWidth || 0, 'win': win.screen.availWidth };
            return this[0] === doc ? ret['doc'] : this[0] === win ? ret['win'] : pInt(getComputeStyle(this[0], 'width'));
        }
    },
    height: function () {
        if (this['length'] > 0) {
            var ret = { 'doc': doc.documentElement && doc.documentElement.clientHeight || doc.body && doc.body.clientHeight || 0, 'win': win.screen.availHeight };
            return this[0] === doc ? ret['doc'] : this[0] === win ? ret['win'] : pInt(getComputeStyle(this[0], 'height'));
        }
    },
    innerWidth: function () {
        if (this['length'] > 0) {
            return this[0].clientWidth;
        }
    },
    innerHeight: function () {
        if (this['length'] > 0) {
            return this[0].clientHeight;
        }
    },
    outerWidth: function (b) {
        if (this['length'] > 0) {
            var offsetWidth = this[0].offsetWidth, ext = 0;
            if (isBoolean(b) && b) {
                ext += pInt(getComputeStyle(this[0], 'margin-left'));
                ext += pInt(getComputeStyle(this[0], 'margin-right'));
            }
            return offsetWidth + ext;
        }
    },
    outerHeight: function (b) {
        if (this['length'] > 0) {
            var offsetHeight = this[0].offsetHeight, ext = 0;
            if (isBoolean(b) && b) {
                ext += pInt(getComputeStyle(this[0], 'margin-top'));
                ext += pInt(getComputeStyle(this[0], 'margin-bottom'));
            }
            return offsetHeight + ext;
        }
    },
    parent: function (selector) {
        var ret = [];
        this.each(function () {
            ret.push(this.parentNode || this.ownerDocument || doc);
        })
        if (defined(selector)) {
            var _s = Selector(selector);
            ret = filterArray(ret, _s);
        }
        return $$(ret);
    },
    children: function (selector) {
        var ret = [];
        this.each(function () {
            for (var i = 0, child, len = this.children.length; i < len; i++) {
                if ((child = this.children[i]).nodeType === 1) {
                    ret.push(child);
                }
            }
        });
        if (defined(selector)) {
            var _s = Selector(selector);
            ret = filterArray(ret, _s);
        }
        return $$(ret);
    },
    append: function (node) {
        var nodes = (node instanceof $$) && node || $$(node), fragment = doc.createDocumentFragment();
        for (var i = 0, len = nodes['length']; i < len; i++) {
            fragment.appendChild(nodes[i]);
        }
        len > 0 && this.each(function () {
            this.appendChild(fragment.cloneNode(!0));
        });
        discardElement(fragment);
        return this;
    },
    prepend: function (node) {
        var nodes = (node instanceof $$) && node || $$(node), fragment = doc.createDocumentFragment();
        for (var i = 0, len = nodes['length']; i < len; i++) {
            fragment.appendChild(nodes[i]);
        }
        len > 0 && this.each(function () {
            this.insertBefore(fragment.cloneNode(!0), this.firstChild);
        });
        discardElement(fragment);
        return this;
    },
    appendTo: function (node) {
        var nodes = (node instanceof $$) && node || $$(node);
        nodes.append(this);
        return this;
    },
    after: function (node) {
        var nodes = (node instanceof $$) && node || $$(node), fragment = doc.createDocumentFragment();
        for (var i = 0, len = nodes['length']; i < len; i++) {
            fragment.appendChild(nodes[i]);
        }
        len > 0 && this.each(function () {
            this.parentNode && this.parentNode.insertBefore(fragment.cloneNode(!0), next(this));
        });
        discardElement(fragment);
        return this;
    },
    before: function (node) {
        var nodes = (node instanceof $$) && node || $$(node), fragment = doc.createDocumentFragment();
        for (var i = 0, len = nodes['length']; i < len; i++) {
            fragment.appendChild(nodes[i]);
        }
        len > 0 && this.each(function () {
            this.parentNode && this.parentNode.insertBefore(fragment.cloneNode(!0), this);
        });
        discardElement(fragment);
        return this;
    }
};
win.JSON = win.JSON || {};
win.JSON.stringify = mjson.stringify;
win.JSON.parse = mjson.parse;
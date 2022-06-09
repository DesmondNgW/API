/**
 * String.isString
 * */
String.prototype.isString = function() {
    return Object.prototype.toString.call(this) == "[object String]";
}

/**
 * Number.isNumber
 * */
Number.prototype.isNumber = function() {
    return Object.prototype.toString.call(this) == "[object Number]";
}

/**
 * Boolean.isBoolean
 * */
Boolean.prototype.isBoolean = function() {
    return Object.prototype.toString.call(this) == "[object Boolean]";
}

/**
 * Function.isFunction
 * */
Function.prototype.isFunction = function() {
    return Object.prototype.toString.call(this) == "[object Function]";
}

/**
 * Object.isNull
 * */
Object.prototype.isNull = function() {
    return Object.prototype.toString.call(this) == "[object Null]";
}

/**
 * Object.isUndefined
 * */
Object.prototype.isUndefined = function() {
    return Object.prototype.toString.call(this) == "[object Undefined]";
}

/**
 * Object.isObject
 * */
Object.prototype.isObject = function() {
    return Object.prototype.toString.call(this) == "[object Object]";
}

/**
 * Date.isDate
 * */
Date.prototype.isDate = function() {
    return Object.prototype.toString.call(this) == "[object Date]";
}

/**
 * RegExp.isRegExp
 * */
RegExp.prototype.isRegExp = function() {
    return Object.prototype.toString.call(this) == "[object RegExp]";
}
'use strict';
/**
 * Number.floor
 * @param {any} length
 */
Number.prototype.floor = function (length) {
    return Math.floor(this * Math.pow(10, length)) / Math.pow(10, length);
}

/**
 * Number.ceil
 * @param {any} length
 */
Number.prototype.ceil = function (length){
    return Math.ceil(this * Math.pow(10, length)) / Math.pow(10, length);
}

/**
 * String.isNullOrEmpty
 * */
String.prototype.isNullOrEmpty = function () {
    return !this;
}

/**
 * String.isNullOrWhiteSpace
 * */
String.prototype.isNullOrWhiteSpace = function () {
    return !(this && this.replace(/\s/ig, ""));
}

/**
 * String.getLengthMixZh{中文字符串长度}
 * */
String.prototype.getLengthMixZh = function () {
    return this.replace(new RegExp("[^\x00-\xff]", "ig"), "OK").length;
}
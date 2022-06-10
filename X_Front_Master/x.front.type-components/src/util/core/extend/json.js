"use strict";
/**
 * JSON.toJson
 * */
Object.prototype.toJson = function() {
    return JSON.stringify(this);
}

/**
 * JSON.fromJson
 * */
String.prototype.fromJson = function() {
    return JSON.parse(this);
}

/**
 * JSON.isJson
 * */
String.prototype.isJson = function() {
    let json = this;
    json = json.replace(new RegExp("\\\\(?:[\"'\\/bfnrt]|u[0-9a-fA-F]{4})", "ig"), () => "@");
    json = json.replace(new RegExp("\"[^\"\\\\n\\r]*\"|'[^\"\\\n\\r]*'|true|false|null|-?\\d+(?:\\.\\d*)?(?:[eE][+\\-]?\\d+)?", "ig"), () => "]");
    json = json.replace(new RegExp("(?:^|:|,)(?:\\s*\\[)+", "ig"), () => "");
    return new RegExp("^[\\],:{}\\s]*$", "ig").test(json);
}

/**
 * Base64.encode
 * */
String.prototype.toBase64 = function() {
    return btoa(encodeURIComponent(this));
}

/**
 * Base64.decode
 * */
String.prototype.fromBase64 = function() {
    return decodeURIComponent(atob(this));
}
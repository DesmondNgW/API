'use strict';

/**
 * Date.addDays
 * @param {any} days
 */
Date.prototype.addDays = function (days) {
    this.setDate(this.getDate() + days);
    return this;
};

/**
 * Date.addMonths
 * @param {any} months
 */
Date.prototype.addMonths = function (months) {
    this.setMonth(this.getMonth() + months);
    return this;
};

/**
 * Date.addFullYears
 * @param {any} years
 */
Date.prototype.addFullYears = function (years) {
    this.setFullYear(this.getFullYear() + years);
    return this;
};

/**
 * Date.add
 * @param {any} interval
 * @param {any} number
 */
Date.prototype.add = function (interval, number) {
    let dt = this, addMilliseconds = 0;
    switch (interval) {
        case 's':
            addMilliseconds = 1000 * number;
            break;
        case 'n':
            addMilliseconds = 60000 * number;
            break;
        case 'h':
            addMilliseconds = 3600000 * number;
            break;
        case 'd':
            addMilliseconds = 86400000 * number;
            break;
        case 'w':
            addMilliseconds = 86400000 * 7 * number;
            break;
        case 'q':
            return this.addMonths(number * 3);
        case 'm':
            return this.addMonths(number);
        case 'y':
            return this.addFullYears(number);
    }
    return new Date(+dt + addMilliseconds);
}

/**
 * Date.isLeapYear
 * */
Date.prototype.isLeapYear = function () {
    return (0 == this.getYear() % 4 && ((this.getYear() % 100 != 0) || (this.getYear() % 400 == 0)));
}

/**
 * Date.format
 * @param {any} fmt
 */
Date.prototype.format = function (fmt) {
    var o = {
        "M+": this.getMonth() + 1,
        "d+": this.getDate(),
        "H+": this.getHours(),
        "m+": this.getMinutes(),
        "s+": this.getSeconds(),
        "q+": Math.floor((this.getMonth() + 3) / 3),
        "f": this.getMilliseconds()
    };
    if (/(y+)/.test(fmt))
        fmt = fmt.replace(RegExp.$1, (this.getFullYear() + "").substr(4 - RegExp.$1.length));
    for (var k in o)
        if (new RegExp("(" + k + ")").test(fmt))
            fmt = fmt.replace(RegExp.$1, (RegExp.$1.length == 1) ? (o[k]) : (("00" + o[k]).substr(("" + o[k]).length)));
    return fmt;
};

/**
 * Date.getTimestamp
 * */
Date.prototype.getTimestamp = function () {
    return +this;
}
function Target() {}

Target.prototype.add = function(a, b) {
    return a + b;
}

Target.prototype.pow = function(a, b) {
    return Math.pow(a, b);
}

Target.prototype.sqrt = function(a) {
    return Math.sqrt(a);
}


export const target = new Target();
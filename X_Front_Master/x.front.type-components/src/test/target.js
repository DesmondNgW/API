function Target() { }

Target.prototype.add = function (a, b) {
    return a + b;
}

export const target = new Target();
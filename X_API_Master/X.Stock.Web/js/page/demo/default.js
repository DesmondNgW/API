define(function (require) {
    var $ = require("jQuery");
    return function () {
        $.fn.SetColor = function (colors, value) {
            var target = $(this);
            var t = parseFloat(target.html());
            var color = t > value ? colors[0] : t < value ? colors[2] : colors[1];
            target.removeClass(colors.join(" ")).addClass(color);
        };
        var colors = ["red", "black", "green"];
        $("#total").SetColor(colors, 100000);
        $("#benifit").SetColor(colors, 0);
    }
});
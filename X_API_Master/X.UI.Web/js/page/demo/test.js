define(function(require) {
    var $ = require("jQuery");
    var classNameMap = ["one", "two", "three", "four", "five", "six", "seven", "eight"];
    var level = {
        "easy": [9, 9, 10],
        "normal": [16, 16, 40],
        "hard": [24, 16, 99]
    };
    var extend = function(a, b) {
        a = a || {};
        for (var n in b) if (b.hasOwnProperty(n)) a[n] = b[n];
        return a;
    };
    var convert = function(options) {
        var arr = level[options.level] || level["easy"];
        options.width = arr[0];
        options.height = arr[1];
        options.error = arr[2];
        return options;
    };
    var statusTemp = "<div class='game-level'><em data-level='easy'>初级</em><em data-level='normal'>中级</em><em data-level='hard'>高级</em></div>";
    var levelTemp = "<div class='game-status fn-clear'><ul><li class='fn-left'><span class='game-tip'>数目:&nbsp;&nbsp;</span><span class='game-error'>10</span></li><li class='fn-left'><span class='game-start'>开始</span></li><li class='fn-left'><span class='game-tip'>时间:&nbsp;&nbsp;</span><span class='game-time'>0</span></li></ul></div>";
    var options = {
        node: ".main-box",
        level: "easy"
    };

    var Module = function(options) {
        if (!(this instanceof Module)) return new Module(options);
        options = convert(options);
        var width = Math.min(Math.max(9, options.width), 30),
            height = Math.min(Math.max(9, options.height), 24),
            node = $(options.node),
            arr = [statusTemp, levelTemp, "<div class='game-box fn-clear'>"];
        this.error = Math.min(Math.max(10, options.error), 668);
        this.width = width;
        this.height = height;
        this.node = node;
        this.data = [];
        this.options = options;
        for (var i = 0; i < height; i++) {
            var subArr = [];
            subArr.push("<p>");
            for(var j = 0; j < width; j++) {
                var id = "boxItem_" + i + "_" + j;
                subArr.push("<span class=\"box-item fn-left\" id=\"" + id + "\"></span>");
                if(!this.data[i]) this.data[i] = [];
                this.data[i][j] = 0;
            }
            subArr.push("</p>");
            arr.push(subArr.join(""));
        }
        arr.push("</div>");
        node.html(arr.join(""));
        $(".game-error").html(this.error);
    };

    extend(Module.prototype, {
        init: function(x, y) {
            if(!this.inited) {
                var error = this.error, arr = this.data, width = this.width, height = this.height;
                while (error) {
                    var rand = parseInt(width * height * Math.random()), w = parseInt(rand / width), h = rand - w * width;
                    if (arr[w][h] == 0 && (x != w || h != y)) {
                        error--;
                        arr[w][h] = -1;
                    }
                }
                for(var i = 0, len = arr.length; i < len; i++) {
                    for(var j= 0, jLen = arr[i].length; j < jLen; j++) {
                        if (arr[i][j] == -1) {
                            for(var k = Math.max(0, i - 1); k <= Math.min(i + 1, len - 1); k++) {
                                for(var m = Math.max(0, j - 1); m <= Math.min(j + 1, jLen - 1); m++) {
                                    if(arr[k][m] != -1) arr[k][m]++;
                                }
                            }
                        }
                    }
                }
                this.inited = true;
                this.data = arr;
            }
            return this;
        },
        find: function(i, j) {
            var arr = this.data, result = [];
            if (arr[i][j] == 0){
                for(var k = Math.max(0, i - 1); k <= Math.min(i + 1, arr.length - 1); k++) {
                    for(var m = Math.max(0, j - 1); m <= Math.min(j + 1, arr[k].length - 1); m++) {
                        if(arr[k][m] != -1) result.push(k + "#" + m);
                    }
                }
            }
            return result;
        },
        findAll: function(i, j){
            var result = [], f = this.find(i, j), ht = {}, current, arr;
            while (current = f.length && f.shift()) {
                if(!ht[current]) {
                    ht[current] = 1;
                    result.push(current);
                    arr = current.match(/^(\d+)#(\d+)$/);
                    f = f.concat(this.find(Number(arr[1]), Number(arr[2])));
                }
            }
            return result;
        },
        start: function(){
            return this.click();
        },
        reStart: function(){
            return new Module(this.options).start();
        },
        click: function(){
            var _ = this;
            $(".game-level em").on("click", function() {
                _.options.level = $(this).attr("data-level");
                clearInterval(_.IntervalId);
                _.reStart();
            });
            $(".game-start").on("click", function() {
                clearInterval(_.IntervalId);
                _.reStart();
            });
            if(!this.clicked){
                $(".box-item", this.node).on("click", function(e){
                    var target = $(e.target);
                    if(!target.hasClass("box-item-open")){
                        var position = e.target.id.split("_"), i = parseInt(position[1]), j = parseInt(position[2]);
                        if (!_.inited) {
                            _.init(i, j);
                            _.IntervalId = setInterval(function() {
                                var time = $(".game-time"), t = parseInt(time.html());
                                t++;
                                time.html(t);
                                if(t >= 999) clearInterval(_.IntervalId);
                            }, 1000);
                        }
                        var data = _.data;
                        var value = data[i][j];
                        target.addClass("box-item-open").removeClass("box-item");
                        if (value == -1){
                            _.end();
                        } else if (value > 0){
                            target.html(value).addClass(classNameMap[value - 1]);
                        } else {
                            var other = _.findAll(i, j);
                            for(var k = 0, len = other.length; k < len; k++) {
                                var arr = other[k].match(/^(\d+)#(\d+)$/);
                                $("#boxItem_" + arr[1] + "_" + arr[2], _.node).each(function(){
                                    var subTarget = $(this), i = Number(arr[1]), j = Number(arr[2]);
                                    subTarget.addClass("box-item-open").removeClass("box-item");
                                    if(data[i][j] > 0) subTarget.html(data[i][j]).addClass(classNameMap[data[i][j] - 1]);
                                });
                            }
                        }
                    }
                    if($(".box-item", _.node).length == _.error){
                        alert("You Win!");
                    }
                });
                this.clicked = true;
            }
            return this;
        },
        end: function() {
            var data = this.data;
            $(".box-item,.box-item-open", this.node).each(function(){
                var target = $(this), position = this.id.split("_"), i = parseInt(position[1]), j = parseInt(position[2]), value = data[i][j];
                target.addClass("box-item-open").removeClass("box-item");
                if (value == -1){
                    target.html("B").addClass("error");
                }else if (value > 0){
                    target.html(value).addClass(classNameMap[value - 1]);
                }
            });
            console.log("Game Over!");
            alert("Game Over!");
        }
    });
    return function()
    {
        $.ajax({
            url:"https://trade.1234567.com.cn/git.js",
            dataType:"jsonp",
            success:function(a,b,c){
                //alert(a.status)
                //alert(b)
                //alert(c)
            },
            error:function(a,b,c){
                alert(a.status)
                //alert(b)
                //alert(c)
            }
        })
        //new Module(options).start();
    }
});

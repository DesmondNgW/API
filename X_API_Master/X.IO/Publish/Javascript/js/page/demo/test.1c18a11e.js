define("/js/page/demo/test.1c18a11e",["jQuery"],function(t){var a=t("jQuery"),e=["one","two","three","four","five","six","seven","eight"],n={easy:[9,9,10],normal:[16,16,40],hard:[24,16,99]},i=function(t,a){t=t||{};for(var e in a)a.hasOwnProperty(e)&&(t[e]=a[e]);return t},r=function(t){var a=n[t.level]||n.easy;return t.width=a[0],t.height=a[1],t.error=a[2],t},s="<div class='game-level'><em data-level='easy'>初级</em><em data-level='normal'>中级</em><em data-level='hard'>高级</em></div>",l="<div class='game-status fn-clear'><ul><li class='fn-left'><span class='game-tip'>数目:&nbsp;&nbsp;</span><span class='game-error'>10</span></li><li class='fn-left'><span class='game-start'>开始</span></li><li class='fn-left'><span class='game-tip'>时间:&nbsp;&nbsp;</span><span class='game-time'>0</span></li></ul></div>",o={node:".main-box",level:"easy"},h=function(t){if(!(this instanceof h))return new h(t);t=r(t);var e=Math.min(Math.max(9,t.width),30),n=Math.min(Math.max(9,t.height),24),i=a(t.node),o=[s,l,"<div class='game-box fn-clear'>"];this.error=Math.min(Math.max(10,t.error),668),this.width=e,this.height=n,this.node=i,this.data=[],this.options=t;for(var m=0;m<n;m++){var d=[];d.push("<p>");for(var v=0;v<e;v++){var c="boxItem_"+m+"_"+v;d.push('<span class="box-item fn-left" id="'+c+'"></span>'),this.data[m]||(this.data[m]=[]),this.data[m][v]=0}d.push("</p>"),o.push(d.join(""))}o.push("</div>"),i.html(o.join("")),a(".game-error").html(this.error)};return i(h.prototype,{init:function(t,a){if(!this.inited){for(var e=this.error,n=this.data,i=this.width,r=this.height;e;){var s=parseInt(i*r*Math.random()),l=parseInt(s/i),o=s-l*i;0!=n[l][o]||t==l&&o==a||(e--,n[l][o]=-1)}for(var h=0,m=n.length;h<m;h++)for(var d=0,v=n[h].length;d<v;d++)if(n[h][d]==-1)for(var c=Math.max(0,h-1);c<=Math.min(h+1,m-1);c++)for(var f=Math.max(0,d-1);f<=Math.min(d+1,v-1);f++)n[c][f]!=-1&&n[c][f]++;this.inited=!0,this.data=n}return this},find:function(t,a){var e=this.data,n=[];if(0==e[t][a])for(var i=Math.max(0,t-1);i<=Math.min(t+1,e.length-1);i++)for(var r=Math.max(0,a-1);r<=Math.min(a+1,e[i].length-1);r++)e[i][r]!=-1&&n.push(i+"#"+r);return n},findAll:function(t,a){for(var e,n,i=[],r=this.find(t,a),s={};e=r.length&&r.shift();)s[e]||(s[e]=1,i.push(e),n=e.match(/^(\d+)#(\d+)$/),r=r.concat(this.find(Number(n[1]),Number(n[2]))));return i},start:function(){return this.click()},reStart:function(){return new h(this.options).start()},click:function(){var t=this;return a(".game-level em").on("click",function(){t.options.level=a(this).attr("data-level"),clearInterval(t.IntervalId),t.reStart()}),a(".game-start").on("click",function(){clearInterval(t.IntervalId),t.reStart()}),this.clicked||(a(".box-item",this.node).on("click",function(n){var i=a(n.target);if(!i.hasClass("box-item-open")){var r=n.target.id.split("_"),s=parseInt(r[1]),l=parseInt(r[2]);t.inited||(t.init(s,l),t.IntervalId=setInterval(function(){var e=a(".game-time"),n=parseInt(e.html());n++,e.html(n),n>=999&&clearInterval(t.IntervalId)},1e3));var o=t.data,h=o[s][l];if(i.addClass("box-item-open").removeClass("box-item"),h==-1)t.end();else if(h>0)i.html(h).addClass(e[h-1]);else for(var m=t.findAll(s,l),d=0,v=m.length;d<v;d++){var c=m[d].match(/^(\d+)#(\d+)$/);a("#boxItem_"+c[1]+"_"+c[2],t.node).each(function(){var t=a(this),n=Number(c[1]),i=Number(c[2]);t.addClass("box-item-open").removeClass("box-item"),o[n][i]>0&&t.html(o[n][i]).addClass(e[o[n][i]-1])})}}a(".box-item",t.node).length==t.error&&alert("You Win!")}),this.clicked=!0),this},end:function(){var t=this.data;a(".box-item,.box-item-open",this.node).each(function(){var n=a(this),i=this.id.split("_"),r=parseInt(i[1]),s=parseInt(i[2]),l=t[r][s];n.addClass("box-item-open").removeClass("box-item"),l==-1?n.html("B").addClass("error"):l>0&&n.html(l).addClass(e[l-1])}),console.log("Game Over!"),alert("Game Over!")}}),function(){new h(o).start()}});
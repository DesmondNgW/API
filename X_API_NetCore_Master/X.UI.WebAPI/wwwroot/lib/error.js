(function () {
	if (window.onerror) pCall = window.onerror;
	window.onerror = function (msg, url, line, col, error) {
		pCall();
		if (msg != "Script error." && !url) return true;
		col = col || (window.event && window.event.errorCharacter) || 0;
		var data = { url: url, line: line, col: col, msg: msg }, ext, f, c;
		if (!!error && !!error.stack) {
			data.msg = error.stack.toString();
		} else if (!!arguments.callee) {
			ext = [];
			f = arguments.callee.caller;
			c = 3;
			while (f && (--c > 0)) {
				ext.push(f.toString().substring(0, 30));
				if (f === f.caller) {
					break;
				}
				f = f.caller;
			}
			ext = ext.join(",");
			data.msg += ext;
		}
		setTimeout(function () {
			//把data上报到后台！
			//(function (src, charset, callback) {
			//	var script = document.createElement('script');
			//	script.charset = charset;
			//	script.type = 'text/javascript';
			//	callback = callback || Function();
			//	script.onload = script.onreadystatechange = function () {
			//		var state = script.readyState;
			//		if (!callback.done && (!state || (state === 'loaded' || state === 'complete'))) {
			//			callback.done = true;
			//			callback();
			//			script.parentNode.removeChild(script);
			//		}
			//	};
			//	script.src = src;
			//	document.getElementsByTagName('head')[0].appendChild(script);
			//}).call(this,)
		}, 0);
		return true;
	}
}).call(this)

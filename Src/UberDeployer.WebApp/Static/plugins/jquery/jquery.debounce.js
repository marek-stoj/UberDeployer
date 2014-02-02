(function ($) {
	'use strict';
	$.debounce = function (delay, fn) {
		var timer = null;
		return function () {
			var context = this, args = arguments;
			clearTimeout(timer);
			timer = setTimeout(function () {
				fn.apply(context, args);
			}, delay);
		};
	};
})(jQuery);
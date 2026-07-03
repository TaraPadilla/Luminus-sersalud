(function (w) {
    "use strict";
    w.hospWhenReady = function (fn) {
        function run() {
            if (w.jQuery) {
                try {
                    fn(w.jQuery);
                } catch (e) {
                    console.error("hospWhenReady:", e);
                }
                return;
            }
            w.setTimeout(run, 25);
        }
        run();
    };
    w.hospOnReady = function (fn) {
        w.hospWhenReady(function ($) {
            $(fn);
        });
    };
})(window);

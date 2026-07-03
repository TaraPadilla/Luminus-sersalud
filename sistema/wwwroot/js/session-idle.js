(function () {
    "use strict";

    var script = document.currentScript;
    var idleMinutes = parseInt(script && script.getAttribute("data-idle-minutes"), 10);
    if (!idleMinutes || idleMinutes <= 0) return;

    var idleMs = idleMinutes * 60 * 1000;
    var timer = null;
    var logoutUrl = "/Identity/Account/Logout";

    function resetTimer() {
        if (timer) clearTimeout(timer);
        timer = setTimeout(onIdle, idleMs);
    }

    function onIdle() {
        document.removeEventListener("mousemove", resetTimer);
        document.removeEventListener("mousedown", resetTimer);
        document.removeEventListener("keydown", resetTimer);
        document.removeEventListener("touchstart", resetTimer);
        document.removeEventListener("scroll", resetTimer);
        window.location.href = logoutUrl + "?returnUrl=" + encodeURIComponent("/Identity/Account/Login");
    }

    ["mousemove", "mousedown", "keydown", "touchstart", "scroll"].forEach(function (ev) {
        document.addEventListener(ev, resetTimer, { passive: true });
    });

    resetTimer();
})();

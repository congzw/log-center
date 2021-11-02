var common = common || {};
(function () {
    'use strict';

    String.prototype.trimRight = function (charlist) {
        if (charlist === undefined)
            charlist = "\s";
        return this.replace(new RegExp("[" + charlist + "]+$"), "");
    };

    common.global = common.global || {};
    common.global.setVirtualApp = function (currentPagePath) {
        //hack virtual path app!
        var getAbsoluteUrl = (function () {
            var a;
            return function (url) {
                if (!a) a = document.createElement('a');
                a.href = url;
                return a.href;
            }
        })();
        var path = getAbsoluteUrl(currentPagePath);
        var virtualApp = path.replace(window.location.origin + "/", '').trimRight("/");
        common.global.virtualApp = virtualApp;
        console.log("setVirtualApp:", virtualApp);
    };
}());

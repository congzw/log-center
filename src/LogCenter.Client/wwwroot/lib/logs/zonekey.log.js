var zonekey = zonekey || {};
(function () {
    'use strict';

    if (zonekey.log) {
        //防止页面重复加载
        return;
    }

    //------ log 日志组件（仿照Log4Net的思路）-------
    zonekey.log = zonekey.log || {};
    zonekey.log.levels = {
        TRACE: 0,
        DEBUG: 1,
        INFO: 2,
        WARN: 3,
        ERROR: 4,
        FATAL: 5,
        OFF: 100
    };
    //todo read and set by config => zonekey.log.level
    zonekey.log.level = zonekey.log.levels.DEBUG;
    console.log("zonekey.log.level: ", zonekey.log.level);

    zonekey.log.log = function (logObject, logLevel, prefix) {
        if (!window.console || !window.console.log) {
            return;
        }

        if (logLevel != undefined && logLevel < zonekey.log.level) {
            return;
        }

        if (typeof (prefix) === "string" && prefix !== null && prefix !== undefined) {
            console.log("[" + prefix + "]", logObject);
        } else {
            console.log(logObject);
        }
    };

    zonekey.log.trace = function (logObject, prefix) {
        zonekey.log.log(logObject, zonekey.log.levels.TRACE, prefix);
    };
    zonekey.log.debug = function (logObject, prefix) {
        //zonekey.log.log("DEBUG: ", zonekey.log.levels.DEBUG);
        zonekey.log.log(logObject, zonekey.log.levels.DEBUG, prefix);
    };
    zonekey.log.info = function (logObject, prefix) {
        //zonekey.log.log("INFO: ", zonekey.log.levels.INFO);
        zonekey.log.log(logObject, zonekey.log.levels.INFO, prefix);
    };
    zonekey.log.warn = function (logObject, prefix) {
        //zonekey.log.log("WARN: ", zonekey.log.levels.WARN);
        zonekey.log.log(logObject, zonekey.log.levels.WARN, prefix);
    };
    zonekey.log.error = function (logObject, prefix) {
        //zonekey.log.log("ERROR: ", zonekey.log.levels.ERROR);
        zonekey.log.log(logObject, zonekey.log.levels.ERROR, prefix);
    };
    zonekey.log.fatal = function (logObject, prefix) {
        //zonekey.log.log("FATAL: ", zonekey.log.levels.FATAL);
        zonekey.log.log(logObject, zonekey.log.levels.FATAL, prefix);
    };

}());


//zonekey.log.info('this is a message')

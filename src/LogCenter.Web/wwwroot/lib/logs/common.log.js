var common = common || {};
(function () {
    'use strict';

    if (common.log) {
        //防止页面重复加载
        return;
    }

    //------ log 日志组件（仿照Log4Net的思路）-------
    common.log = common.log || {};
    common.log.levels = {
        TRACE: 0,
        DEBUG: 1,
        INFO: 2,
        WARN: 3,
        ERROR: 4,
        FATAL: 5,
        OFF: 100
    };
    //todo read and set by config => common.log.level
    common.log.level = common.log.levels.DEBUG;
    console.log("common.log.level: ", common.log.level);

    common.log.log = function (logObject, logLevel, prefix) {
        if (!window.console || !window.console.log) {
            return;
        }

        if (logLevel != undefined && logLevel < common.log.level) {
            return;
        }

        if (typeof (prefix) === "string" && prefix !== null && prefix !== undefined) {
            console.log("[" + prefix + "]", logObject);
        } else {
            console.log(logObject);
        }
    };

    common.log.trace = function (logObject, prefix) {
        common.log.log(logObject, common.log.levels.TRACE, prefix);
    };
    common.log.debug = function (logObject, prefix) {
        //common.log.log("DEBUG: ", common.log.levels.DEBUG);
        common.log.log(logObject, common.log.levels.DEBUG, prefix);
    };
    common.log.info = function (logObject, prefix) {
        //common.log.log("INFO: ", common.log.levels.INFO);
        common.log.log(logObject, common.log.levels.INFO, prefix);
    };
    common.log.warn = function (logObject, prefix) {
        //common.log.log("WARN: ", common.log.levels.WARN);
        common.log.log(logObject, common.log.levels.WARN, prefix);
    };
    common.log.error = function (logObject, prefix) {
        //common.log.log("ERROR: ", common.log.levels.ERROR);
        common.log.log(logObject, common.log.levels.ERROR, prefix);
    };
    common.log.fatal = function (logObject, prefix) {
        //common.log.log("FATAL: ", common.log.levels.FATAL);
        common.log.log(logObject, common.log.levels.FATAL, prefix);
    };

}());


//common.log.info('this is a message')

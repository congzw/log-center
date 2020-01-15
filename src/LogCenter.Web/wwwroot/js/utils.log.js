var nb = nb || {};
(function () {
  'use strict';

  if (nb.log) {
    //防止页面重复加载
    return;
  }

  //------ log 日志组件（仿照Log4Net的思路）-------
  nb.log = nb.log || {};
  nb.log.levels = {
    DEBUG: 1,
    INFO: 2,
    WARN: 3,
    ERROR: 4,
    FATAL: 5,
    OFF: 100
  };
  //todo read and set by config => nb.log.level
  nb.log.level = nb.log.levels.DEBUG;
  console.log("nb.log.level: ", nb.log.level);

  nb.log.log = function (logObject, logLevel) {
    if (!window.console || !window.console.log) {
      return;
    }

    if (logLevel != undefined && logLevel < nb.log.level) {
      return;
    }
    //console.log('logLevel -> ' + logLevel);
    console.log(logObject);
  };
  nb.log.trace = function (logObject) {
      //nb.log.log("DEBUG: ", nb.log.levels.DEBUG);
      nb.log.trace(logObject, nb.log.levels.DEBUG);
  };
  nb.log.debug = function (logObject) {
    //nb.log.log("DEBUG: ", nb.log.levels.DEBUG);
    nb.log.log(logObject, nb.log.levels.DEBUG);
  };
  nb.log.info = function (logObject) {
    //nb.log.log("INFO: ", nb.log.levels.INFO);
    nb.log.log(logObject, nb.log.levels.INFO);
  };
  nb.log.warn = function (logObject) {
    //nb.log.log("WARN: ", nb.log.levels.WARN);
    nb.log.log(logObject, nb.log.levels.WARN);
  };
  nb.log.error = function (logObject) {
    //nb.log.log("ERROR: ", nb.log.levels.ERROR);
    nb.log.log(logObject, nb.log.levels.ERROR);
  };
  nb.log.fatal = function (logObject) {
    //nb.log.log("FATAL: ", nb.log.levels.FATAL);
    nb.log.log(logObject, nb.log.levels.FATAL);
  };
  
}());


//nb.log.info('this is a message')

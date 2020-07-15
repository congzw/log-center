'use strict';
function logHubHelper() {

    const ReportLog = "ReportLog";
    const ReportLogCallback = "ReportLogCallback";
    let category = "ClientJs";
    let showLevel = 0;
    let levels = {
        0: 'Trace',
        1: 'Debug',
        2: 'Info',
        3: 'Warn',
        4: 'Error',
        5: 'Fatal'
    };
    let colors = {
        0: 'gray',
        1: 'black',
        2: 'green',
        3: 'orange',
        4: 'red',
        5: 'darkred'
    };
    let _enabled = true;

    let _callerHost = null;
    let _connection = null;
    let _logger = null;
    let _oldLog = null;
    let _newLog = null;

    function colorLog(logArgs) {
        var message = logArgs.message;
        var category = logArgs.category;
        var logLevel = logArgs.level;
        if (logLevel < showLevel) {
            return;
        }

        var color = "gray";
        if (logLevel <= 5) {
            color = colors[logLevel];
        } else {
            color = "darkred";
        }

        console.log("%c" + category + ' ' + levels[logLevel] + ' => ', "color:" + color, message);
    }

    function replaceLog(connection, theLog) {

        _connection = connection;
        _logger = theLog;
        _oldLog = theLog.log;

        _newLog = function (logObject, logLevel, prefix) {

            var reportLogArgs = {
                category: category,
                message: logObject,
                level: logLevel
            };

            if (typeof (prefix) === "string" && prefix !== null && prefix !== undefined) {
                reportLogArgs.category = category + "[" + prefix + "]";
            }

            if (_callerHost) {
                reportLogArgs.category = _callerHost + " " + reportLogArgs.category;
            }

            colorLog(reportLogArgs);

            _connection.invoke(ReportLog, reportLogArgs)
                .catch(err => console.error(err));
        };

        _logger.log = _newLog;
    }

    function setShowLevel(level) {
        showLevel = level;
    }

    function setEnable(enabled) {
        _enabled = enabled;
        if (_enabled) {
            _logger.log = _newLog;
        } else {
            _logger.log = _oldLog;
        }
    }

    function init(options) {

        let hubUri = options.hubUri;
        let logger = options.logger;
        if (options.category) {
            category = options.category;
        }
        if (options.callerHost) {
            _callerHost = options.callerHost;
        }
        let connection = new signalR.HubConnectionBuilder()
            .withUrl(hubUri,
                {
                    skipNegotiation: true,
                    transport: signalR.HttpTransportType.WebSockets
                })
            .build();


        connection.start({ jsonp: true })
            .then(() => {

                //替换Log方法
                replaceLog(connection, logger);

                //监听InvokeMethod方法
                connection.on(ReportLogCallback,
                    (result) => {
                        if (result && result.success) {
                            colorLog(result.data);
                        }
                    });
            })
            .catch(err => console.error(err));
    }

    return {
        setShowLevel: setShowLevel,
        setEnable: setEnable,
        init: init
    };
};
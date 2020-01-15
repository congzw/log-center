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

    function colorLog(logArgs) {
        var message = logArgs.message;
        var category = logArgs.category;
        var logLevel = logArgs.level;
        if (logLevel < showLevel) {
            return;
        }

        var color = "gray";
        if (logLevel <= 0) {
            color = "gray";
        }
        else if (logLevel <= 1) {
            color = "black";
        }
        else if (logLevel <= 2) {
            color = "green";
        }
        else if (logLevel <= 3) {
            color = "Orange";
        }
        else if (logLevel <= 4) {
            color = "Red";
        } else {
            color = "darkred";
        }

        console.log("%c" + category + ' ' + levels[logLevel] + ' => ', "color:" + color, message);
    }

    function replaceLog(connection, theLog) {
        theLog.log = function (logObject, logLevel) {
            var reportLogArgs = {
                Category: category,
                Message: logObject,
                Level: logLevel
            };
            connection.invoke(ReportLog, reportLogArgs)
                .catch(err => console.error(err));
        }
    }

    function setShowLevel(level) {
        showLevel = level;
    }

    function init(options) {

        let hubUri = options.hubUri;
        let logger = options.logger;
        if (options.category) {
            category = options.category;
        }
        let connection = new signalR.HubConnectionBuilder()
            .withUrl(hubUri, {})
            .build();
        
        connection.start()
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
        init: init
    };
};
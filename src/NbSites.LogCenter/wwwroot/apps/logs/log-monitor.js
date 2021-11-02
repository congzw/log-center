let app = new Vue({
    el: '#settingApp',
    data: {
        setting: {
            clientId: "*",
            hubUri: "ws://" + window.location.host  + "/hubs/logHub",
            //connectHubUri : "" 增加这个属性声明，导致其被监控，备选功能将会失效！
            enabled: true,
            category: "ClientJs",
            baseApi: '/Api/Fx/Logs/LogReporter/'
        },
        connState: {
            connBtnText: "尝试连接",
            connected: false
        }
    },
    created() {
        var self = this;
        var theApiUri = self.setting.baseApi + "GetConfig";
        var theHubUri = "ws://" + window.location.host + "/hubs/logHub";
        if (common && common.global) {
            var virtualApp = common.global.virtualApp;
            if (virtualApp) {
                theApiUri = "/" + virtualApp + theApiUri;
                theHubUri = "ws://" + window.location.host + "/" + virtualApp + "/hubs/logHub";
                self.setting.getConfigUrl = theApiUri;
                self.setting.hubUri = theHubUri;
            }
        }
        console.log('getConfigUrl', theApiUri);
        console.log('hubUri', theHubUri);
        $.getJSON(theApiUri, function (data) {
            console.log('created getApiSetting', data);
            if (data) {
                if (data.clientId) {
                    self.setting.clientId = data.clientId;
                }
                if (data.hubUri) {
                    self.setting.hubUri = data.hubUri;
                }
            }
        });
    },
    methods: {
        initLogMonitor: function () {
            console.log('尝试连接:', this.setting.connectHubUri);

            let theLog = common.log;
            let theHelper = logHubHelper();
            let initCfg = {
                logger: theLog,
                clientId: this.setting.clientId,
                hubUri: this.setting.connectHubUri,
                category: this.setting.category,
                enabled: true
            };
            let connState = this.connState;

            theHelper.init(initCfg, function (success) {
                connState.connected = success;
                connState.connBtnText = success ? "已连接" : "连接失败";
            });


            //demo: filter logs
            $('#showLevel').on('change',
                function (e) {
                    var optionSelected = $("option:selected", this);
                    var showLevel = optionSelected.val();
                    theHelper.setShowLevel(showLevel);
                });

            //demo: send remote logs!
            $("#btnReportLog").click(function () {

                let date = new Date();
                var msg = date.getHours() + ':' + date.getMinutes() + ':' + date.getSeconds() + ' ' + date.getMilliseconds();
                theLog.trace(msg, "foo trace");
                theLog.debug(msg, "foo debug");
                theLog.info(msg);
                theLog.warn(msg, "bar");
                theLog.error(msg);
                theLog.fatal(msg);
            });
        }
    },
    computed: {
        hubUri: {
            // getter
            get: function () {
                this.setting.connectHubUri = this.setting.hubUri + "?LogMonitor=true&ClientId=" + this.setting.clientId;
                return this.setting.connectHubUri;
            },
            // setter
            set: function (newValue) {
                this.setting.connectHubUri = newValue;
            }
        }
    }
});
let app = new Vue({
    el: '#theApp',
    data: {
        setting: {
            clientId: "foo",
            hubUri: "ws://" + window.location.host  + "/hubs/test-hub",
            enabled: true,
            category: "ClientJs"
        },
        connState: {
            connBtnText: "尝试连接",
            connected: false
        }
    },
    created() {
        var self = this;
        var theHubUri = "ws://" + window.location.host + "/hubs/test-hub";
        if (common && common.global) {
            var virtualApp = common.global.virtualApp;
            if (virtualApp) {
                theHubUri = "ws://" + window.location.host + "/" + virtualApp + "/hubs/test-hub";
                self.setting.hubUri = theHubUri;
            }
        }
        console.log('hubUri', theHubUri);
    },
    methods: {
        initConnect: function () {
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
        }
    },
    computed: {
        hubUri: {
            // getter
            get: function () {
                this.setting.connectHubUri = this.setting.hubUri + "?ClientId=" + this.setting.clientId;
                return this.setting.connectHubUri;
            },
            // setter
            set: function (newValue) {
                this.setting.connectHubUri = newValue;
            }
        }
    }
});
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace LogCenter.Client
{
    //all method use http get for simple use with browser
    [Route("Api/LogCenter/Client/RemoteSetting")]
    public class RemoteHubReporterSettingApi : ControllerBase
    {
        /// <summary>
        /// 获取配置
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetConfig")]
        public RemoteHubReporterConfig GetConfig()
        {
            var remoteHubReporter = RemoteHubReporter.Instance;
            return remoteHubReporter.Config;
        }

        /// <summary>
        /// 设置是否启用
        /// </summary>
        /// <param name="enabled"></param>
        /// <returns></returns>
        [HttpGet("SetEnabled")]
        public RemoteHubReporterConfig SetEnabled(bool enabled)
        {
            var remoteHubReporter = RemoteHubReporter.Instance;
            remoteHubReporter.Config.Enabled = enabled;
            return remoteHubReporter.Config;
        }
        
        /// <summary>
        /// 重置为配置文件里的默认值
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        [HttpGet("ResetConfig")]
        public async Task<RemoteHubReporterConfig> ResetConfig([FromServices] RemoteHubReporterConfig config)
        {
            var remoteHubReporter = RemoteHubReporter.Instance;
            await remoteHubReporter.Init(config);
            return config;
        }
    }
}

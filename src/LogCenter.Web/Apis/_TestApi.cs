using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace LogCenter.Web.Apis
{
    [Route("Api/Test")]
    public class TestApi : BaseLogCenterApi
    {
        private readonly ILogger _logger;

        public TestApi(ILogger logger)
        {
            _logger = logger;
        }

        [HttpGet("GetLog")]
        public string GetLog()
        {
            var msg ="Log FROM " + this.GetType().Name + " " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            _logger.LogWarning(msg);
            return msg;
        }
    }
}

using System;
using LogCenter.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace LogCenter.Web.Apis
{
    [Route("Api/Test")]
    public class TestApi : BaseLogCenterApi
    {
        private readonly ILogger _logger;
        private readonly ILogger<TestApi> _testApiLog;

        public TestApi(ILogger logger, ILogger<TestApi> testApiLog)
        {
            _logger = logger;
            _testApiLog = testApiLog;
        }

        [HttpGet("GetLog")]
        public string GetLog()
        {
            //Level Typical Use

            //Fatal   Something bad happened; application is going down
            //Error Something failed; application may or may not continue
            //Warn Something unexpected; application will continue
            //Info Normal behavior like mail sent, user updated profile etc.
            //Debug For debugging; executed query, user authenticated, session expired
            //Trace For trace debugging; begin method X, end method X

            var msg = "Log FROM " + this.GetType().Name + " " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            _logger.LogWarning(">> " + msg);
            _testApiLog.LogWarning(">> " + msg);

            var values = Enum.GetValues(typeof(LogLevel));
            foreach (int value in values)
            {
                var logHelper = LogHelper.Instance;
                logHelper.Log(msg, value);
            }
            return msg;
        }
    }
}

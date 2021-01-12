using System;
using System.Collections.Generic;
using Common.Logs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace LogCenter.Web.Apis
{
    [Route("Api/Test")]
    public class TestApi : BaseLogCenterApi
    {
        private readonly ILogger<TestApi> _testApiLog;

        public TestApi(ILogger<TestApi> testApiLog)
        {
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

            var logHelper = LogHelper.Instance;
            _testApiLog.LogTrace("_testApiLog.LogTrace");
            _testApiLog.LogInformation("_testApiLog.LogInformation");

            var loggerHelperMsg = "FROM ILogHelper: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            var values = Enum.GetValues(typeof(LogLevel));
            var results = new List<string>();
            foreach (LogLevel value in values)
            {
                var intValue = (int) value;
                var nLevel = NLog.LogLevel.FromOrdinal(intValue);
                var desc = $"{value}({intValue}) <=> NLog:{nLevel}({intValue})";
                var msg = $"{loggerHelperMsg} => {desc}";
                results.Add($" {desc}");
                logHelper.Log(msg, value);
            }
            return "OK" + string.Join(',', results);
        }
    }
}

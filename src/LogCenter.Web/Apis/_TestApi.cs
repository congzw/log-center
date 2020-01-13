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

        public TestApi(ILogger logger)
        {
            _logger = logger;
        }

        [HttpGet("GetLog")]
        public string GetLog()
        {
            var serviceLocator = ServiceLocator.Current;
            var service = serviceLocator.GetService<ILogger>();

            //Level Typical Use

            //Fatal   Something bad happened; application is going down
            //Error Something failed; application may or may not continue
            //Warn Something unexpected; application will continue
            //Info Normal behavior like mail sent, user updated profile etc.
            //Debug For debugging; executed query, user authenticated, session expired
            //Trace For trace debugging; begin method X, end method X

            var msg = "Log FROM " + this.GetType().Name + " " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            _logger.LogWarning(msg);

            var values = Enum.GetValues(typeof(LogLevel));
            foreach (int value in values)
            {
                //var level = (LogLevel)(value);
                //LogHelper.Debug(msg + " " + level);
                
                var logHelper = LogHelper.Instance;
                logHelper.Log(msg, value);

                //var simpleLog = logHelper.GetLogger();
                //simpleLog.Log(msg + " " + level, (SimpleLogLevel)value);
            }

            return msg;
        }
    }
}

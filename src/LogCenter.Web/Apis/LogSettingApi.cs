//using Microsoft.AspNetCore.Mvc;
//using NLog;

//namespace LogCenter.Web.Apis
//{
//    //todo: runtime change LogManager.Configuration.Variables["logDirectory"] not work!
//    [Route("Api/LogSetting")]
//    public class LogSettingApi : BaseLogCenterApi
//    {
//        [HttpGet("Get")]
//        public LogSetting GetLog()
//        {
//            var variable = LogManager.Configuration.Variables["logDirectory"];
//            if (variable != null)
//            {
//                Setting.LogDirectory = variable.ToString();
//            }
//            return Setting;
//        }

//        [HttpGet("Set")]
//        public string SetLog([FromQuery]LogSetting setting)
//        {
//            if (string.IsNullOrWhiteSpace(setting.Category))
//            {
//                Setting.Category = string.Empty;
//            }
//            else
//            {
//                Setting.Category = setting.Category.Trim();
//            }
//            var newPath = "wwwroot/logs/" + Setting.Category;
//            newPath = newPath.TrimEnd('/') + "/";
//            LogManager.Configuration.Variables["logDirectory"] = newPath;
//            Setting.LogDirectory = newPath;
//            return "OK";
//        }


//        public static LogSetting Setting  = new LogSetting();
//    }

//    public class LogSetting
//    {
//        public string Category { get; set; } = string.Empty;

//        public string LogDirectory { get; set; }
//    }
//}

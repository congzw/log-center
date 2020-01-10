using System;
using Microsoft.AspNetCore.Mvc;

namespace LogCenter.Web.Apis
{
    public interface ILogCenterApi
    {
    }
    
    [ApiController]
    public abstract class BaseLogCenterApi : ILogCenterApi
    {
        [HttpGet("GetDesc")]
        public string GetDesc()
        {
            return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " FROM " + this.GetType().Name;
        }
    }
}

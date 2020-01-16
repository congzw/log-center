using System;
using Microsoft.AspNetCore.Mvc;

namespace LogCenter.Client.Apis
{
    public interface ILogClientApi
    {
    }
    
    [ApiController]
    public abstract class BaseLogClientApi : ILogClientApi
    {
        [HttpGet("GetDesc")]
        public string GetDesc()
        {
            return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " FROM " + this.GetType().Name;
        }
    }
}

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace Common
{
    public static partial class Extensions
    {
        public static Task<HttpResponseMessage> PostAsJsonAsync<T>(this HttpClient httpClient, string url, T data, Encoding encoding = null, string mediaType = "application/json")
        {
            var json = JsonConvert.SerializeObject(data);
            var content = new StringContent(json, encoding, mediaType);
            return httpClient.PostAsync(url, content);
        }

        public static async Task<T> ReadAsJsonAsync<T>(this HttpContent content)
        {
            var response = await content.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(response))
            {
                return default(T);
            }

            if (typeof(T) == typeof(string))
            {
                return (dynamic)response;
            }

            //return JsonConvert.DeserializeObject(response);
            return JsonConvert.DeserializeObject<T>(response);
        }


        /// <summary>
        /// 尝试返回找到的第一个匹配，如果都没有找到，返回最后一个
        /// </summary>
        /// <param name="httpContext"></param>
        /// <param name="names"></param>
        /// <returns></returns>
        public static string TryFindFirstMatchQueryParameter(this HttpContext httpContext, params string[] names)
        {
            if (names == null) throw new ArgumentNullException(nameof(names));
            if (names.Length == 0 )
            {
                throw new ArgumentException("至少要包含一个以上元素");
            }

            if (httpContext?.Request == null)
            {
                return null;
            }
            var query = httpContext.Request.Query;

            foreach (var name in names)
            {
                if (query.ContainsKey(name))
                {
                    return name;
                }
            }
            return names.Last();
        }
        
        public static IDictionary<string, string> TryFindItemsFromQueryOrClaims(this HttpContext httpContext)
        {
            var result = new ConcurrentDictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            if (httpContext == null)
            {
                return result;
            }

            var user = httpContext.User;
            foreach (var userClaim in user.Claims)
            {
                result[userClaim.Type] = userClaim.Value;
            }

            //query override claims
            var query = httpContext.Request.Query;
            if (query != null)
            {
                var dictionary = query.ToDictionary(x => x.Key, x => x.Value);
                foreach (var item in dictionary)
                {
                    result[item.Key] = item.Value;
                }
            }

            return result;
        }
        
        //从参数或claims中寻找，并忽略大小写差异
        public static string TryFindByQueryOrClaims(this HttpContext httpContext, string key, string defaultValue = "")
        {
            var user = httpContext.User;
            var theValue = httpContext.TryGetQueryParameterValue(key, defaultValue);
            if (string.IsNullOrWhiteSpace(theValue))
            {
                theValue = user.FindFirst(x => x.Type.Equals(key, StringComparison.OrdinalIgnoreCase))?.Value;
            }
            return theValue;
        }
        
        public static T TryGetQueryParameterValue<T>(this HttpContext httpContext, string queryParameterName, T defaultValue = default(T))
        {
            if (httpContext == null)
            {
                return defaultValue;
            }

            if (httpContext.Request == null)
            {
                return defaultValue;
            }

            var query = httpContext.Request.Query;
            if (query == null)
            {
                return defaultValue;
            }
            return TryGetQueryParameterValue<T>(query, queryParameterName, defaultValue);
        }

        public static T TryGetQueryParameterValue<T>(this IQueryCollection httpQuery, string queryParameterName, T defaultValue = default(T))
        {
            try
            {
                return httpQuery.TryGetValue(queryParameterName, out var value) && value.Any()
                    ? (T)Convert.ChangeType(value.FirstOrDefault(), typeof(T))
                    : defaultValue;
            }
            catch (Exception)
            {
                return defaultValue;
            }
        }

        public static void AddHttpContextAccessorIf(this IServiceCollection services)
        {
            //Add only not registered
            var httpContextAccessorDescriptor = services.LastOrDefault(d => d.ServiceType == typeof(IHttpContextAccessor));
            if (httpContextAccessorDescriptor == null)
            {
                services.AddHttpContextAccessor();
            }
        }
        

        public static string GetCurrentEndPointId(this HttpContext httpContext)
        {
            var actionId = httpContext.GetCurrentActionId();
            if (!string.IsNullOrWhiteSpace(actionId))
            {
                return actionId;
            }

            var pageId = httpContext.GetCurrentPageId();
            if (!string.IsNullOrWhiteSpace(pageId))
            {
                return pageId;
            }

            return string.Empty;
        }

        public static ActionDescriptor GetControllerActionDescriptor(this HttpContext httpContext)
        {
            var endpoint = httpContext.GetEndpoint();
            return endpoint?.Metadata.GetMetadata<ControllerActionDescriptor>();
        }

        internal static string GetRouteTemplate(this HttpContext httpContext)
        {
            var actionDescriptor = httpContext.GetControllerActionDescriptor();
            var routeTemplate = actionDescriptor?.AttributeRouteInfo?.Template;
            return routeTemplate;
        }

        internal static string GetCurrentActionId(this HttpContext httpContext)
        {
            var actionDescriptor = httpContext.GetControllerActionDescriptor();
            //JwtAndCookie.Controllers.DemoController.Fallback (JwtAndCookie) => JwtAndCookie.Controllers.DemoController.Fallback
            var actionId = actionDescriptor?.DisplayName?.Split().FirstOrDefault();
            return actionId;
        }

        internal static string GetCurrentPageId(this HttpContext httpContext)
        {
            //routeData.Values.Select(x => $"{x.Key}={x.Value}").MyJoin();
            //Page  => "page=/DemoAuth/Index,area=NbSites.App.Dev"
            var routeData = httpContext.GetRouteData();
            var hasPage = routeData.Values.TryGetValue("page", out var page);
            if (!hasPage)
            {
                return null;
            }
            var hasArea = routeData.Values.TryGetValue("area", out var area);

            if (hasArea)
            {
                //=> "NbSites.App.Dev/DemoAuth/Index"
                return $"{area}{page}";
            }
            
            //=> "/DemoAuth/Index"
            return $"{page}";
        }
    }
}

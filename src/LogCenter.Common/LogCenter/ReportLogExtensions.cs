using System;
using System.Linq;
using Microsoft.AspNetCore.Http;

namespace LogCenter
{
    public static class ReportLogExtensions
    {
        public static ReportLogResult Validate(this ReportLogArgs args)
        {
            var validate = ReportLogArgs.Validate(args, out var message);
            return ReportLogResult.Create(validate, message, args);
        }

        public static T TryGetQueryParameterValue<T>(this IQueryCollection httpQuery, string queryParameterName, T defaultValue = default(T))
        {
            return httpQuery.TryGetValue(queryParameterName, out var value) && value.Any()
                ? (T)Convert.ChangeType(value.FirstOrDefault(), typeof(T))
                : defaultValue;
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
    }
}
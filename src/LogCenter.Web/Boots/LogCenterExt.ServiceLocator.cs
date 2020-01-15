using System;
using System.Collections.Generic;
using LogCenter.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace LogCenter.Web.Boots
{
    //how to use:
    //1 setup => services.AddSingleton<IServiceLocator, HttpRequestServiceLocator>();
    //2 use => ServiceLocator.Initialize(app.ApplicationServices.GetService<IServiceLocator>());
    public class HttpRequestServiceLocator : IServiceLocator
    {
        private readonly IServiceProvider _serviceProvider;

        public HttpRequestServiceLocator(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public T GetService<T>()
        {
            var contextAccessor = _serviceProvider.GetService<IHttpContextAccessor>();
            var httpContext = contextAccessor.HttpContext;
            if (httpContext == null)
            {
                return _serviceProvider.GetService<T>();
            }
            return contextAccessor.HttpContext.RequestServices.GetService<T>();
        }

        public IEnumerable<T> GetServices<T>()
        {
            var contextAccessor = _serviceProvider.GetService<IHttpContextAccessor>();
            var httpContext = contextAccessor.HttpContext;
            if (httpContext == null)
            {
                return _serviceProvider.GetServices<T>();
            }
            return contextAccessor.HttpContext.RequestServices.GetServices<T>();
        }

        public object GetService(Type type)
        {
            var contextAccessor = _serviceProvider.GetService<IHttpContextAccessor>();
            var httpContext = contextAccessor.HttpContext;
            if (httpContext == null)
            {
                return _serviceProvider.GetService(type);
            }
            return contextAccessor.HttpContext.RequestServices.GetService(type);
        }

        public IEnumerable<object> GetServices(Type type)
        {
            var contextAccessor = _serviceProvider.GetService<IHttpContextAccessor>();
            var httpContext = contextAccessor.HttpContext;
            if (httpContext == null)
            {
                return _serviceProvider.GetServices(type);
            }
            return contextAccessor.HttpContext.RequestServices.GetServices(type);
        }
    }
}

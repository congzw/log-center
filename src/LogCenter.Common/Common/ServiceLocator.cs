using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Common
{
    public interface IServiceLocator
    {
        T GetService<T>();
        IEnumerable<T> GetServices<T>();
        object GetService(Type type);
        IEnumerable<object> GetServices(Type type);
    }

    public class NullServiceLocator : IServiceLocator
    {
        public T GetService<T>()
        {
            return default(T);
        }

        public IEnumerable<T> GetServices<T>()
        {
            return Enumerable.Empty<T>();
        }

        public object GetService(Type type)
        {
            return null;
        }

        public IEnumerable<object> GetServices(Type type)
        {
            return Enumerable.Empty<object>();
        }
    }

    //only for static inject and old history code hacking!
    //ServiceLocator is an anti-pattern, avoid using it as possible as you can!
    public static class ServiceLocator
    {
        private static IServiceLocator diProxy;

        private static readonly IServiceLocator _nullLocator = new NullServiceLocator();

        public static IServiceLocator Current => diProxy ?? _nullLocator;

        public static void Initialize(IServiceLocator proxy)
        {
            if (diProxy != null)
            {
                throw new InvalidOperationException("不能被重复初始化！");
            }
            diProxy = proxy;
        }

        public static void Reset()
        {
            diProxy = null;
        }
    }

    #region http adapter
    
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

    #endregion
}

using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Common
{
    public class RootServiceProvider
    {
        public IServiceProvider Root;

        /// <summary>
        /// 自动获取： 当前HttpContext.RequestServices || RootServiceProvider
        /// </summary>
        /// <returns></returns>
        public IServiceProvider GetCurrentServiceProvider()
        {
            var root = this.Root;
            if (root == null)
            {
                return null;
            }

            var currentRequestProvider = root?.GetService<IHttpContextAccessor>()?.HttpContext?.RequestServices;
            if (currentRequestProvider != null)
            {
                return currentRequestProvider;
            }

            return root.GetService<IServiceProvider>();
        }

        /// <summary>
        /// 获取服务，首先尝试从DI系统查找，没有找到则返回默认
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="defaultFactory"></param>
        /// <returns></returns>
        public T TryGetService<T>(Func<T> defaultFactory = null)
        {
            var serviceProvider = this.GetCurrentServiceProvider();
            if (serviceProvider == null)
            {
                return defaultFactory == null ? default : defaultFactory();
            }

            var service = serviceProvider.GetService<T>();
            if (service == null)
            {
                return defaultFactory == null ? default : defaultFactory();
            }

            return service;
        }

        /// <summary>
        /// 获取服务列表，首先尝试从DI系统查找，没有找到则返回长度为0的集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="defaultFactory"></param>
        /// <returns></returns>
        public IEnumerable<T> TryGetServices<T>(Func<IEnumerable<T>> defaultFactory = null)
        {
            var serviceProvider = this.GetCurrentServiceProvider();
            if (serviceProvider == null)
            {
                return defaultFactory == null ? new List<T>() : defaultFactory();
            }

            var services = serviceProvider.GetServices<T>();
            return services;
        }

        private RootServiceProvider() {}
        public static RootServiceProvider Instance = new RootServiceProvider();
    }
}

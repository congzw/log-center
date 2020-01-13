using System;
using System.Collections.Generic;
using System.Linq;

namespace LogCenter.Common
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
}

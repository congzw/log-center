using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Common.Utilities
{
    public class LazySingleton
    {
        #region used as singleton

        public static LazySingleton Instance = new LazySingleton();

        #endregion

        #region helper class

        class LazyValueFactory<T>
        {
            private Lazy<T> _lazy;
            public Lazy<T> GetResolve(Func<T> valueFactory)
            {
                if (valueFactory == null) throw new ArgumentNullException(nameof(valueFactory));
                _lazy ??= new Lazy<T>(valueFactory);
                return _lazy;
            }
            public static readonly LazyValueFactory<T> Default = new LazyValueFactory<T>();
        }

        #endregion

        #region helper method

        public IServiceProvider RootServiceProvider { get; set; }

        internal Func<T> GetResolve<T>(Func<T> defaultFactory)
        {
            //try from root first!
            if (RootServiceProvider != null)
            {
                var theService = RootServiceProvider.GetService<T>();
                if (theService != null)
                {
                    return () => theService;
                }
            }

            if (defaultFactory == null) throw new ArgumentNullException(nameof(defaultFactory));
            Resolves.TryGetValue(typeof(T), out var result);
            if (result != null)
            {
                return (Func<T>)result;
            }

            Func<T> theFunc = () => LazyValueFactory<T>.Default.GetResolve(defaultFactory).Value;
            Resolves[typeof(T)] = theFunc;
            return theFunc;
        }
        //[type, Func<T>]
        internal IDictionary<Type, object> Resolves { get; set; } = new ConcurrentDictionary<Type, object>();

        #endregion

        #region how to use

        //public class LazyResolveDemo
        //{
        //    public string Id { get; set; } = Guid.NewGuid().ToString();

        //    [LazySingleton]
        //    public static LazyResolveDemo Whatever => LazySingleton.Instance.Resolve<LazyResolveDemo>();
        //}

        #endregion

        public T Resolve<T>(Func<T> defaultFactory)
        {
            return GetResolve(defaultFactory).Invoke();
        }

        public List<LazySingletonItem> GetLazySingletonItems(params Assembly[] assemblies)
        {
            var members = LazySingletonAttribute.GetLazySingletonMembers(assemblies).ToArray();
            var items = LazySingletonAttribute.InvokeLazySingletonMembers(members);
            return items;
        }
    }

    [AttributeUsage(AttributeTargets.All)]
    public class LazySingletonAttribute : Attribute
    {
        public static List<LazySingletonItem> InvokeLazySingletonMembers(params MemberInfo[] members)
        {
            //反射找到所有的属性或方法，调用得到实例
            var items = new List<LazySingletonItem>();
            foreach (var member in members)
            {
                var instance = InvokeLazySingletonMember(member);
                items.Add(instance);
            }
            return items;
        }
        public static LazySingletonItem InvokeLazySingletonMember(MemberInfo member)
        {
            if (member == null) throw new ArgumentNullException(nameof(member));

            var item = new LazySingletonItem();
            if (member is PropertyInfo prop)
            {
                item.Type = prop.PropertyType;
                item.Value = prop.GetValue(null, null);
                return item;
            }

            if (member is MethodInfo method)
            {
                item.Type = method.ReturnType;
                item.Value = method.Invoke(null, null);
                return item;
            }
            return null;
        }
        public static IEnumerable<MemberInfo> GetLazySingletonMembers(params Assembly[] assemblies)
        {
            var members = assemblies
                .SelectMany(x => x.GetExportedTypes())
                .SelectMany(x => x.GetMembers())
                .Where(x => x.GetCustomAttributes(typeof(LazySingletonAttribute), true).Length > 0);
            return members;
        }
        public static IEnumerable<MemberInfo> GetLazySingletonMembers(Type theType)
        {
            var members = theType
                .GetMembers(BindingFlags.Static | BindingFlags.Public)
                //.GetMembers()
                .Where(x => x.GetCustomAttributes(typeof(LazySingletonAttribute), true).Length > 0);
            return members;
        }
    }

    public class LazySingletonItem
    {
        public Type Type { get; set; }
        public object Value { get; set; }
    }
}

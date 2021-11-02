using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;

namespace Common.Utilities
{
    public interface IBagsHelper
    {
        /// <summary>
        /// 创建Bags字典
        /// </summary>
        /// <returns></returns>
        IDictionary<string, object> Create();

        /// <summary>
        /// 自动检测Bags属性：可能有，也可能没有
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="bagsName"></param>
        /// <returns></returns>
        IDictionary<string, object> TryGuessBags(object instance, string bagsName = null);
    }

    public class BagsHelper : IBagsHelper
    {
        #region for di extensions

        [LazySingleton]
        public static IBagsHelper Instance => LazySingleton.Instance.Resolve(() => new BagsHelper());

        #endregion

        public IDictionary<string, object> Create()
        {
            return new ConcurrentDictionary<string, object>(StringComparer.OrdinalIgnoreCase);
        }
        
        public static string GetTheBagsPropertyName = "GetTheBagsPropertyName";
        public IDictionary<string, object> TryGuessBags(object instance, string bagsName = null)
        {
            var theType = instance.GetType();
            var theBagsName = GetTheBagsName(instance, bagsName);
            if (!string.IsNullOrWhiteSpace(theBagsName))
            {
                var propInfo = theType.GetProperty(theBagsName ?? string.Empty, BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase);
                if (propInfo == null)
                {
                    return null;
                }
                return propInfo.GetValue(instance, null) as IDictionary<string, object>;
            }
            
            //没有定义方法: GetBagsPropertyName()
            if (instance is IHaveBags haveBags)
            {
                return haveBags.Bags;
            }
            
            var propInfo2 = theType.GetProperty("Bags", BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase);
            if (propInfo2 != null)
            {
                return propInfo2.GetValue(instance, null) as IDictionary<string, object>;
            }

            return null;
        }
        private static string GetTheBagsName(object instance, string bagsName)
        {
            if (!string.IsNullOrWhiteSpace(bagsName))
            {
                return bagsName;
            }
            
            var theType = instance.GetType();
            var methodInfo = theType.GetMethod(GetTheBagsPropertyName, BindingFlags.Instance |BindingFlags.Static | BindingFlags.Public | BindingFlags.IgnoreCase);
            //有定义方法: GetBagsPropertyName()
            if (methodInfo != null)
            {
                if (methodInfo.IsStatic)
                {
                    return methodInfo.Invoke(null, null) as string;
                }
                return methodInfo.Invoke(instance, null) as string;
            }
            
            //没有定义方法: GetBagsPropertyName()
            return null;
        }
    }
    
    public interface IHaveBags
    {
        IDictionary<string, object> Bags { get; set; }
    }

    public interface IHaveTheBagsProperty
    {
        string GetTheBagsPropertyName();
    }

    public static class HaveBagsExtensions
    {
        public static IDictionary<string, object> TryGuessBags<T>(this T instance, string bagsPropertyName = null)
        {
            return BagsHelper.Instance.TryGuessBags(instance, bagsPropertyName);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Common.Utilities;
using Microsoft.CSharp.RuntimeBinder;

namespace Common
{
    public static partial class Extensions
    {
        /// <summary>
        /// 拷贝到
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="copyFrom"></param>
        /// <param name="copyTo"></param>
        /// <param name="excludeProperties"></param>
        /// <returns></returns>
        public static T TryCopyTo<T>(this object copyFrom, T copyTo, string[] excludeProperties = null) 
        {
            ModelHelper.Instance.TryCopyProperties(copyTo, copyFrom, excludeProperties);
            return copyTo;
        }

        /// <summary>
        /// 映射为对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="copyFrom"></param>
        /// <param name="valueIfNull"></param>
        /// <param name="excludeProperties"></param>
        /// <returns></returns>
        public static T TryMapTo<T>(this object copyFrom, T valueIfNull = default, string[] excludeProperties = null) where T : new()
        {
            if (copyFrom == null)
            {
                return valueIfNull;
            }

            var copyTo = new T();
            copyFrom.TryCopyTo(copyTo, excludeProperties);
            return copyTo;
        }

        /// <summary>
        /// 映射为对象集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="copyFromItems"></param>
        /// <param name="autoTrimNull"></param>
        /// <param name="excludeProperties"></param>
        /// <returns></returns>
        public static IEnumerable<T> TryMapTo<T>(this IEnumerable<object> copyFromItems, bool autoTrimNull = true, string[] excludeProperties = null) where T : new()
        {
            foreach (var copyFromItem in copyFromItems)
            {
                if (copyFromItem == null && autoTrimNull)
                {
                    continue;
                }
                yield return copyFromItem.TryMapTo<T>(default, excludeProperties);
            }
        }

        /// <summary>
        /// 动态调用成员（属性，字段）
        /// </summary>
        /// <param name="target"></param>
        /// <param name="member"></param>
        /// <returns></returns>
        public static dynamic GetMember(this object target, string member)
        {
            return GetPropertyAsDynamic(target, member);
        }

        /// <summary>
        /// 动态调用成员（属性，字段）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="target"></param>
        /// <param name="member"></param>
        /// <returns></returns>
        public static T GetMemberAs<T>(this object target, string member)
        {
            return (T)target.GetMember(member);
        }
        
        //support dynamic member invoke: 
        //dynamicFoo["bar"]会抛出异常
        private static dynamic GetPropertyAsDynamic(object target, string name)
        {
            var site = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(0, name, target.GetType(), new[] { CSharpArgumentInfo.Create(0, null) }));
            return site.Target(site, target);
        }
    }
}

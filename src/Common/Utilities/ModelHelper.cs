using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;

namespace Common.Utilities
{
    public class ModelHelper
    {
        #region for di extensions

        [LazySingleton]
        public static ModelHelper Instance => LazySingleton.Instance.Resolve(() => new ModelHelper());

        #endregion

        public void TryCopyProperties(object updatingObj, object collectedObj, string[] excludeProperties = null)
        {
            if (collectedObj != null && updatingObj != null)
            {
                //获取类型信息
                var updatingObjType = updatingObj.GetType();
                var updatingObjPropertyInfos = updatingObjType.GetProperties();

                var collectedObjType = collectedObj.GetType();
                var collectedObjPropertyInfos = collectedObjType.GetProperties();

                var fixedExProperties = excludeProperties ?? new string[] { };

                foreach (var updatingObjPropertyInfo in updatingObjPropertyInfos)
                {
                    foreach (var collectedObjPropertyInfo in collectedObjPropertyInfos)
                    {
                        if (updatingObjPropertyInfo.Name.Equals(collectedObjPropertyInfo.Name, StringComparison.OrdinalIgnoreCase))
                        {
                            if (fixedExProperties.Contains(updatingObjPropertyInfo.Name, StringComparer.OrdinalIgnoreCase))
                            {
                                continue;
                            }

                            //do not process complex property
                            var isSimpleType = IsSimpleType(collectedObjPropertyInfo.PropertyType);
                            if (!isSimpleType)
                            {
                                continue;
                            }

                            //fix dynamic problems: System.Reflection.TargetParameterCountException
                            var declaringType = collectedObjPropertyInfo.DeclaringType;
                            if (declaringType != null && declaringType != collectedObjType)
                            {
                                //do not process base class dynamic property
                                if (NotProcessPropertyBaseTypes.Contains(declaringType))
                                {
                                    continue;
                                }
                            }

                            var value = collectedObjPropertyInfo.GetValue(collectedObj, null);
                            if (updatingObjPropertyInfo.CanWrite)
                            {
                                //do not process read only property
                                updatingObjPropertyInfo.SetValue(updatingObj, value, null);
                            }
                            break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 是否是简单类型
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public bool IsSimpleType(Type type)
        {
            var typeInfo = type.GetTypeInfo();
            if (typeInfo.IsGenericType && typeInfo.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                // nullable type, check if the nested type is simple.
                return IsSimpleType(typeInfo.GetGenericArguments()[0]);
            }
            return typeInfo.IsPrimitive
                   || typeInfo.IsEnum
                   || type == typeof(string)
                   || type == typeof(decimal)
                   || type.IsSubclassOf(typeof(ValueType)); //Guid, Datetime, etc...
        }


        private IList<Type> _notProcessPropertyBaseTypes = new List<Type>
        {
            typeof (DynamicObject),
            typeof (object),
            //typeof (BaseViewModel),
            //typeof (BaseViewModel<>),
            //typeof (Expando)
        };
        /// <summary>
        /// 在这些类型中声明的属性不处理
        /// </summary>
        public IList<Type> NotProcessPropertyBaseTypes
        {
            get => _notProcessPropertyBaseTypes;
            set => _notProcessPropertyBaseTypes = value ?? throw new ArgumentNullException(nameof(value));
        }
    }
}

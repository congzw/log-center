using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Common
{
    public static partial class Extensions
    {
        public static string GetAssemblyShortName(this Assembly assembly)
        {
            return assembly.FullName?.Split(",").FirstOrDefault();
        }

        public static List<string> GetAssemblyShortNames(this IEnumerable<Assembly> assemblies)
        {
            return assemblies.Select(x => x.GetAssemblyShortName()).Where(x => !string.IsNullOrWhiteSpace(x)).ToList();
        }

        public static string GetFriendlyNameWithNamespace(this Type type)
        {
            var friendlyName = GetTypeName(type);
            return string.Format("{0}.{1}", type.Namespace, friendlyName);
        }

        public static string GetFriendlyName(this Type type)
        {
            var friendlyName = GetTypeName(type);
            return friendlyName;
        }

        public static string GetFriendlyFileName(this Type type)
        {
            var friendlyName = GetTypeName(type);
            return friendlyName.Replace("<", "[").Replace(">", "]");
        }

        public static string GetFriendlyNameForMethod(this MethodInfo methodInfo)
        {
            string friendlyName = methodInfo.Name;
            if (methodInfo.IsGenericMethod)
            {
                int iBacktick = friendlyName.IndexOf('`');
                if (iBacktick > 0)
                {
                    friendlyName = friendlyName.Remove(iBacktick);
                }
                friendlyName += "<";
                Type[] typeParameters = methodInfo.GetGenericArguments();
                for (int i = 0; i < typeParameters.Length; ++i)
                {
                    //string typeParamName = typeParameters[i].Name;
                    //friendlyName += (i == 0 ? typeParamName : "," + typeParamName);
                    var subType = typeParameters[i];
                    var subTypeName = GetFriendlyName(subType);
                    friendlyName += (i == 0 ? subTypeName : "," + subTypeName);
                }
                friendlyName += ">";
            }

            return friendlyName;
        }

        public static bool IsSubOfGeneric(this Type subType, Type genericType)
        {
            var interfaces = subType.GetInterfaces();
            foreach (var interfaceType in interfaces)
            {
                if (!interfaceType.IsGenericType)
                {
                    continue;
                }
                var subDefinition = interfaceType.GetGenericTypeDefinition();
                var isAssignableFrom = subDefinition.IsAssignableFrom(genericType);
                if (isAssignableFrom)
                {
                    return true;
                }
            }

            return false;
        }

        public static bool IsSubOfGeneric(this object instance, Type genericType)
        {
            return instance.GetType().IsSubOfGeneric(genericType);
        }

        /// <summary>
        /// 查找所有带有attributeType的静态方法
        /// </summary>
        /// <param name="assemblies"></param>
        /// <param name="attributeType"></param>
        /// <returns></returns>
        public static List<MethodInfo> GetAttributeMethods(this Type attributeType, params Assembly[] assemblies)
        {
            var methodInfos = assemblies.SelectMany(x => GetAttributeMethods(x, attributeType)).ToList();
            return methodInfos;
        }

        /// <summary>
        /// 查找所有扩展方法
        /// </summary>
        /// <param name="assemblies"></param>
        /// <param name="extendedType"></param>
        /// <returns></returns>
        public static List<MethodInfo> GetExtensionMethods(this Type extendedType, params Assembly[] assemblies)
        {
            var methodInfos = assemblies.SelectMany(x => GetExtensionMethods(x, extendedType)).ToList();
            return methodInfos;
        }

        /// <summary>
        /// 根据名称查找类型
        /// </summary>
        /// <param name="assemblies"></param>
        /// <param name="typeName"></param>
        /// <param name="tryFindWithoutNamespace"></param>
        /// <returns></returns>
        public static Type TryFindTypeByName(this IEnumerable<Assembly> assemblies, string typeName, bool tryFindWithoutNamespace = true)
        {
            foreach (var assembly in assemblies)
            {
                var theType = assembly.TryFindTypeByName(typeName, tryFindWithoutNamespace);
                if (theType != null)
                {
                    return theType;
                }
            }

            return null;
        }

        /// <summary>
        /// 根据名称查找类型
        /// </summary>
        /// <param name="assembly"></param>
        /// <param name="typeName"></param>
        /// <param name="tryFindWithoutNamespace"></param>
        /// <returns></returns>
        public static Type TryFindTypeByName(this Assembly assembly, string typeName, bool tryFindWithoutNamespace = true)
        {
            if (assembly == null) throw new ArgumentNullException(nameof(assembly));
            if (!tryFindWithoutNamespace)
            {
                return assembly.GetType(typeName, false, true);
            }
            var theOne = assembly.GetTypes().SingleOrDefault(x => x.FullName != null && (x.FullName.Equals(typeName, StringComparison.OrdinalIgnoreCase) || x.Name.Equals(typeName, StringComparison.OrdinalIgnoreCase)));
            return theOne;
        }

        /// <summary>
        /// 前缀过滤
        /// </summary>
        /// <param name="assemblies"></param>
        /// <param name="namespacePrefix"></param>
        /// <returns></returns>
        public static IEnumerable<Assembly> AssemblyNameStartWith(this IEnumerable<Assembly> assemblies, params string[] namespacePrefix)
        {
            return assemblies.Where(x => x.FullName.NameStartWith(namespacePrefix));
        }

        public static IEnumerable<string> AssemblyNameStartWith(this IEnumerable<string> names, params string[] namespacePrefix)
        {
            return names.Where(x => x.NameStartWith(namespacePrefix));
        }

        public static bool NameStartWith(this string name, params string[] namespacePrefix)
        {
            var startItems = namespacePrefix.Where(x => !string.IsNullOrEmpty(x)).Distinct().ToList();
            if (startItems.Count == 0)
            {
                return true;
            }

            if (string.IsNullOrWhiteSpace(name))
            {
                return false;
            }
            return startItems.Any(prefix => name.StartsWith(prefix, StringComparison.OrdinalIgnoreCase));
        }

        //----helpers----
        private static string GetTypeName(Type type)
        {
            if (type.IsGenericType)
            {
                return GetGenericTypeName(type);
            }
            return GetSimpleTypeName(type);
        }
        private static string GetSimpleTypeName(Type simpleType)
        {
            if (simpleType.IsGenericType)
            {
                throw new InvalidOperationException(simpleType + " Is Not A Simple Type.");
            }

            var namespacePrex = simpleType.Namespace + ".";
            var simpleClassNameTemplate = namespacePrex == "." ? simpleType.ToString() : simpleType.ToString().Replace(namespacePrex, "");
            return simpleClassNameTemplate;
        }
        private static string GetGenericTypeName(Type genericType)
        {
            if (!genericType.IsGenericType)
            {
                throw new InvalidOperationException(genericType + " Is Not A Generic Type.");
            }
            //var type = typeof(HelloWorld<int>.MyClass.HelloWorld2<string, object>);
            //ZQNB.Common.Extensions.HelloWorld`1+MyClass+HelloWorld2`2[System.Int32,System.String,System.Object]
            var namespacePrex = genericType.Namespace + ".";

            var genericClassNameTemplate = namespacePrex == "." ? genericType.ToString() : genericType.ToString().Replace(namespacePrex, "");
            var genericArguments = genericType.GetGenericArguments().ToList();

            int proecssedGenericIndex = 0;
            int indexOfGeneric = genericClassNameTemplate.IndexOf('`');
            while (indexOfGeneric > 0)
            {
                var currentGenericCount = Convert.ToInt32(genericClassNameTemplate[(indexOfGeneric + 1)].ToString());
                genericClassNameTemplate = genericClassNameTemplate.Remove(indexOfGeneric, 2);

                var currentGenericArguments = genericArguments.Skip(proecssedGenericIndex).Take(currentGenericCount).ToList();
                proecssedGenericIndex += currentGenericCount;

                var subTypeNames = currentGenericArguments.Select(GetTypeName).ToList();
                var subTypeName = "<" + string.Join(",", subTypeNames) + ">";
                genericClassNameTemplate = genericClassNameTemplate.Insert(indexOfGeneric, subTypeName);

                indexOfGeneric = genericClassNameTemplate.IndexOf('`');
            }

            int indexOfGenericParamsLast = genericClassNameTemplate.IndexOf('[');
            if (indexOfGenericParamsLast > 0)
            {
                genericClassNameTemplate = genericClassNameTemplate.Substring(0, indexOfGenericParamsLast);
            }

            return genericClassNameTemplate;

        }
        private static IEnumerable<MethodInfo> GetExtensionMethods(Assembly assembly, Type extendedType)
        {
            var query = from type in assembly.GetTypes()
                        where type.IsSealed && !type.IsGenericType && !type.IsNested
                        from method in type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                        where method.IsDefined(typeof(ExtensionAttribute), false)
                        where method.GetParameters()[0].ParameterType == extendedType
                        select method;
            return query;
        }
        private static IEnumerable<MethodInfo> GetAttributeMethods(Assembly assembly, Type attributeType)
        {
            var methods = assembly.GetTypes()
                .SelectMany(t => t.GetMethods())
                .Where(m => m.GetCustomAttributes(attributeType, true).Length > 0);
            return methods;
        }
    }
}
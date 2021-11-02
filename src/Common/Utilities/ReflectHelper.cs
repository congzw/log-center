using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Common.Utilities
{
    public class ReflectHelper
    {
        #region for di extensions

        [LazySingleton]
        public static ReflectHelper Instance => LazySingleton.Instance.Resolve(() => new ReflectHelper());

        #endregion

        public List<Assembly> LoadedRefAssemblies { get; set; } = new List<Assembly>();
        
        public List<Assembly> GetAssembliesFrom(string baseDirectory, string[] namespacePrefix)
        {
            baseDirectory ??= AppDomain.CurrentDomain.BaseDirectory;
            return GetAssembliesFrom(baseDirectory, name => name.NameStartWith(namespacePrefix));
        }

        public List<Assembly> GetAssembliesFrom(string baseDirectory, Func<string, bool> filter = null)
        {
            baseDirectory ??= AppDomain.CurrentDomain.BaseDirectory;

            var loadAssemblies = new List<Assembly>();
            var assemblyFiles = Directory.GetFiles(baseDirectory, "*.dll");
            var fileInfos = assemblyFiles.Select(x => new FileInfo(x)).ToList();
            Log($"find assemblyFiles: {baseDirectory} => {fileInfos.Select(x => x.Name).MyJoin()}");
            foreach (var fileInfo in fileInfos)
            {
                if (filter == null || filter(fileInfo.Name))
                {
                    Log($"Assembly.LoadFrom: {fileInfo.FullName}");
                    //LoadFile => InvalidCastException: [A]NbSites.Shared.ApiDoc.ApiDocMeta cannot be cast to [B]NbSites.Shared.ApiDoc.ApiDocMeta...
                    //var loadAssembly = Assembly.LoadFile(fileInfo.FullName);
                    var loadAssembly = Assembly.LoadFrom(fileInfo.FullName);
                    loadAssemblies.Add(loadAssembly);
                }
            }
            return loadAssemblies.OrderBy(x => x.FullName).ToList();
        }

        public MemberInfo GetMemberInfo(Expression<Func<object>> expression)
        {
            return GetLambdaBodyMemberInfo(expression);
        }

        public MemberInfo GetMemberInfo<T>(Expression<Func<T, object>> expression)
        {
            return GetLambdaBodyMemberInfo(expression);
        }
        
        public List<Assembly> GetAssembliesFromCurrentDomain(params string[] namespacePrefix)
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies(); ;
            return assemblies.AssemblyNameStartWith(namespacePrefix).ToList();
        }

        public List<Assembly> LoadAllAssemblies(Assembly entryAssembly, params string[] namespacePrefix)
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies().ToList();
            //Load referenced assemblies
            var refAssemblyNames = entryAssembly.GetReferencedAssemblies().OrderBy(x => x.FullName);
            foreach (var assemblyName in refAssemblyNames)
            {
                var loadOne = assemblies.SingleOrDefault(x => x.GetName(false) == assemblyName);
                if (loadOne == null)
                {
                    var assembly = Assembly.Load(assemblyName);
                    if (!LoadedRefAssemblies.Contains(assembly))
                    {
                        LoadedRefAssemblies.Add(assembly);
                    }
                }
            }
            //重新加载
            assemblies = AppDomain.CurrentDomain.GetAssemblies().OrderBy(x => x.FullName).ToList();
            if (namespacePrefix != null && namespacePrefix.Length > 0)
            {
                return assemblies.AssemblyNameStartWith(namespacePrefix).ToList();
            }
            return assemblies;
        }

        public List<T> FindUpAttributes<T>(Type currentType) where T : Attribute
        {
            var attrs = new List<T>();
            AppendParentGroupIf(attrs, currentType);
            return attrs;
        }

        private void AppendParentGroupIf<T>(List<T> attrs, Type currentType) where T : Attribute
        {
            if (attrs == null) throw new ArgumentNullException(nameof(attrs));

            var groupAttr = currentType.GetCustomAttribute<T>();
            if (groupAttr != null)
            {
                attrs.Add(groupAttr);
            }

            if (!currentType.IsNested)
            {
                return;
            }
            var parentType = currentType.DeclaringType;
            AppendParentGroupIf(attrs, parentType);
        }

        private void AppendReferencedAssemblies(Assembly entryAssembly, List<Assembly> assemblies, bool appendEntryAssembly, params string[] namespacePrefix)
        {
            if (!assemblies.Contains(entryAssembly) && appendEntryAssembly)
            {
                assemblies.Add(entryAssembly);
            }
            var refAssemblyNames = entryAssembly.GetReferencedAssemblies().OrderBy(x => x.FullName).Where(x => x.FullName.NameStartWith(namespacePrefix)).ToList();
            foreach (var refAssemblyName in refAssemblyNames)
            {
                var sameOne = assemblies.SingleOrDefault(x => x.GetName(false) == refAssemblyName);
                if (sameOne == null)
                {
                    var assembly = Assembly.Load(refAssemblyName);
                    assemblies.Add(assembly);
                    AppendReferencedAssemblies(assembly, assemblies, false, namespacePrefix);
                }
            }
        }

        private static void Log(string msg)
        {
            //InternalLog.Debug<ReflectHelper>(msg);
        }

        #region expression helper

        internal static MemberInfo GetLambdaBodyMemberInfo(LambdaExpression lambda)
        {
            if (lambda == null) throw new ArgumentNullException(nameof(lambda));
            var expressionBody = lambda.Body;
            if (expressionBody is MemberExpression memberExpression)
            {
                return memberExpression.Member;
            }
            if (expressionBody is MethodCallExpression methodCallExpression)
            {
                return methodCallExpression.Method;
            }
            if (expressionBody is UnaryExpression unaryExpression)
            {
                return ((MethodCallExpression)unaryExpression.Operand).Method;
            }
            throw new InvalidOperationException("没有处理: " + expressionBody.GetType().FullName);
        }

        #endregion
    }
}

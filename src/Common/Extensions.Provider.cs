//using System;
//using System.Collections.Concurrent;
//using System.Collections.Generic;
//using Microsoft.Extensions.DependencyInjection;

//namespace Common
//{
//    public static partial class Extensions
//    {
//        public static bool GetServicesIf<T>(this IServiceProvider serviceProvider)
//        {

//        }
//    }

    
//    public class AutoProviderHelper
//    {
//        public bool GetProviders<T>(IServiceProvider serviceProvider)
//        {
//            var theType = typeof(T);
//            if (RegisterProviders.TryGetValue(theType, out var hasIt))
//            {
//                return hasIt;
//            }

//            var registerProvider = serviceProvider.GetServices(theType);
//            RegisterProviders[theType] = registerProvider;
//        }

//        public IDictionary<Type, bool> RegisterProviders { get; set; } = new ConcurrentDictionary<Type, bool>();
//    }
//}

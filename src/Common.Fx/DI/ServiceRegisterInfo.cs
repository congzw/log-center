using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace Common.Fx.DI
{
    public static class ServiceDescriptorExtensions
    {
        public static IEnumerable<ServiceRegisterInfo> GetServiceRegisterInfos(this IServiceCollection services)
        {
            return services.Select(ServiceRegisterInfo.ToServiceRegisterInfo);
        }
    }

    public class ServiceRegisterInfo
    {
        public string Lifetime { get; set; }
        public string Service { get; set; }
        public string RegisterAs => GetRegisterAs();
        public string Impl { get; set; }
        public string ImplFactory => Factory != null ? "Yes" : "No";
        public string ImplInstance => Instance != null ? Instance.GetType().FullName : string.Empty;

        internal object Instance { get; set; }
        internal Func<IServiceProvider, object> Factory { get; set; }

        internal string GetRegisterAs()
        {
            if (Impl != null)
            {
                return "Impl";
            }
            if (Factory != null)
            {
                return "Factory";
            }
            if (Instance != null)
            {
                return "Instance";
            }
            return string.Empty;
        }

        public static ServiceRegisterInfo ToServiceRegisterInfo(ServiceDescriptor serviceDescriptor)
        {
            var info = new ServiceRegisterInfo();

            info.Lifetime = serviceDescriptor.Lifetime.ToString();
            info.Service = serviceDescriptor.ServiceType.FullName;
            info.Impl = serviceDescriptor.ImplementationType?.FullName;
            info.Factory = serviceDescriptor.ImplementationFactory;
            info.Instance = serviceDescriptor.ImplementationInstance;

            return info;
        }
    }
}

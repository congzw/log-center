using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using RI = System.Runtime.InteropServices.RuntimeInformation;

// ReSharper disable once CheckNamespace
namespace Common
{
    public class AppRuntime
    {
        public IDictionary<string, object> Bags { get; set; } = new ConcurrentDictionary<string, object>(StringComparer.OrdinalIgnoreCase);
        
        private bool _initApplied;
        public bool InitApplied()
        {
            return _initApplied;
        }

        public bool IsWindows()
        {
            return RI.IsOSPlatform(OSPlatform.Windows);
        }

        public void Init(Action<AppRuntime> setup)
        {
            if (_initApplied)
            {
                throw new InvalidOperationException("不能重复初始化！");
            }

            //init by default
            Bags[nameof(RI.FrameworkDescription)] = RI.FrameworkDescription;
            Bags[nameof(RI.OSDescription)] = RI.OSDescription;
            Bags[nameof(RI.OSArchitecture)] = RI.OSArchitecture;
            Bags[nameof(RI.ProcessArchitecture)] = RI.ProcessArchitecture;
            var hostName = Dns.GetHostName();
            Bags["DnsHostName"] = hostName;
            var addressList = Dns.GetHostEntry(hostName).AddressList.Select(x => x.ToString()).ToList();
            Bags["AddressList"] = addressList;

            //init by setup
            setup?.Invoke(this);
            
            //init by config
            Bags["AppRuntimeSourceExist"] = false;
            var source = AppRegistry.Instance.LoadFromFile<AppRuntime>(false)?.ValueAs<AppRuntime>();
            if (source != null)
            {
                Bags["AppRuntimeSourceExist"] = true;
                foreach (var bag in source.Bags)
                {
                    Bags[bag.Key] = bag.Value;
                }
            }

            _initApplied = true;
        }
       
        //方便使用
        public static AppRuntime Instance => AppRegistry.Instance.AppRuntime();
    }

    public static class AppRuntimeExtensions
    {
        [AppRegistryExtend]
        public static AppRuntime AppRuntime(this AppRegistry registry)
        {
            //框架不负责初始化逻辑，由扩展程序决定
            var appRuntime = registry.GetOrCreate<AppRuntime>().WithTitle("应用运行时信息").ValueAs<AppRuntime>();
            return appRuntime;
        }
    }
}

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Common.Logs;
using Common.Utilities;
using Microsoft.Extensions.DependencyInjection;

// ReSharper disable once CheckNamespace
namespace Common
{
    public class AppRegistry
    {
        public List<AppRegistryItem> Items { get; set; } = new List<AppRegistryItem>();
        public AppRegistryItem TryFind(string itemId)
        {
            return Items.SingleOrDefault(x => x.Id.Equals(itemId, StringComparison.OrdinalIgnoreCase));
        }

        //防止被序列化
        private List<Assembly> _assemblies = new List<Assembly>();
        public List<Assembly> GetAppAssemblies()
        {
            return _assemblies;
        }

        public MemoryLogger Logger { get; set; } = MemoryLogFactory.Instance.GetLogger(typeof(AppRegistry).FullName).WithMaxLength(200);

        public void Register(params AppRegistryItem[] items)
        {
            foreach (var item in items)
            {
                if (!Items.Contains(item))
                {
                    Items.Add(item);
                }
            }
        }

        public AppRegistry Init(params Assembly[] assemblies)
        {
            _assemblies = assemblies.ToList();

            var methodInfos = this.GetType().GetExtensionMethods(assemblies);
            foreach (var methodInfo in methodInfos)
            {
                var autoRegisterAttributes = methodInfo.GetCustomAttributes<AppRegistryExtendAttribute>().ToList();
                if (autoRegisterAttributes.Count > 0)
                {
                    methodInfo.Invoke(null, new object[] { this });
                }
            }
            return this;
        }
        
        public static AppRegistry Instance = new AppRegistry();
    }

    public class AppRegistryItem
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Group { get; set; }
        public object Value { get; set; }
        public T ValueAs<T>(bool alwaysByJson = false)
        {
            //处理特殊属性，例如字典的JObject问题
            return Value.ConvertAs<T>(alwaysByJson);
        }

        #region equatable helpers

        public string GetCompareKey()
        {
            return $"{Id}".ToLower();
        }

        public bool Equals(AppRegistryItem other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Id == other.Id;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((AppRegistryItem)obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(GetCompareKey());
        }

        #endregion

        public AppRegistryItem WithTitle(string title)
        {
            Title = title;
            return this;
        }
        public AppRegistryItem WithGroup(string group)
        {
            Group = group;
            return this;
        }

        public static string RootGroup = ".Root";
        public static AppRegistryItem Create(string metaId, object metaValue)
        {
            if (string.IsNullOrWhiteSpace(metaId)) throw new ArgumentNullException(nameof(metaId));
            if (metaValue == null) throw new ArgumentNullException(nameof(metaValue));

            var item = new AppRegistryItem
            {
                Id = metaId,
                Title = metaId,
                Group = RootGroup,
                Value = metaValue
            };
            
            return item;
        }
    }
    
    [AttributeUsage(AttributeTargets.Method)]
    public class AppRegistryExtendAttribute : Attribute
    {
    }

    public static class AppRegistryExtensions
    {
        public static string CreateItemId<T>(this AppRegistry registry)
        {
            if (registry == null) throw new ArgumentNullException(nameof(registry));
            return typeof(T).GetFriendlyName();
        }

        public static AppRegistry SetupSingletons(this AppRegistry registry, IServiceCollection services)
        {
            if (registry == null) throw new ArgumentNullException(nameof(registry));
            //把注册表和包含的项目，全部注册成Singleton
            services.AddSingleton(registry);
            foreach (var item in registry.Items)
            {
                var theType = item.Value.GetType();
                registry.Logger.Log($"AppRegistry Items Singleton Setup => {theType.GetFriendlyName()}");
                services.AddSingleton(theType, item.Value);
            }
            return registry;
        }

        public static AppRegistryItem Register(this AppRegistryItem theItem, AppRegistry registry)
        {
            if (theItem == null) throw new ArgumentNullException(nameof(theItem));
            if (registry == null) throw new ArgumentNullException(nameof(registry));

            registry.Register(theItem);
            return theItem;
        }

        public static AppRegistryItem TryFindItemValue<T>(this AppRegistry registry, string itemId)
        {
            if (registry == null) throw new ArgumentNullException(nameof(registry));
            if (itemId == null) throw new ArgumentNullException(nameof(itemId));

            var theItem = registry.TryFind(itemId);
            return theItem;
        }
        
        public static AppRegistryItem GetOrCreate<T>(this AppRegistry registry) where T : new()
        {
            var itemId = registry.CreateItemId<T>();
            var itemValue = registry.TryFindItemValue<T>(itemId);
            if (itemValue != null)
            {
                return itemValue;
            }
            itemValue = AppRegistryItem.Create(itemId, new T()).Register(registry);
            return itemValue;
        }
        
        public static List<AppRegistryItem> LoadFromFile(this AppRegistry registry)
        {
            if (registry == null) throw new ArgumentNullException(nameof(registry));
            
            var fileDbHelper = FileDbHelper.Instance;
            var ids = registry.Items.Select(x => x.Id).ToList();

            var items = new List<AppRegistryItem>();
            foreach (var id in ids)
            {
                var filePath = fileDbHelper.MakeAppDataFilePath("AppRegistry", $"{id}.json");
                var theOne = fileDbHelper.Read<AppRegistryItem>(filePath).SingleOrDefault();
                if (theOne != null)
                {
                    items.Add(theOne);
                }
            }
            return items;
        }
        public static AppRegistryItem LoadFromFile(this AppRegistry registry, string id)
        {
            if (registry == null) throw new ArgumentNullException(nameof(registry));

            var fileDbHelper = FileDbHelper.Instance;
            var filePath = fileDbHelper.MakeAppDataFilePath("AppRegistry", $"{id}.json");
            var theOne = fileDbHelper.Read<AppRegistryItem>(filePath).SingleOrDefault();
            return theOne;
        }
        public static void SaveToFile(this AppRegistry registry, AppRegistryItem item)
        {
            if (registry == null) throw new ArgumentNullException(nameof(registry));
            if (item == null) throw new ArgumentNullException(nameof(item));
            var fileDbHelper = FileDbHelper.Instance;
            var filePath = fileDbHelper.MakeAppDataFilePath("AppRegistry", $"{item.Id}.json");
            fileDbHelper.Save(filePath, item);
        }
        public static AppRegistryItem LoadFromFile<T>(this AppRegistry registry, bool updateSelf) where T : new()
        {
            if (registry == null) throw new ArgumentNullException(nameof(registry));
            var itemId = registry.CreateItemId<T>();
            var theSource =registry.LoadFromFile(itemId);
            if (theSource != null)
            {
                var item = registry.GetOrCreate<T>();
                if (updateSelf)
                {
                    item.Id = theSource.Id;
                    item.Title = theSource.Title;
                    item.Group = theSource.Group;
                    item.Value = theSource.Value;
                    return item;
                }
            }
            return theSource;
        }
    }
}

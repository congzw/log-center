using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Common.Utilities
{
    #region abstracts
    
    public interface IGroupName
    {
        /// <summary>
        /// 分组
        /// </summary>
        public string Group { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }
    }

    public interface IGroupNameFile : IGroupName
    {
        string Extension { get; set; }
        string BaseDirectory { get; set; }
    }

    public interface IGroupNameHelper
    {
        IEnumerable<T> Search<T>(IEnumerable<T> source, GroupNameSearchArgs args) where T : IGroupName;
        IEnumerable<T> Locate<T>(IEnumerable<T> source, GroupNameLocateArgs args) where T : IGroupName;
    }
    
    public class GroupNameSearchArgs
    {
        public string Group { get; set; }
        public string Name { get; set; }
        public bool DrillDown { get; set; }

        public GroupNameSearchArgs AutoFix()
        {
            Group = GroupName.Create(Group, string.Empty).AutoFix().Group;
            if (string.IsNullOrWhiteSpace(Name))
            {
                Name = string.Empty;
            }
            return this;
        }

        public static GroupNameSearchArgs Create(string theGroup, string name, bool drillDown)
        {
            return new GroupNameSearchArgs {Group = theGroup, Name = name, DrillDown = drillDown};
        }
    }

    public class GroupNameLocateArgs
    {
        public string Group { get; set; }
        public string Names { get; set; }
        
        public GroupNameLocateArgs AutoFix()
        {
            Group = GroupName.Create(Group, string.Empty).AutoFix().Group;
            if (string.IsNullOrWhiteSpace(Names))
            {
                Names = string.Empty;
            }
            return this;
        }
        
        public static GroupNameLocateArgs Create(string theGroup, string names)
        {
            return new GroupNameLocateArgs {Group = theGroup, Names = names};
        }
    }

    #endregion

    #region extensions
    
    public static class GroupNameExtensions
    {
        public static string GetGroupFix(this IGroupName item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));

            if (string.IsNullOrWhiteSpace(item.Group))
            {
                return "/";
            }

            var theGroup = item.Group.Trim().Replace('\\', '/').TrimStart('/').TrimEnd('/');
            if (string.IsNullOrWhiteSpace(theGroup))
            {
                return "/";
            }
            return $"/{theGroup}/";
        }

        public static string GetNameFix(this IGroupName item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));

            if (string.IsNullOrWhiteSpace(item.Name))
            {
                return "";
            }

            var theName = item.Name.Trim().Replace('\\', '/').TrimStart('/');
            return theName;
        }

        public static string GetFullGroupName(this IGroupName item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));
            return $"{item.GetGroupFix()}{item.GetNameFix()}";
        }
        
        public static T AutoFix<T>(this T item) where T : IGroupName
        {
            if (item != null)
            {
                item.Group = item.GetGroupFix();
                item.Name = item.GetNameFix();
            }
            return item;
        }
        
        public static IEnumerable<T> Search<T>(this IEnumerable<T> source, GroupNameSearchArgs args) where T : IGroupName
        {
            return GroupNameHelper.Instance.Search(source, args);
        }
        
        public static IEnumerable<T> Locate<T>(this IEnumerable<T> source, GroupNameLocateArgs args) where T : IGroupName
        {
            return GroupNameHelper.Instance.Locate(source, args);
        }
        
        public static IEnumerable<T> OrderByGroupName<T>(this IEnumerable<T> source) where T : IGroupName
        {
            if (source == null)
            {
                return Enumerable.Empty<T>();
            }
            return source.OrderBy(x => x.Group).ThenBy(x => x.Name);
        }
    }

    public static class GroupNameFileExtensions
    {
        public static string GetExtensionFix(this IGroupNameFile item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));

            var ext = string.Empty;
            if (string.IsNullOrWhiteSpace(item.Extension))
            {
                return ext;
            }

            ext = "." + item.Extension.Trim().Replace('。', '.').TrimStart('.');
            return ext;
        }
        
        public static string GetBaseDirectoryFix(this IGroupNameFile item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));

            var basePath = string.Empty;
            if (string.IsNullOrWhiteSpace(item.BaseDirectory))
            {
                return basePath;
            }

            basePath = item.BaseDirectory.Trim().Replace('：', ':').Replace('\\','/').TrimEnd('/');
            return basePath;
        }
        
        public static T AutoFixFile<T>(this T item) where T : IGroupNameFile
        {
            if (item != null)
            {
                item.AutoFix();
                item.Extension = item.GetExtensionFix();
                item.BaseDirectory = item.GetBaseDirectoryFix();
            }
            return item;
        }
        
        public static string GetFullFilePath(this IGroupNameFile item)
        {
            item.AutoFixFile();
            return $"{item.BaseDirectory}{item.Group}{item.Name}{item.Extension}";
        }

        public static string GetFullGroupPath(this IGroupNameFile item)
        {
            item.AutoFixFile();
            return $"{item.BaseDirectory}{item.Group}";
        }

        public static string GetFullGroupNameWithExtension(this IGroupNameFile item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));
            return $"{item.GetGroupFix()}{item.GetNameFix()}{item.GetExtensionFix()}";
        }
        
        public static GroupNameFile ToGroupNameFile(this FileInfo fileInfo, string basePath)
        {
            if (string.IsNullOrWhiteSpace(basePath))
            {
                throw new ArgumentNullException(nameof(basePath));
            }
            return GroupNameFile.Create(basePath, fileInfo.FullName);
        }
        
        public static List<GroupNameFile> ToGroupNameFiles(this IEnumerable<FileInfo> fileInfos, string basePath)
        {
            return fileInfos.Select(x => x.ToGroupNameFile(basePath)).OrderBy(x => x.Group).ThenBy(x => x.Name).ToList();
        }

        public static List<GroupNameFile> ToGroupNameFiles(this IEnumerable<string> files, string basePath)
        {
            return files.Select(x => GroupNameFile.Create(basePath, x)).OrderBy(x => x.Group).ThenBy(x => x.Name).ToList();
        }
    }

    #endregion

    public class GroupName : IGroupName
    {
        /// <summary>
        /// 分组
        /// </summary>
        public string Group { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        public static GroupName Create(string theGroup, string theName)
        {
            var item = new GroupName();
            item.Group = theGroup;
            item.Name = theName;
            return item;
        }
    }

    public class GroupNameFile : IGroupNameFile
    {
        public string Group { get; set; }
        public string Name { get; set; }
        public string Extension { get; set; }
        public string BaseDirectory { get; set; }
        
        public static GroupNameFile Create(string basePath, string fileFullPath)
        {
            if (basePath == null) throw new ArgumentNullException(nameof(basePath));
            if (string.IsNullOrWhiteSpace(fileFullPath))
            { 
                throw new ArgumentNullException(nameof(fileFullPath));
            }
            
            var item = new GroupNameFile();
            
            item.BaseDirectory = basePath;
            item.BaseDirectory = item.GetBaseDirectoryFix();

            item.Name = Path.GetFileNameWithoutExtension(fileFullPath);
            item.Name = item.GetNameFix();

            item.Extension = Path.GetExtension(fileFullPath);
            item.Extension = item.GetExtensionFix();
            
            var basePathFix = basePath.Replace('\\', '/').TrimEnd('/');
            item.Group = Path.GetDirectoryName(fileFullPath)?.Replace('\\', '/').Replace(basePathFix, "");
            item.Group = item.GetGroupFix();
            return item;
        }
        
        public static GroupNameFile Create(string basePath, string group, string name, string extension)
        {
            return new GroupNameFile { BaseDirectory = basePath, Group = group, Name = name, Extension = extension };
        }

        public static List<FileInfo> GetFiles(string basePath, bool drillDown, string searchPattern)
        {
            if (string.IsNullOrWhiteSpace(basePath))
            {
                basePath = AppDomain.CurrentDomain.BaseDirectory;
            }

            var items = new List<FileInfo>();

            var foundFiles = Directory.GetFiles(basePath, searchPattern, drillDown ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
            foreach (var foundFile in foundFiles)
            {
                var fileInfo = new FileInfo(foundFile);
                items.Add(fileInfo);
            }

            return items;
        }
    }
    
    public class GroupNameValue : IGroupName
    {
        public string Group { get; set; }
        public string Name { get; set; }
        public object Value { get; set; }
    }

    public class GroupNameHelper : IGroupNameHelper
    {
        #region auto resolve from di or default

        [LazySingleton]
        public static IGroupNameHelper Instance => LazySingleton.Instance.Resolve(() => new GroupNameHelper());

        #endregion

        public IEnumerable<T> Search<T>(IEnumerable<T> source, GroupNameSearchArgs args) where T : IGroupName
        {
            if (args == null) throw new ArgumentNullException(nameof(args));
            if (source == null)
            {
                return Enumerable.Empty<T>();
            }

            var searchArgs = args.AutoFix();
            var query = source;
            query = searchArgs.DrillDown
                ? query.Where(x => x.GetGroupFix().StartsWith(searchArgs.Group, StringComparison.OrdinalIgnoreCase))
                : query.Where(x => x.GetGroupFix().Equals(searchArgs.Group, StringComparison.OrdinalIgnoreCase));
            
            if (!string.IsNullOrWhiteSpace(searchArgs.Name))
            {
                query = query.Where(x => x.GetNameFix().Contains(searchArgs.Name, StringComparison.OrdinalIgnoreCase));
            }

            return query;
        }
        
        public IEnumerable<T> Locate<T>(IEnumerable<T> source, GroupNameLocateArgs args) where T : IGroupName
        {
            if (args == null) throw new ArgumentNullException(nameof(args));
            if (source == null)
            {
                return new List<T>();
            }

            var names = args.Names.MySplit();
            if (names.Count == 0)
            {
                return new List<T>();
            }
            
            var theGroup = GroupName.Create(args.Group, "").AutoFix().Group;
            var query = source.Select(x => x.AutoFix());
            query = query.Where(x => x.Group.MyEquals(theGroup));
            if (!names.Contains("*"))
            {
                query = names.Count > 0 
                    ? query.Where(x => names.MyContains(x.Name))
                    : query.Where(x => false);
            }
            
            return query.OrderByGroupName().ToList();
        }
    }
}

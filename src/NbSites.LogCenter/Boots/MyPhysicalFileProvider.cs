using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;

namespace NbSites.LogCenter.Boots
{
    /// <summary>
    /// 解决Linux文件大小写的问题
    /// </summary>
    public class MyPhysicalFileProvider : IFileProvider
    {
        private readonly IFileProvider _provider;

        public MyPhysicalFileProvider(IFileProvider provider)
        {
            _provider = provider;
            FileCache = new ConcurrentDictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        }

        public IDictionary<string, string> FileCache { get; set; }


        public IFileInfo GetFileInfo(string subPath)
        {
            if (FileCache.TryGetValue(subPath, out var value))
            {
                if (subPath != value)
                {
                    return _provider.GetFileInfo(value);
                }
            }
            return _provider.GetFileInfo(subPath);
        }

        public IDirectoryContents GetDirectoryContents(string subPath)
        {
            if (FileCache.TryGetValue(subPath, out string value))
            {
                if (subPath != value)
                {
                    return _provider.GetDirectoryContents(value);
                }
            }
            return _provider.GetDirectoryContents(subPath);
        }

        public IChangeToken Watch(string filter)
        {
            return _provider.Watch(filter);
        }

        public void InitMap(string parentPath, IFileProvider fileProvider)
        {
            var currentContents = fileProvider.GetDirectoryContents(parentPath);
            foreach (var currentContent in currentContents)
            {
                var currentPath = parentPath + "/" + currentContent.Name;
                FileCache.Add(currentPath, currentPath);
                if (currentContent.IsDirectory)
                {
                    InitMap(currentPath, fileProvider);
                }
            }
        }
    }
}

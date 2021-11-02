using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Common.Utilities
{
    public class FileHelper
    {
        #region auto resolve from di or default

        [LazySingleton]
        public static FileHelper Instance => LazySingleton.Instance.Resolve(() => new FileHelper());

        #endregion

        public async Task<string> ReadFileAsync(string filePath)
        {
            if (!File.Exists(filePath))
            {
                return null;
            }
            return await File.ReadAllTextAsync(filePath);
        }
        public Task SaveFileAsync(string filePath, string content)
        {
            string dirPath = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }
            return File.WriteAllTextAsync(filePath, content, Encoding.UTF8);
        }

        public string ReadFile(string filePath)
        {
            if (!File.Exists(filePath))
            {
                return null;
            }

            return File.ReadAllText(filePath);
        }
        public void SaveFile(string filePath, string content)
        {
            string dirPath = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }
            File.WriteAllText(filePath, content, Encoding.UTF8);
        }

        #region path

        public string CombinePath(params string[] paths)
        {
            //todo: auto fix paths
            return Path.Combine(paths);
        }
        public string CombineBaseDirectory(params string[] paths)
        {
            var items = new List<string>();
            items.Add(AppDomain.CurrentDomain.BaseDirectory);
            foreach (var path in paths)
            {
                if (!string.IsNullOrWhiteSpace(path))
                {
                    items.Add(path);
                }
            }
            return CombinePath(items.ToArray());
        }

        #endregion
    }

    public static class FileHelperExtensions
    {
        public static string GetPathForAppData(this FileHelper helper)
        {
            return helper.CombineBaseDirectory("App_Data");
        }
        public static string GetPathForInit(this FileHelper helper)
        {
            return helper.CombineBaseDirectory("App_Data", "Init");
        }
    }
}

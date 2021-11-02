using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Common.Utilities
{
    public interface IFileDbHelper
    {
        Task<List<T>> ReadAsync<T>(string path);
        Task SaveAsync<T>(string path, IEnumerable<T> list);

        List<T> Read<T>(string path);
        void Save<T>(string path, IEnumerable<T> list);
    }

    public static class FileDbHelperExtensions
    {
        public static string MakeFileName<T>(this IFileDbHelper helper, string suffix = ".json")
        {
            Type type = typeof(T);
            return helper.MakeFileName(type, suffix);
        }

        public static string MakeFileName(this IFileDbHelper helper, Type type, string suffix = ".json")
        {
            string className = type.GetFriendlyFileName();
            return $"{className}{suffix}";
        }

        public static string MakeAppDataFilePath(this IFileDbHelper helper, params string[] paths)
        {
            var items = new List<string>();
            items.Add("App_Data");
            foreach (var path in paths)
            {
                if (!string.IsNullOrWhiteSpace(path))
                {
                    items.Add(path);
                }
            }
            return FileHelper.Instance.CombineBaseDirectory(items.ToArray());
        }

        public static Task SaveAsync<T>(this IFileDbHelper helper, string path, params T[] items)
        {
            return helper.SaveAsync(path, items.ToList());
        }
        public static void Save<T>(this IFileDbHelper helper, string path, params T[] items)
        {
            helper.Save(path, items.ToList());
        }
    }

    /// <summary>
    /// Default Impl: JsonFile
    /// </summary>
    public class FileDbHelper : IFileDbHelper
    {
        #region auto resolve from di or default

        [LazySingleton]
        public static IFileDbHelper Instance => LazySingleton.Instance.Resolve(() => new FileDbHelper());

        #endregion

        public async Task<List<T>> ReadAsync<T>(string path)
        {
            var defaultResult = new List<T>();
            var jsonValue = await FileHelper.Instance.ReadFileAsync(path);
            if (string.IsNullOrWhiteSpace(jsonValue))
            {
                return defaultResult;
            }
            return jsonValue.FromJson(defaultResult);
        }

        public List<T> Read<T>(string path)
        {
            var defaultResult = new List<T>();
            var jsonValue = FileHelper.Instance.ReadFile(path);
            if (string.IsNullOrWhiteSpace(jsonValue))
            {
                return defaultResult;
            }
            return jsonValue.FromJson(defaultResult);
        }

        public Task SaveAsync<T>(string path, IEnumerable<T> list)
        {
            var jsonValue = list.ToJson(true);
            return FileHelper.Instance.SaveFileAsync(path, jsonValue);
        }

        public void Save<T>(string path, IEnumerable<T> list)
        {
            var jsonValue = list.ToJson(true);
            FileHelper.Instance.SaveFile(path, jsonValue);
        }
    }
}
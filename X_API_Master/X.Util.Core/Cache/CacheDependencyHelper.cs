using System;
using System.IO;
using System.Reflection;
using System.Runtime.Caching;
using System.Text;
using X.Util.Core.Configuration;
using X.Util.Core.Kernel;
using X.Util.Core.Log;
using X.Util.Entities.Enums;

namespace X.Util.Core.Cache
{
    public class CacheDependencyHelper
    {
        /// <summary>
        /// 缓存依赖文件
        /// </summary>
        private static string CacheDependencyFile
        {
            get { return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data\\CacheDependency.cdy"); }
        }

        /// <summary>
        /// 内存key映射本地文件
        /// </summary>
        /// <param name="key"></param>
        /// <param name="createFold"></param>
        /// <returns></returns>
        private static string GetCacheDependencyPath(string key, bool createFold)
        {
            var cacheDependencyBaseDirectory = AppConfig.CacheDependencyBaseDirectory;
            if (string.IsNullOrEmpty(cacheDependencyBaseDirectory))
            {
                cacheDependencyBaseDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory + "/../", "CacheDependencyBaseDirectory");
            }
            var sha1 = CoreUtil.Sha1(key);
            var fold = Path.Combine(cacheDependencyBaseDirectory, sha1.Substring(20));
            var path = sha1.Substring(20, 20) + ".cdy";
            if (createFold && !Directory.Exists(fold))
            {
                Directory.CreateDirectory(fold);
            }
            return Path.Combine(fold, path);
        }

        /// <summary>
        /// 更新缓存缓存依赖文件内容
        /// </summary>
        /// <param name="createNotModify">只创建文件不覆盖</param>
        /// <returns></returns>
        public static void UpdateCacheDependencyFile(bool createNotModify)
        {
            var file = CacheDependencyFile;
            if (createNotModify && File.Exists(file)) return;
            try
            {
                File.WriteAllText(file, string.Empty, Encoding.UTF8);
            }
            catch (Exception ex)
            {
                Logger.Client.Error(Logger.Client.GetMethodInfo(MethodBase.GetCurrentMethod(), new object[] { file }), ex, LogDomain.Util);
            }
        }

        /// <summary>
        /// 更新缓存缓存依赖文件内容
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string UpdateCacheDependencyFile(string key, object value)
        {
            var path = GetCacheDependencyPath(key, true);
            try
            {
                File.WriteAllText(path, value != null ? value.ToJson() : string.Empty, Encoding.UTF8);
            }
            catch (Exception ex)
            {
                Logger.Client.Error(Logger.Client.GetMethodInfo(MethodBase.GetCurrentMethod(), new object[] { key, path }), ex, LogDomain.Util);
            }
            return path;
        }

        /// <summary>
        /// 缓存依赖
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static HostFileChangeMonitor CacheDependency(string key, object value)
        {
            UpdateCacheDependencyFile(true);
            var path = UpdateCacheDependencyFile(key, value);
            return new HostFileChangeMonitor(new[] { path, CacheDependencyFile });
        }

        /// <summary>
        /// 从文件中获取缓存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public static T GetCacheResultFromFile<T>(string key)
        {
            var result = default(T);
            var path = GetCacheDependencyPath(key, false);
            if (!File.Exists(path)) return result;
            var content = File.ReadAllText(path, Encoding.UTF8);
            if (!string.IsNullOrEmpty(content))
            {
                result = content.FromJson<T>();
            }
            return result;
        }
    }
}

using System;
using System.IO;
using System.Reflection;
using System.Text;
using X.Util.Core;
using X.Util.Core.Kernel;
using X.Util.Core.Log;
using X.Util.Entities;
using X.Util.Entities.Enum;

namespace X.Util.Other
{
    public class FileBase
    {
        public static bool WriteFile(string filePath, string fileName, string content, string encode, FileBaseMode mode)
        {
            var result = false;
            try
            {
                var dir = new DirectoryInfo(filePath);
                if (!dir.Exists) dir.Create();
            }
            catch (Exception ex)
            {
                Logger.Client.Error(Logger.Client.GetMethodInfo(MethodBase.GetCurrentMethod(), new object[] { filePath, fileName, content, encode, mode }), ex, LogDomain.Util);
            }
            var fm = FileBaseMode.Append.Equals(mode) ? FileMode.OpenOrCreate : FileMode.Create;
            FileStream fs = default(FileStream);
            var realFilePath = Path.Combine(filePath, fileName);
            var ed = encode.Contains("utf8") || encode.Contains("utf-8") ? Encoding.UTF8 : encode.Contains("unicode") ? Encoding.Unicode : Encoding.GetEncoding(encode);
            CoreUtil.CoderLocker(realFilePath, () =>
            {
                try
                {
                    fs = new FileStream(realFilePath, fm, FileAccess.Write);
                    var sw = new StreamWriter(fs, ed);
                    sw.BaseStream.Seek(0, SeekOrigin.End);
                    sw.WriteLine(content);
                    sw.Flush();
                    sw.Close();
                    result = true;
                }
                catch (Exception ex)
                {
                    Logger.Client.Error(Logger.Client.GetMethodInfo(MethodBase.GetCurrentMethod(), new object[] { filePath, fileName, content, encode, mode }), ex, LogDomain.Util);
                }
                finally
                {
                    if (fs != null) fs.Close();
                }
            });
            return result;
        }

        public static string ReadFile(string filePath, string encode)
        {
            var ed = encode.Contains("utf8") || encode.Contains("utf-8") ? Encoding.UTF8 : encode.Contains("unicode") ? Encoding.Unicode : Encoding.GetEncoding(encode);
            var sr = new StreamReader(filePath, ed);
            var content = sr.ReadToEnd();
            sr.Close();
            return content;
        }

        public static T ReadJson<T>(string filePath, string encode)
        {
            return ReadFile(filePath, encode).FromJson<T>();
        }

        public static byte[] GetFileBytes(string path)
        {
            var result = default(byte[]);
            try
            {
                var fi = new FileInfo(path);
                if (fi.Exists)
                {
                    var len = fi.Length;
                    var fs = new FileStream(path, FileMode.Open);
                    var buffer = new byte[len];
                    fs.Read(buffer, 0, (int)len);
                    fs.Close();
                    result = buffer;
                }
            }
            catch (Exception e)
            {
                Logger.Client.Error(Logger.Client.GetMethodInfo(MethodBase.GetCurrentMethod(), new object[] { path }), e, LogDomain.Util);
            }
            return result;
        }

        public static byte[] GetStreamBytes(Stream stream)
        {
            var result = default(byte[]);
            try
            {
                var len = stream.Length;
                var bytes = new byte[len];
                stream.Read(bytes, 0, (int)len);
                stream.Seek(0, SeekOrigin.Begin);
                stream.Close();
                result = bytes;
            }
            catch (Exception e)
            {
                Logger.Client.Error(Logger.Client.GetMethodInfo(MethodBase.GetCurrentMethod(), new object[] { }), e, LogDomain.Util);
            }
            return result;
        }

        public static bool DeleteDirectory(string filePath)
        {
            var state = false;
            try
            {
                var dir = new DirectoryInfo(filePath);
                if (dir.Exists) dir.Delete(true);
                state = true;
            }
            catch (Exception ex)
            {
                Logger.Client.Error(Logger.Client.GetMethodInfo(MethodBase.GetCurrentMethod(), new object[] { filePath }), ex, LogDomain.Util);
            }
            return state;
        }

        public static bool DeleteFile(string filePath)
        {
            var state = false;
            try
            {
                var dir = new FileInfo(filePath);
                if (dir.Exists) dir.Delete();
                state = true;
            }
            catch (Exception ex)
            {
                Logger.Client.Error(Logger.Client.GetMethodInfo(MethodBase.GetCurrentMethod(), new object[] { filePath }), ex, LogDomain.Util);
            }
            return state;
        }
    }
}

using System;
using System.IO;
using System.Reflection;
using System.Text;
using X.Util.Core;

namespace X.Util.Other
{
    public enum FileBaseMode
    {
        Create,
        Append
    }
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
                Logger.Error(MethodBase.GetCurrentMethod(), LogDomain.Util, null, "Directory error:" + ex);
            }
            var fm = FileBaseMode.Append.Equals(mode) ? FileMode.OpenOrCreate : FileMode.Create;
            FileStream fs = null;
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
                    Logger.Error(MethodBase.GetCurrentMethod(), LogDomain.Util, null, "save err：" + ex);
                }
                finally
                {
                    fs?.Close();
                }
            });
            return result;
        }

        public static string ReadFile(string filePath, string encode)
        {
            var sr = new StreamReader(filePath, Encoding.GetEncoding(encode));
            var content = sr.ReadToEnd();
            sr.Close();
            return content;
        }

        public static T ReadJson<T>(string filePath, string encode)
        {
            return ReadFile(filePath, encode).FromJson<T>();
        }

        public static bool Delete(string filePath)
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
                Logger.Error(MethodBase.GetCurrentMethod(), LogDomain.Util, null, "Directory error:" + ex);
            }
            return state;
        }
    }
}

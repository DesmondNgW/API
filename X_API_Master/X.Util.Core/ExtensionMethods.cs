using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace X.Util.Core
{
    public static class ExtensionMethods
    {
        #region 序列化
        private static readonly BinaryFormatter BinaryFormatter = new BinaryFormatter();
        public static bool IsJson(this string json)
        {
            json = new Regex("\\\\(?:[\"'\\/bfnrt]|u[0-9a-fA-F]{4})").Replace(json, m => "@");
            json = new Regex("\"[^\"\\\\n\\r]*\"|'[^\"\\\n\\r]*'|true|false|null|-?\\d+(?:\\.\\d*)?(?:[eE][+\\-]?\\d+)?").Replace(json, m => "]");
            json = new Regex("(?:^|:|,)(?:\\s*\\[)+").Replace(json, m => "");
            return Regex.IsMatch(json, "^[\\],:{}\\s]*$");
        }

        public static T FromJson<T>(this string json)
        {
            return JsonConvert.DeserializeObject<T>(json, new JsonSerializerSettings { MaxDepth = int.MaxValue, DateTimeZoneHandling = DateTimeZoneHandling.Local });
        }

        public static object FromJson(this string json, Type T)
        {
            return JsonConvert.DeserializeObject(json, T, new JsonSerializerSettings { MaxDepth = int.MaxValue, DateTimeZoneHandling = DateTimeZoneHandling.Local });
        }

        public static string ToJson(this object t)
        {
            return JsonConvert.SerializeObject(t, new JsonSerializerSettings { MaxDepth = int.MaxValue, DateTimeZoneHandling = DateTimeZoneHandling.Local });
        }

        public static TDest AutoMapper<TDest>(this object src)
        {
            return src.ToJson().FromJson<TDest>();
        }

        public static byte[] Serialize(this object t)
        {
            var mStream = new MemoryStream();
            BinaryFormatter.Serialize(mStream, t);
            return mStream.GetBuffer();
        }

        public static T Deserialize<T>(this byte[] b)
        {
            return (T)BinaryFormatter.Deserialize(new MemoryStream(b));
        } 
        #endregion

        #region decimal
        public static string FloorString(this decimal d, int length)
        {
            return (Math.Floor(d * (long)Math.Pow(10, length)) / (long)Math.Pow(10, length)).ToString("N" + length);
        }

        public static decimal FloorDecimal(this decimal d, int length)
        {
            return Math.Floor(d * (long)Math.Pow(10, length)) / (long)Math.Pow(10, length);
        }

        public static string CeilingString(this decimal d, int length)
        {
            return (Math.Ceiling(d * (long)Math.Pow(10, length)) / (long)Math.Pow(10, length)).ToString("N" + length);
        }

        public static decimal CeilingDecimal(this decimal d, int length)
        {
            return Math.Ceiling(d * (long)Math.Pow(10, length)) / (long)Math.Pow(10, length);
        } 
        #endregion

        #region string
        public static bool IsNullOrEmpty(this string s)
        {
            return string.IsNullOrEmpty(s);
        }

        public static bool IsNullOrWhiteSpace(this string s)
        {
            return string.IsNullOrWhiteSpace(s);
        }

        public static bool RegexContains(this string input, string item, string split)
        {
            return Regex.IsMatch(input, "(^|" + split + ")" + item + "(" + split + "|$)");
        } 
        #endregion

        #region object
        public static bool IsNull(this object o)
        {
            return o == null;
        }
        #endregion

        #region Class
        public static Version Add(this Version v, int major, int minor, int build, int revision)
        {
            return new Version(Math.Max(v.Major, 0) + major, Math.Max(v.Minor, 0) + minor, Math.Max(v.Build, 0) + build, Math.Max(v.Revision, 0) + revision);
        }

        public static T EnsureNotNull<T>(this T t) where T : class
        {
            return t ?? Activator.CreateInstance<T>();
        }
        #endregion
    }
}

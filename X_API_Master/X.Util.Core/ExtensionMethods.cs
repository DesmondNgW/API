using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace X.Util.Core
{
    public static class ExtensionMethods
    {
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
            return JsonConvert.DeserializeObject<T>(json, new JsonSerializerSettings { MaxDepth = int.MaxValue });
        }

        public static object FromJson(this string json, Type T)
        {
            return JsonConvert.DeserializeObject(json, T, new JsonSerializerSettings { MaxDepth = int.MaxValue });
        }

        public static string ToJson(this object t)
        {
            return JsonConvert.SerializeObject(t, new JsonSerializerSettings { MaxDepth = int.MaxValue });
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

        public static bool RegexContains(this string input, string item, string split)
        {
            return Regex.IsMatch(input, "(^|" + split + ")" + item + "(" + split + "|$)");
        }

        public static string FloorString(this decimal d, int length)
        {
            return (Math.Floor(d * (long)Math.Pow(10, length)) / (long)Math.Pow(10, length)).ToString("N" + length);
        }

        public static string CeilingString(this decimal d, int length)
        {
            return (Math.Ceiling(d * (long)Math.Pow(10, length)) / (long)Math.Pow(10, length)).ToString("N" + length);
        }

        public static Version Add(this Version v, int major, int minor, int build, int revision)
        {
            return new Version(Math.Max(v.Major, 0) + major, Math.Max(v.Minor, 0) + minor, Math.Max(v.Build, 0) + build, Math.Max(v.Revision, 0) + revision);
        }
    }
}

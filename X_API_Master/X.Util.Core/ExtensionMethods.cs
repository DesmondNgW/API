using System;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
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
            if (json.IsNullOrEmpty()) return default(T);
            try
            {
                return JsonConvert.DeserializeObject<T>(json, new JsonSerializerSettings { MaxDepth = int.MaxValue, DateTimeZoneHandling = DateTimeZoneHandling.Local });
            }
            catch (Exception e)
            {
                Logger.Error(MethodBase.GetCurrentMethod(), LogDomain.Util, null, string.Empty, e.ToString());
            }
            return default(T);
        }

        public static object FromJson(this string json, Type T)
        {
            if (json.IsNullOrEmpty()) return null;
            try
            {
                return JsonConvert.DeserializeObject(json, T, new JsonSerializerSettings { MaxDepth = int.MaxValue, DateTimeZoneHandling = DateTimeZoneHandling.Local });
            }
            catch (Exception e)
            {
                Logger.Error(MethodBase.GetCurrentMethod(), LogDomain.Util, null, string.Empty, e.ToString());
            }
            return null;
        }

        public static string ToJson(this object t)
        {
            return t == null ? string.Empty : JsonConvert.SerializeObject(t, new JsonSerializerSettings { MaxDepth = int.MaxValue, DateTimeZoneHandling = DateTimeZoneHandling.Local });
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

        #region byte
        public static string FromUtf8Bytes(this byte[] b)
        {
            return b != null ? Encoding.UTF8.GetString(b, 0, b.Length) : null;
        }

        public static string FromAsciiBytes(this byte[] b)
        {
            return b != null ? Encoding.ASCII.GetString(b, 0, b.Length) : null;
        }

        public static string FromDefaultBytes(this byte[] b)
        {
            return b != null ? Encoding.Default.GetString(b, 0, b.Length) : null;
        }

        public static string Bytes2Base64(this byte[] bytes)
        {
            var base64ArraySize = (int)Math.Ceiling(bytes.Length / 3d) * 4;
            var charBuffer = new char[base64ArraySize];
            Convert.ToBase64CharArray(bytes, 0, bytes.Length, charBuffer, 0);
            return new string(charBuffer);
        }

        public static string Bytes2Hex(this byte[] bytes)
        {
            var hexString = new StringBuilder();
            if (Equals(bytes, null)) return hexString.ToString();
            foreach (var t in bytes) hexString.Append(t.ToString("x2"));
            return hexString.ToString();
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

        public static string Replace(this string s, string pattern, string value)
        {
            return new Regex(pattern).Replace(s, m => value);
        }

        public static string Replace(this string s, string pattern, Func<Match, string> match)
        {
            return new Regex(pattern).Replace(s, m => match(m));
        }

        public static byte[] ToUtf8Bytes(this string s)
        {
            return Encoding.UTF8.GetBytes(s);
        }

        public static byte[] ToAsciiBytes(this string s)
        {
            return Encoding.ASCII.GetBytes(s);
        }

        public static byte[] ToDefaultBytes(this string s)
        {
            return Encoding.Default.GetBytes(s);
        }

        public static string ToBase64(this string s)
        {
            var binBuffer = s.ToUtf8Bytes();
            var base64ArraySize = (int)Math.Ceiling(binBuffer.Length / 3d) * 4;
            var charBuffer = new char[base64ArraySize];
            Convert.ToBase64CharArray(binBuffer, 0, binBuffer.Length, charBuffer, 0);
            return new string(charBuffer);
        }

        public static string FromBase64(this string base64)
        {
            var charBuffer = base64.ToCharArray();
            return Convert.FromBase64CharArray(charBuffer, 0, charBuffer.Length).FromUtf8Bytes();
        }

        public static byte[] Base64ToBytes(this string base64)
        {
            var charBuffer = base64.ToCharArray();
            return Convert.FromBase64CharArray(charBuffer, 0, charBuffer.Length);
        }

        public static byte[] Hex2Bytes(this string s)
        {
            var l = s.Length / 2;
            var ret = new byte[l];
            for (var i = 0; i < l; i++)
            {
                var str = s.Substring(i * 2, 2);
                ret[i] = Convert.ToByte(str, 16);
            }
            return ret;
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

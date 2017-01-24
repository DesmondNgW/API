using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using X.Util.Core.Kernel;
using X.Util.Core.Log;
using X.Util.Entities.Enums;

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
                Logger.Client.Error(Logger.Client.GetMethodInfo(MethodBase.GetCurrentMethod(), new object[] { json }), e, LogDomain.Util);
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
                Logger.Client.Error(Logger.Client.GetMethodInfo(MethodBase.GetCurrentMethod(), new object[] { json, T }), e, LogDomain.Util);
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
            var result = mStream.GetBuffer();
            mStream.Close();
            mStream.Dispose();
            return result;
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

        public static string ReplaceRegex(this string s, string pattern, string value)
        {
            return new Regex(pattern).Replace(s, m => value);
        }

        public static string ReplaceRegex(this string s, string pattern, Func<Match, string> match)
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

        #region DateTime
        /// <summary>
        /// 1970年1月1日以来毫秒数
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static long GetMilliseconds (this DateTime dt)
        {
            return (dt.Ticks - new DateTime(1970, 1, 1).Ticks) / 10000;
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

        public static string GetDeclaringFullName(this MethodBase m)
        {
            return m.DeclaringType != null ? m.DeclaringType.FullName : "NullDeclaringType";
        }

        public static string GetFullName(this Type m)
        {
            return m != null ? m.FullName : "NullType";
        }

        public static T Save<T>(this T context, string name, object value)
        {
            if (string.IsNullOrEmpty(name)) return context;
            var props = typeof(T).GetProperties();
            foreach (var p in props.Where(p => string.Equals(p.Name, name, StringComparison.CurrentCultureIgnoreCase)))
            {
                p.SetValue(context, value, null);
            }
            return context;
        }

        public static T Save<T>(this T context, Dictionary<string, object> dictionary)
        {
            if (dictionary == null) return context;
            var props = typeof(T).GetProperties();
            dictionary = new Dictionary<string, object>(dictionary, StringComparer.OrdinalIgnoreCase);
            foreach (var p in props.Where(p => dictionary.ContainsKey(p.Name)))
            {
                p.SetValue(context, dictionary[p.Name], null);
            }
            return context;
        }
        #endregion

        #region ExecutionContext
        public static T Update<T>(this T context, string name, object value)
        {
            context = context.Save(name, value);
            CallContext.SetData("CallContext.Data." + typeof(T).FullName, context);
            return context;
        }

        public static T LogicalUpdate<T>(this T context, string name, object value)
        {
            context = context.Save(name, value);
            CallContext.LogicalSetData("CallContext.LogicalData." + typeof(T).FullName, context);
            return context;
        }

        public static T Update<T>(this T context, Dictionary<string, object> dictionary)
        {
            context = context.Save(dictionary);
            CallContext.SetData("CallContext.Data." + typeof(T).FullName, context);
            return context;
        }

        public static T LogicalUpdate<T>(this T context, Dictionary<string, object> dictionary)
        {
            context = context.Save(dictionary);
            CallContext.LogicalSetData("CallContext.LogicalData." + typeof(T).FullName, context);
            return context;
        }
        #endregion

        #region 类型转换
        public static bool Convert2Boolean(this string value)
        {
            return CoreUtil.Convert2Type(value, bool.TryParse, false);
        }

        public static bool Convert2Boolean(this string value, bool defaultValue)
        {
            return CoreUtil.Convert2Type(value, bool.TryParse, defaultValue);
        }

        public static bool? Convert2Boolean(this string value, bool? defaultValue)
        {
            return CoreUtil.Convert2Type(value, bool.TryParse, defaultValue);
        }

        public static byte Convert2Byte(this string value, byte defaultValue, NumberStyles style = NumberStyles.Integer)
        {
            return CoreUtil.Convert2Type(value, byte.TryParse, style, CultureInfo.InvariantCulture, defaultValue);
        }

        public static byte? Convert2Byte(this string value, byte? defaultValue, NumberStyles style = NumberStyles.Integer)
        {
            return CoreUtil.Convert2Type(value, byte.TryParse, style, CultureInfo.InvariantCulture, defaultValue);
        }

        public static short Convert2Int16(this string value, short defaultValue, NumberStyles style = NumberStyles.Integer)
        {
            return CoreUtil.Convert2Type(value, short.TryParse, style, CultureInfo.InvariantCulture, defaultValue);
        }

        public static short? Convert2Int16(this string value, short? defaultValue, NumberStyles style = NumberStyles.Integer)
        {
            return CoreUtil.Convert2Type(value, short.TryParse, style, CultureInfo.InvariantCulture, defaultValue);
        }

        public static int Convert2Int32(this string value, int defaultValue, NumberStyles style = NumberStyles.Integer)
        {
            return CoreUtil.Convert2Type(value, int.TryParse, style, CultureInfo.InvariantCulture, defaultValue);
        }

        public static int? Convert2Int32(this string value, int? defaultValue, NumberStyles style = NumberStyles.Integer)
        {
            return CoreUtil.Convert2Type(value, int.TryParse, style, CultureInfo.InvariantCulture, defaultValue);
        }

        public static long Convert2Int64(this string value, long defaultValue, NumberStyles style = NumberStyles.Integer)
        {
            return CoreUtil.Convert2Type(value, long.TryParse, style, CultureInfo.InvariantCulture, defaultValue);
        }

        public static long? Convert2Int64(this string value, long? defaultValue, NumberStyles style = NumberStyles.Integer)
        {
            return CoreUtil.Convert2Type(value, long.TryParse, style, CultureInfo.InvariantCulture, defaultValue);
        }

        public static double Convert2Double(this string value, double defaultValue, NumberStyles style = NumberStyles.Float)
        {
            return CoreUtil.Convert2Type(value, double.TryParse, style, CultureInfo.InvariantCulture, defaultValue);
        }

        public static double? Convert2Double(this string value, double? defaultValue, NumberStyles style = NumberStyles.Float)
        {
            return CoreUtil.Convert2Type(value, double.TryParse, style, CultureInfo.InvariantCulture, defaultValue);
        }

        public static float Convert2Single(this string value, float defaultValue, NumberStyles style = NumberStyles.Float)
        {
            return CoreUtil.Convert2Type(value, float.TryParse, style, CultureInfo.InvariantCulture, defaultValue);
        }

        public static float? Convert2Single(this string value, float? defaultValue, NumberStyles style = NumberStyles.Float)
        {
            return CoreUtil.Convert2Type(value, float.TryParse, style, CultureInfo.InvariantCulture, defaultValue);
        }

        public static decimal Convert2Decimal(this string value, decimal defaultValue, NumberStyles style = NumberStyles.Float)
        {
            return CoreUtil.Convert2Type(value, decimal.TryParse, style, CultureInfo.InvariantCulture, defaultValue);
        }

        public static decimal? Convert2Decimal(this string value, decimal? defaultValue, NumberStyles style = NumberStyles.Float)
        {
            return CoreUtil.Convert2Type(value, decimal.TryParse, style, CultureInfo.InvariantCulture, defaultValue);
        }

        public static DateTime Convert2DateTime(this string value, DateTime defaultValue)
        {
            return CoreUtil.Convert2Type(value, DateTime.TryParse, defaultValue);
        }

        public static DateTime? Convert2DateTime(this string value, DateTime? defaultValue)
        {
            return CoreUtil.Convert2Type(value, DateTime.TryParse, defaultValue);
        }

        public static DateTime Convert2DateTime(this string value, string format, DateTime defaultValue, DateTimeStyles style = DateTimeStyles.None)
        {
            return CoreUtil.Convert2Type(value, DateTime.TryParseExact, format, style, CultureInfo.InvariantCulture, defaultValue);
        }

        public static DateTime? Convert2DateTime(this string value, string format, DateTime? defaultValue, DateTimeStyles style = DateTimeStyles.None)
        {
            return CoreUtil.Convert2Type(value, DateTime.TryParseExact, format, style, CultureInfo.InvariantCulture, defaultValue);
        }
        #endregion
    }
}

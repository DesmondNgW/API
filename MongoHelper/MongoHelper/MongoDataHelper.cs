using MongoDB.Bson;
using System;

namespace MongoDbHelper
{
    #region 数据转换
    /// <summary>
    /// Mongo数据转换
    /// </summary>
    public class MongoDbDataHelper
    {
        #region BsonDocument
        public static BsonDocument GetBsonDocument(BsonDocument document, string name)
        {
            try
            {
                if (document[name].IsBsonDocument)
                {
                    return document[name] as BsonDocument;
                }
            }
            catch
            {
                // ignored
            }
            return new BsonDocument();
        }
        #endregion

        #region String
        public static string GetString(BsonDocument document, string name)
        {
            try
            {
                return !document[name].IsBsonNull ? document[name].ToString().Trim() : string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }
        public static string GetString(BsonDocument document, string name, string defaultValue)
        {
            try
            {
                return !document[name].IsBsonNull ? document[name].ToString().Trim() : defaultValue;
            }
            catch
            {
                return defaultValue;
            }
        }
        #endregion

        #region Boolean
        public static bool GetBoolean(BsonDocument document, string name)
        {
            return bool.Parse(GetString(document, name));
        }

        public static bool GetBoolean(BsonDocument document, string name, bool defaultValue)
        {
            var value = GetString(document, name, string.Empty);
            bool result;
            return bool.TryParse(value, out result) ? result : defaultValue;
        }

        public static bool? GetBoolean(BsonDocument document, string name, bool? defaultValue)
        {
            var value = GetString(document, name, string.Empty);
            bool result;
            return bool.TryParse(value, out result) ? result : defaultValue;
        }
        #endregion

        #region Byte
        public static byte GetByte(BsonDocument document, string name, byte defaultValue)
        {
            var value = GetString(document, name, string.Empty);
            byte result;
            return byte.TryParse(value, out result) ? result : defaultValue;
        }
        public static byte? GetByte(BsonDocument document, string name, byte? defaultValue)
        {
            var value = GetString(document, name, string.Empty);
            byte result;
            return byte.TryParse(value, out result) ? result : defaultValue;
        }
        #endregion

        #region Int16/Short
        public static short GetInt16(BsonDocument document, string name, short defaultValue)
        {
            var value = GetString(document, name, string.Empty);
            short result;
            return short.TryParse(value, out result) ? result : defaultValue;
        }
        public static short? GetInt16(BsonDocument document, string name, short? defaultValue)
        {
            var value = GetString(document, name, string.Empty);
            short result;
            return short.TryParse(value, out result) ? result : defaultValue;
        }
        #endregion

        #region Int32
        public static int GetInt32(BsonDocument document, string name, int defaultValue)
        {
            var value = GetString(document, name, string.Empty);
            int result;
            return int.TryParse(value, out result) ? result : defaultValue;
        }
        public static int? GetInt32(BsonDocument document, string name, int? defaultValue)
        {
            var value = GetString(document, name, string.Empty);
            int result;
            return int.TryParse(value, out result) ? result : defaultValue;
        }
        #endregion

        #region Int64
        public static long GetInt64(BsonDocument document, string name, long defaultValue)
        {
            var value = GetString(document, name, string.Empty);
            long result;
            return long.TryParse(value, out result) ? result : defaultValue;
        }
        public static long? GetInt64(BsonDocument document, string name, long? defaultValue)
        {
            var value = GetString(document, name, string.Empty);
            long result;
            return long.TryParse(value, out result) ? result : defaultValue;
        }
        #endregion

        #region Double
        public static double GetDouble(BsonDocument document, string name, double defaultValue)
        {
            var value = GetString(document, name, string.Empty);
            double result;
            return double.TryParse(value, out result) ? result : defaultValue;
        }
        public static double? GetDouble(BsonDocument document, string name, double? defaultValue)
        {
            var value = GetString(document, name, string.Empty);
            double result;
            return double.TryParse(value, out result) ? result : defaultValue;
        }
        #endregion

        #region Decimal
        public static decimal GetDecimal(BsonDocument document, string name, decimal defaultValue)
        {
            var value = GetString(document, name, string.Empty);
            decimal result;
            return decimal.TryParse(value, out result) ? result : defaultValue;
        }
        public static decimal? GetDecimal(BsonDocument document, string name, decimal? defaultValue)
        {
            var value = GetString(document, name, string.Empty);
            decimal result;
            return decimal.TryParse(value, out result) ? result : defaultValue;
        }
        #endregion

        #region DateTime
        public static DateTime GetDateTime(BsonDocument document, string name, DateTime defaultValue)
        {
            var value = GetString(document, name, string.Empty);
            DateTime result;
            return DateTime.TryParse(value, out result) ? result : defaultValue;
        }
        public static DateTime? GetDateTime(BsonDocument document, string name, DateTime? defaultValue)
        {
            var value = GetString(document, name, string.Empty);
            DateTime result;
            return DateTime.TryParse(value, out result) ? result : defaultValue;
        }
        #endregion

        #region EnumValue        
        /// <summary>
        /// 根据枚举值返回枚举对象
        /// </summary>
        /// <typeparam name="T">枚举对象</typeparam>
        /// <param name="document">BsonDocument</param>
        /// <param name="name">字段名</param>
        /// <param name="defaultValue">默认枚举对象</param>
        /// <returns>
        /// 枚举实例
        /// </returns>
        public static T GetEnumValue<T>(BsonDocument document, string name, T defaultValue)
        {
            var val = GetInt32(document, name, null);
            if (!val.HasValue)
            {
                return defaultValue;
            }
            try
            {
                return (T)Enum.ToObject(typeof(T), val.Value);
            }
            catch //(Exception ex)
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// 根据枚举名返回枚举对象
        /// </summary>
        /// <typeparam name="T">枚举对象</typeparam>
        /// <param name="document">BsonDocument</param>
        /// <param name="name">字段名</param>
        /// <param name="defaultValue">默认枚举对象</param>
        /// <returns>
        /// 枚举实例
        /// </returns>
        public static T GetEnumString<T>(BsonDocument document, string name, T defaultValue)
        {
            var val = GetString(document, name);
            if (string.IsNullOrEmpty(val))
            {
                return defaultValue;
            }
            try
            {
                return (T)Enum.Parse(typeof(T), val);
            }
            catch //(Exception ex)
            {
                return defaultValue;
            }
        }
        #endregion

    }
    #endregion
}

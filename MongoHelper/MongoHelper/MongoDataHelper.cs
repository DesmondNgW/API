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
                return document[name].IsBsonDocument ? (BsonDocument)document[name] : new BsonDocument();
            }
            catch
            {
                return new BsonDocument();
            }
        }
        #endregion

        #region String
        public static string GetString(BsonValue value)
        {
            try
            {
                return !value.IsBsonNull ? value.ToString().Trim() : string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }

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
        public static bool GetBoolean(BsonValue value)
        {
            bool result;
            return bool.TryParse(GetString(value), out result) && result;
        }

        public static bool GetBoolean(BsonDocument document, string name)
        {
            bool result;
            return bool.TryParse(GetString(document, name), out result) && result;
        }

        public static bool GetBoolean(BsonValue value, bool defaultValue)
        {
            bool result;
            return bool.TryParse(GetString(value), out result) ? result : defaultValue;
        }

        public static bool GetBoolean(BsonDocument document, string name, bool defaultValue)
        {
            bool result;
            return bool.TryParse(GetString(document, name, string.Empty), out result) ? result : defaultValue;
        }

        public static bool? GetBoolean(BsonValue value, bool? defaultValue)
        {
            bool result;
            return bool.TryParse(GetString(value), out result) ? result : defaultValue;
        }

        public static bool? GetBoolean(BsonDocument document, string name, bool? defaultValue)
        {
            bool result;
            return bool.TryParse(GetString(document, name, string.Empty), out result) ? result : defaultValue;
        }
        #endregion

        #region Byte
        public static byte GetByte(BsonValue value, byte defaultValue)
        {
            byte result;
            return byte.TryParse(GetString(value), out result) ? result : defaultValue;
        }

        public static byte? GetByte(BsonValue value, byte? defaultValue)
        {
            byte result;
            return byte.TryParse(GetString(value), out result) ? result : defaultValue;
        }

        public static byte GetByte(BsonDocument document, string name, byte defaultValue)
        {
            byte result;
            return byte.TryParse(GetString(document, name, string.Empty), out result) ? result : defaultValue;
        }
        public static byte? GetByte(BsonDocument document, string name, byte? defaultValue)
        {
            byte result;
            return byte.TryParse(GetString(document, name, string.Empty), out result) ? result : defaultValue;
        }

        #endregion

        #region Int16/Short
        public static short GetInt16(BsonValue value, short defaultValue)
        {
            short result;
            return short.TryParse(GetString(value), out result) ? result : defaultValue;
        }

        public static short? GetInt16(BsonValue value, short? defaultValue)
        {
            short result;
            return short.TryParse(GetString(value), out result) ? result : defaultValue;
        }

        public static short GetInt16(BsonDocument document, string name, short defaultValue)
        {
            short result;
            return short.TryParse(GetString(document, name, string.Empty), out result) ? result : defaultValue;
        }

        public static short? GetInt16(BsonDocument document, string name, short? defaultValue)
        {
            short result;
            return short.TryParse(GetString(document, name, string.Empty), out result) ? result : defaultValue;
        }
        #endregion

        #region Int32
        public static int GetInt32(BsonValue value, int defaultValue)
        {
            int result;
            return int.TryParse(GetString(value), out result) ? result : defaultValue;
        }

        public static int? GetInt32(BsonValue value, int? defaultValue)
        {
            int result;
            return int.TryParse(GetString(value), out result) ? result : defaultValue;
        }

        public static int GetInt32(BsonDocument document, string name, int defaultValue)
        {
            int result;
            return int.TryParse(GetString(document, name, string.Empty), out result) ? result : defaultValue;
        }

        public static int? GetInt32(BsonDocument document, string name, int? defaultValue)
        {
            int result;
            return int.TryParse(GetString(document, name, string.Empty), out result) ? result : defaultValue;
        }
        #endregion

        #region Int64
        public static long GetInt64(BsonValue value, long defaultValue)
        {
            long result;
            return long.TryParse(GetString(value), out result) ? result : defaultValue;
        }

        public static long? GetInt64(BsonValue value, long? defaultValue)
        {
            long result;
            return long.TryParse(GetString(value), out result) ? result : defaultValue;
        }

        public static long GetInt64(BsonDocument document, string name, long defaultValue)
        {
            long result;
            return long.TryParse(GetString(document, name, string.Empty), out result) ? result : defaultValue;
        }

        public static long? GetInt64(BsonDocument document, string name, long? defaultValue)
        {
            long result;
            return long.TryParse(GetString(document, name, string.Empty), out result) ? result : defaultValue;
        }
        #endregion

        #region Double
        public static double GetDouble(BsonValue value, double defaultValue)
        {
            double result;
            return double.TryParse(GetString(value), out result) ? result : defaultValue;
        }

        public static double? GetDouble(BsonValue value, double? defaultValue)
        {
            double result;
            return double.TryParse(GetString(value), out result) ? result : defaultValue;
        }

        public static double GetDouble(BsonDocument document, string name, double defaultValue)
        {
            double result;
            return double.TryParse(GetString(document, name, string.Empty), out result) ? result : defaultValue;
        }

        public static double? GetDouble(BsonDocument document, string name, double? defaultValue)
        {
            double result;
            return double.TryParse(GetString(document, name, string.Empty), out result) ? result : defaultValue;
        }
        #endregion

        #region Decimal
        public static decimal GetDecimal(BsonValue value, decimal defaultValue)
        {
            decimal result;
            return decimal.TryParse(GetString(value), out result) ? result : defaultValue;
        }

        public static decimal? GetDecimal(BsonValue value, decimal? defaultValue)
        {
            decimal result;
            return decimal.TryParse(GetString(value), out result) ? result : defaultValue;
        }

        public static decimal GetDecimal(BsonDocument document, string name, decimal defaultValue)
        {
            decimal result;
            return decimal.TryParse(GetString(document, name, string.Empty), out result) ? result : defaultValue;
        }

        public static decimal? GetDecimal(BsonDocument document, string name, decimal? defaultValue)
        {
            decimal result;
            return decimal.TryParse(GetString(document, name, string.Empty), out result) ? result : defaultValue;
        }
        #endregion

        #region DateTime
        public static DateTime GetDateTime(BsonValue value, DateTime defaultValue)
        {
            DateTime result;
            return DateTime.TryParse(GetString(value), out result) ? result : defaultValue;
        }

        public static DateTime? GetDateTime(BsonValue value, DateTime? defaultValue)
        {
            DateTime result;
            return DateTime.TryParse(GetString(value), out result) ? result : defaultValue;
        }

        public static DateTime GetDateTime(BsonDocument document, string name, DateTime defaultValue)
        {
            DateTime result;
            return DateTime.TryParse(GetString(document, name, string.Empty), out result) ? result : defaultValue;
        }

        public static DateTime? GetDateTime(BsonDocument document, string name, DateTime? defaultValue)
        {
            DateTime result;
            return DateTime.TryParse(GetString(document, name, string.Empty), out result) ? result : defaultValue;
        }
        #endregion

        #region EnumValue        
        public static T GetEnum<T>(BsonValue value, T defaultValue) where T : struct 
        {
            T result;
            return Enum.TryParse(GetString(value), out result) ? result : defaultValue;
        }

        public static T GetEnum<T>(BsonDocument document, string name, T defaultValue) where T : struct
        {
            T result;
            return Enum.TryParse(GetString(document, name, string.Empty), out result) ? result : defaultValue;
        }
        #endregion
    }
    #endregion
}

﻿using MongoDB.Bson;
using System;
using X.Util.Core;

namespace X.Util.Extend.Mongo
{
    public class MongoDataHelper
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
            return CoreParse.GetBoolean(GetString(value));
        }
        public static bool GetBoolean(BsonValue value, bool defaultValue)
        {
            return CoreParse.GetBoolean(GetString(value), defaultValue);
        }
        public static bool? GetBoolean(BsonValue value, bool? defaultValue)
        {
            return CoreParse.GetBoolean(GetString(value), defaultValue);
        }
        public static bool GetBoolean(BsonDocument document, string name)
        {
            return CoreParse.GetBoolean(GetString(document, name));
        }
        public static bool GetBoolean(BsonDocument document, string name, bool defaultValue)
        {
            return CoreParse.GetBoolean(GetString(document, name, string.Empty), defaultValue);
        }
        public static bool? GetBoolean(BsonDocument document, string name, bool? defaultValue)
        {
            return CoreParse.GetBoolean(GetString(document, name, string.Empty), defaultValue);
        }
        #endregion

        #region Byte
        public static byte GetByte(BsonValue value, byte defaultValue)
        {
            return CoreParse.GetByte(GetString(value), defaultValue);
        }
        public static byte? GetByte(BsonValue value, byte? defaultValue)
        {
            return CoreParse.GetByte(GetString(value), defaultValue);
        }
        public static byte GetByte(BsonDocument document, string name, byte defaultValue)
        {
            return CoreParse.GetByte(GetString(document, name, string.Empty), defaultValue);
        }
        public static byte? GetByte(BsonDocument document, string name, byte? defaultValue)
        {
            return CoreParse.GetByte(GetString(document, name, string.Empty), defaultValue);
        }
        #endregion

        #region Int16/Short
        public static short GetInt16(BsonValue value, short defaultValue)
        {
            return CoreParse.GetInt16(GetString(value), defaultValue);
        }
        public static short? GetInt16(BsonValue value, short? defaultValue)
        {
            return CoreParse.GetInt16(GetString(value), defaultValue);
        }
        public static short GetInt16(BsonDocument document, string name, short defaultValue)
        {
            return CoreParse.GetInt16(GetString(document, name, string.Empty), defaultValue);
        }
        public static short? GetInt16(BsonDocument document, string name, short? defaultValue)
        {
            return CoreParse.GetInt16(GetString(document, name, string.Empty), defaultValue);
        }
        #endregion

        #region Int32
        public static int GetInt32(BsonValue value, int defaultValue)
        {
            return CoreParse.GetInt32(GetString(value), defaultValue);
        }
        public static int? GetInt32(BsonValue value, int? defaultValue)
        {
            return CoreParse.GetInt32(GetString(value), defaultValue);
        }
        public static int GetInt32(BsonDocument document, string name, int defaultValue)
        {
            return CoreParse.GetInt32(GetString(document, name, string.Empty), defaultValue);
        }
        public static int? GetInt32(BsonDocument document, string name, int? defaultValue)
        {
            return CoreParse.GetInt32(GetString(document, name, string.Empty), defaultValue);
        }
        #endregion

        #region Int64
        public static long GetInt64(BsonValue value, long defaultValue)
        {
            return CoreParse.GetInt64(GetString(value), defaultValue);
        }
        public static long? GetInt64(BsonValue value, long? defaultValue)
        {
            return CoreParse.GetInt64(GetString(value), defaultValue);
        }
        public static long GetInt64(BsonDocument document, string name, long defaultValue)
        {
            return CoreParse.GetInt64(GetString(document, name, string.Empty), defaultValue);
        }
        public static long? GetInt64(BsonDocument document, string name, long? defaultValue)
        {
            return CoreParse.GetInt64(GetString(document, name, string.Empty), defaultValue);
        }
        #endregion

        #region Single
        public static float GetSingle(BsonValue value, float defaultValue)
        {
            return CoreParse.GetSingle(GetString(value), defaultValue);
        }
        public static float? GetSingle(BsonValue value, float? defaultValue)
        {
            return CoreParse.GetSingle(GetString(value), defaultValue);
        }
        public static float GetSingle(BsonDocument document, string name, float defaultValue)
        {
            return CoreParse.GetSingle(GetString(document, name, string.Empty), defaultValue);
        }
        public static float? GetSingle(BsonDocument document, string name, float? defaultValue)
        {
            return CoreParse.GetSingle(GetString(document, name, string.Empty), defaultValue);
        }
        #endregion

        #region Double
        public static double GetDouble(BsonValue value, double defaultValue)
        {
            return CoreParse.GetDouble(GetString(value), defaultValue);
        }
        public static double? GetDouble(BsonValue value, double? defaultValue)
        {
            return CoreParse.GetDouble(GetString(value), defaultValue);
        }
        public static double GetDouble(BsonDocument document, string name, double defaultValue)
        {
            return CoreParse.GetDouble(GetString(document, name, string.Empty), defaultValue);
        }
        public static double? GetDouble(BsonDocument document, string name, double? defaultValue)
        {
            return CoreParse.GetDouble(GetString(document, name, string.Empty), defaultValue);
        }
        #endregion

        #region Decimal
        public static decimal GetDecimal(BsonValue value, decimal defaultValue)
        {
            return CoreParse.GetDecimal(GetString(value), defaultValue);
        }
        public static decimal? GetDecimal(BsonValue value, decimal? defaultValue)
        {
            return CoreParse.GetDecimal(GetString(value), defaultValue);
        }
        public static decimal GetDecimal(BsonDocument document, string name, decimal defaultValue)
        {
            return CoreParse.GetDecimal(GetString(document, name, string.Empty), defaultValue);
        }
        public static decimal? GetDecimal(BsonDocument document, string name, decimal? defaultValue)
        {
            return CoreParse.GetDecimal(GetString(document, name, string.Empty), defaultValue);
        }
        #endregion

        #region DateTime
        public static DateTime GetDateTime(BsonValue value, DateTime defaultValue)
        {
            return CoreParse.GetDateTime(GetString(value), defaultValue);
        }
        public static DateTime? GetDateTime(BsonValue value, DateTime? defaultValue)
        {
            return CoreParse.GetDateTime(GetString(value), defaultValue);
        }
        public static DateTime GetDateTime(BsonDocument document, string name, DateTime defaultValue)
        {
            return CoreParse.GetDateTime(GetString(document, name, string.Empty), defaultValue);
        }
        public static DateTime? GetDateTime(BsonDocument document, string name, DateTime? defaultValue)
        {
            return CoreParse.GetDateTime(GetString(document, name, string.Empty), defaultValue);
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
}

using System;
using System.Globalization;

namespace X.Util.Core
{
    public class CoreParse
    {
        #region Boolean
        public static bool GetBoolean(string value)
        {
            bool result;
            return bool.TryParse(value, out result) && result;
        }

        public static bool GetBoolean(string value, bool defaultValue)
        {
            bool result;
            return bool.TryParse(value, out result) ? result : defaultValue;
        }

        public static bool? GetBoolean(string value, bool? defaultValue)
        {
            bool result;
            return bool.TryParse(value, out result) ? result : defaultValue;
        }
        #endregion

        #region Byte
        public static byte GetByte(string value, byte defaultValue, NumberStyles style = NumberStyles.Integer)
        {
            byte result;
            return byte.TryParse(value, style, CultureInfo.InvariantCulture, out result) ? result : defaultValue;
        }

        public static byte? GetByte(string value, byte? defaultValue, NumberStyles style = NumberStyles.Integer)
        {
            byte result;
            return byte.TryParse(value, style, CultureInfo.InvariantCulture, out result) ? result : defaultValue;
        }
        #endregion

        #region Int16
        public static short GetInt16(string value, short defaultValue, NumberStyles style = NumberStyles.Integer)
        {
            short result;
            return short.TryParse(value, style, CultureInfo.InvariantCulture, out result) ? result : defaultValue;
        }

        public static short? GetInt16(string value, short? defaultValue, NumberStyles style = NumberStyles.Integer)
        {
            short result;
            return short.TryParse(value, style, CultureInfo.InvariantCulture, out result) ? result : defaultValue;
        }
        #endregion

        #region Int32
        public static int GetInt32(string value, int defaultValue, NumberStyles style = NumberStyles.Integer)
        {
            int result;
            return int.TryParse(value, style, CultureInfo.InvariantCulture, out result) ? result : defaultValue;
        }

        public static int? GetInt32(string value, int? defaultValue, NumberStyles style = NumberStyles.Integer)
        {
            int result;
            return int.TryParse(value, style, CultureInfo.InvariantCulture, out result) ? result : defaultValue;
        }
        #endregion

        #region Int64
        public static long GetInt64(string value, long defaultValue, NumberStyles style = NumberStyles.Integer)
        {
            long result;
            return long.TryParse(value, style, CultureInfo.InvariantCulture, out result) ? result : defaultValue;
        }

        public static long? GetInt64(string value, long? defaultValue, NumberStyles style = NumberStyles.Integer)
        {
            long result;
            return long.TryParse(value, style, CultureInfo.InvariantCulture, out result) ? result : defaultValue;
        }
        #endregion

        #region Double
        public static double GetDouble(string value, double defaultValue, NumberStyles style = NumberStyles.Float)
        {
            double result;
            return double.TryParse(value, style, CultureInfo.InvariantCulture, out result) ? result : defaultValue;
        }

        public static double? GetDouble(string value, double? defaultValue, NumberStyles style = NumberStyles.Float)
        {
            double result;
            return double.TryParse(value, style, CultureInfo.InvariantCulture, out result) ? result : defaultValue;
        }
        #endregion

        #region Single
        public static float GetSingle(string value, float defaultValue, NumberStyles style = NumberStyles.Float)
        {
            float result;
            return float.TryParse(value, style, CultureInfo.InvariantCulture, out result) ? result : defaultValue;
        }

        public static float? GetSingle(string value, float? defaultValue, NumberStyles style = NumberStyles.Float)
        {
            float result;
            return float.TryParse(value, style, CultureInfo.InvariantCulture, out result) ? result : defaultValue;
        }
        #endregion

        #region Decimal
        public static decimal GetDecimal(string value, decimal defaultValue, NumberStyles style = NumberStyles.Float)
        {
            decimal result;
            return decimal.TryParse(value, style, CultureInfo.InvariantCulture, out result) ? result : defaultValue;
        }

        public static decimal? GetDecimal(string value, decimal? defaultValue, NumberStyles style = NumberStyles.Float)
        {
            decimal result;
            return decimal.TryParse(value, style, CultureInfo.InvariantCulture, out result) ? result : defaultValue;
        }
        #endregion

        #region DateTime
        public static DateTime GetDateTime(string value, DateTime defaultValue)
        {
            DateTime result;
            return DateTime.TryParse(value, out result) ? result : defaultValue;
        }

        public static DateTime? GetDateTime(string value, DateTime? defaultValue)
        {
            DateTime result;
            return DateTime.TryParse(value, out result) ? result : defaultValue;
        }

        public static DateTime GetDateTime(string value, string format, DateTime defaultValue, DateTimeStyles style = DateTimeStyles.None)
        {
            DateTime result;
            return DateTime.TryParseExact(value, format, CultureInfo.InvariantCulture, style, out result) ? result : defaultValue;
        }

        public static DateTime? GetDateTime(string value, string format, DateTime? defaultValue, DateTimeStyles style = DateTimeStyles.None)
        {
            DateTime result;
            return DateTime.TryParseExact(value, format, CultureInfo.InvariantCulture, style, out result) ? result : defaultValue;
        }
        #endregion
    }
}

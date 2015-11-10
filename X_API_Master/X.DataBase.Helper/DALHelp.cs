using System;
using System.Data.SqlClient;

namespace X.DataBase.Helper
{
    public class DalHelper
    {
        #region AppConfig中须配置对应的数据库连接
        protected static DbHelper DbHelper = GetHelper("XDataBase");

        /// <summary>
        /// 从Web.config从读取数据库的连接以及数据库类型
        ///</summary>
        private static DbHelper GetHelper(string connectionStringName)
        {
            var dbHelper = new DbHelper();
            var conn = System.Configuration.ConfigurationManager.ConnectionStrings[connectionStringName];
            // 从Web.config中读取数据库类型
            var providerName = conn.ProviderName;
            switch (providerName)
            {
                case "Oracle.DataAccess.Client":
                    dbHelper.DatabaseType = DbHelper.DatabaseTypes.Oracle;
                    break;
                case "MySql.Data.MySqlClient":
                    dbHelper.DatabaseType = DbHelper.DatabaseTypes.MySql;
                    break;
                case "System.Data.OleDb":
                    dbHelper.DatabaseType = DbHelper.DatabaseTypes.OleDb;
                    break;
                case "System.Data.SQLite":
                    dbHelper.DatabaseType = DbHelper.DatabaseTypes.SqLite;
                    break;
                default:
                    dbHelper.DatabaseType = DbHelper.DatabaseTypes.Sql;
                    break;
            }
            // 从Web.config中读取数据库连接
            dbHelper.ConnectionString = conn.ConnectionString;
            return dbHelper;
        }
        #endregion

        #region 由Object取值
        /// <summary>
        /// 取得Int16值
        /// </summary>
        public static short? GetInt16(object obj)
        {
            short result;
            return (obj != null && obj != DBNull.Value && short.TryParse(obj.ToString(), out result)) ? (short?)result : null;
        }

        /// <summary>
        /// 取得UInt16值
        /// </summary>
        public static UInt16? GetUInt16(object obj)
        {
            UInt16 result;
            return (obj != null && obj != DBNull.Value && UInt16.TryParse(obj.ToString(), out result)) ? (UInt16?)result : null;
        }

        /// <summary>
        /// 取得Int值
        /// </summary>
        public static int? GetInt(object obj)
        {
            int result;
            return (obj != null && obj != DBNull.Value && int.TryParse(obj.ToString(), out result)) ? (int?)result : null;
        }

        /// <summary>
        /// 取得UInt值
        /// </summary>
        public static UInt32? GetUInt(object obj)
        {
            UInt32 result;
            return (obj != null && obj != DBNull.Value && UInt32.TryParse(obj.ToString(), out result)) ? (UInt32?)result : null;
        }

        /// <summary>
        /// 取得Int64值
        /// </summary>
        public static long? GetLong(object obj)
        {
            long result;
            return (obj != null && obj != DBNull.Value && long.TryParse(obj.ToString(), out result)) ? (long?)result : null;
        }

        /// <summary>
        /// 取得UInt64值
        /// </summary>
        public static UInt64? GetULong(object obj)
        {
            UInt64 result;
            return (obj != null && obj != DBNull.Value && UInt64.TryParse(obj.ToString(), out result)) ? (UInt64?)result : null;
        }

        /// <summary>
        /// 取得byte值
        /// </summary>
        public static byte? GetByte(object obj)
        {
            byte result;
            return (obj != null && obj != DBNull.Value && byte.TryParse(obj.ToString(), out result)) ? (byte?)result : null;
        }

        /// <summary>
        /// 取得sbyte值
        /// </summary>
        public static SByte? GetSByte(object obj)
        {
            SByte result;
            return (obj != null && obj != DBNull.Value && SByte.TryParse(obj.ToString(), out result)) ? (SByte?)result : null;
        }

        /// <summary>
        /// 取得Decimal值
        /// </summary>
        public static decimal? GetDecimal(object obj)
        {
            decimal result;
            return (obj != null && obj != DBNull.Value && decimal.TryParse(obj.ToString(), out result)) ? (decimal?)result : null;
        }

        /// <summary>
        /// 取得float值
        /// </summary>
        public static float? GetFloat(object obj)
        {
            float result;
            return (obj != null && obj != DBNull.Value && float.TryParse(obj.ToString(), out result)) ? (float?)result : null;
        }

        /// <summary>
        /// 取得double值
        /// </summary>
        public static double? GetDouble(object obj)
        {
            double result;
            return (obj != null && obj != DBNull.Value && double.TryParse(obj.ToString(), out result)) ? (double?)result : null;
        }

        /// <summary>
        /// 取得Guid值
        /// </summary>
        public static Guid? GetGuid(object obj)
        {
            Guid result;
            return (obj != null && obj != DBNull.Value && Guid.TryParse(obj.ToString(), out result)) ? (Guid?)result : null;
        }

        /// <summary>
        /// 取得DateTime值
        /// </summary>
        public static DateTime? GetDateTime(object obj)
        {
            DateTime result;
            return (obj != null && obj != DBNull.Value && DateTime.TryParse(obj.ToString(), out result)) ? (DateTime?)result : null;
        }

        /// <summary>
        /// 取得bool值
        /// </summary>
        public static bool? GetBool(object obj)
        {
            bool result;
            return (obj != null && obj != DBNull.Value && bool.TryParse(obj.ToString(), out result)) ? (bool?)result : null;
        }

        /// <summary>
        /// 取得byte[]
        /// </summary>
        public static byte[] GetBinary(object obj)
        {
            return (obj != null && obj != DBNull.Value) ? (byte[])obj : null;
        }

        /// <summary>
        /// 取得string值
        /// </summary>
        public static string GetString(object obj)
        {
            return (obj != null && obj != DBNull.Value) ? obj.ToString() : null;
        }
        #endregion

        #region transaction
        public SqlTransaction BeginTransaction()
        {
            var connection = new SqlConnection(DbHelper.ConnectionString);
            connection.Open();
            return connection.BeginTransaction();
        }

        public void RollbackTransaction(SqlTransaction transaction)
        {
            if (Equals(transaction, null)) return;
            var connection = transaction.Connection;
            transaction.Rollback();
            transaction.Dispose();
            connection.Close();
        }

        public void CommitTransaction(SqlTransaction transaction)
        {
            if (Equals(transaction, null)) return;
            var connection = transaction.Connection;
            transaction.Commit();
            transaction.Dispose();
            connection.Close();
        }
        #endregion
    }
}

﻿using System;
using System.Data.SqlClient;

namespace X.DB.Util
{
    public class DalBase
    {
        #region AppConfig中须配置对应的数据库连接
        public static DbBase DbBase = GetHelper("XDataBase");

        /// <summary>
        /// 从Web.config从读取数据库的连接以及数据库类型
        ///</summary>
        private static DbBase GetHelper(string connectionStringName)
        {
            var dbHelper = new DbBase();
            var conn = System.Configuration.ConfigurationManager.ConnectionStrings[connectionStringName];
            // 从Web.config中读取数据库类型
            var providerName = conn.ProviderName;
            switch (providerName)
            {
                case "Oracle.DataAccess.Client":
                    dbHelper.DatabaseType = DbBase.DatabaseTypes.Oracle;
                    break;
                case "MySql.Data.MySqlClient":
                    dbHelper.DatabaseType = DbBase.DatabaseTypes.MySql;
                    break;
                case "System.Data.OleDb":
                    dbHelper.DatabaseType = DbBase.DatabaseTypes.OleDb;
                    break;
                case "System.Data.SQLite":
                    dbHelper.DatabaseType = DbBase.DatabaseTypes.SqLite;
                    break;
                default:
                    dbHelper.DatabaseType = DbBase.DatabaseTypes.Sql;
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
            return (obj != null && obj != DBNull.Value && short.TryParse(obj.ToString(), out short result)) ? (short?)result : null;
        }

        /// <summary>
        /// 取得UInt16值
        /// </summary>
        public static ushort? GetUInt16(object obj)
        {
            return (obj != null && obj != DBNull.Value && ushort.TryParse(obj.ToString(), out ushort result)) ? (ushort?)result : null;
        }

        /// <summary>
        /// 取得Int值
        /// </summary>
        public static int? GetInt(object obj)
        {
            return (obj != null && obj != DBNull.Value && int.TryParse(obj.ToString(), out int result)) ? (int?)result : null;
        }

        /// <summary>
        /// 取得UInt值
        /// </summary>
        public static uint? GetUInt(object obj)
        {
            return (obj != null && obj != DBNull.Value && uint.TryParse(obj.ToString(), out uint result)) ? (uint?)result : null;
        }

        /// <summary>
        /// 取得Int64值
        /// </summary>
        public static long? GetLong(object obj)
        {
            return (obj != null && obj != DBNull.Value && long.TryParse(obj.ToString(), out long result)) ? (long?)result : null;
        }

        /// <summary>
        /// 取得UInt64值
        /// </summary>
        public static ulong? GetULong(object obj)
        {
            return (obj != null && obj != DBNull.Value && ulong.TryParse(obj.ToString(), out ulong result)) ? (ulong?)result : null;
        }

        /// <summary>
        /// 取得byte值
        /// </summary>
        public static byte? GetByte(object obj)
        {
            return (obj != null && obj != DBNull.Value && byte.TryParse(obj.ToString(), out byte result)) ? (byte?)result : null;
        }

        /// <summary>
        /// 取得sbyte值
        /// </summary>
        public static sbyte? GetSByte(object obj)
        {
            return (obj != null && obj != DBNull.Value && sbyte.TryParse(obj.ToString(), out sbyte result)) ? (sbyte?)result : null;
        }

        /// <summary>
        /// 取得Decimal值
        /// </summary>
        public static decimal? GetDecimal(object obj)
        {
            return (obj != null && obj != DBNull.Value && decimal.TryParse(obj.ToString(), out decimal result)) ? (decimal?)result : null;
        }

        /// <summary>
        /// 取得float值
        /// </summary>
        public static float? GetFloat(object obj)
        {
            return (obj != null && obj != DBNull.Value && float.TryParse(obj.ToString(), out float result)) ? (float?)result : null;
        }

        /// <summary>
        /// 取得double值
        /// </summary>
        public static double? GetDouble(object obj)
        {
            return (obj != null && obj != DBNull.Value && double.TryParse(obj.ToString(), out double result)) ? (double?)result : null;
        }

        public static double GetDouble(object obj, double defaultDouble)
        {
            return (obj != null && obj != DBNull.Value && double.TryParse(obj.ToString(), out double result)) ? result : defaultDouble;
        }

        /// <summary>
        /// 取得Guid值
        /// </summary>
        public static Guid? GetGuid(object obj)
        {
            return (obj != null && obj != DBNull.Value && Guid.TryParse(obj.ToString(), out Guid result)) ? (Guid?)result : null;
        }

        /// <summary>
        /// 取得DateTime值
        /// </summary>
        public static DateTime? GetDateTime(object obj)
        {
            return (obj != null && obj != DBNull.Value && DateTime.TryParse(obj.ToString(), out DateTime result)) ? (DateTime?)result : null;
        }

        /// <summary>
        /// 取得bool值
        /// </summary>
        public static bool? GetBool(object obj)
        {
            return (obj != null && obj != DBNull.Value && bool.TryParse(obj.ToString(), out bool result)) ? (bool?)result : null;
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
            var connection = new SqlConnection(DbBase.ConnectionString);
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
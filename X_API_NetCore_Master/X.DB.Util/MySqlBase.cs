﻿using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Data.Common;
using System.Reflection;
using System.Text;
using X.Util.Core.Log;
using X.Util.Entities;

namespace X.DB.Util
{
    /// <summary>
    /// 数据库操作基类(for MySql)
    /// </summary>
    internal class MySqlBase : IDbBase
    {
        /// <summary>
        /// 获取分页SQL
        /// </summary>
        /// <param name="tblName">表名</param>
        /// <param name="fldSort">排序字段，例如：id asc或简写id，可写多个字段（如果是可重复字段建议最后带上主键，例如name desc,id）</param>
        /// <param name="condition">条件(不需要where)</param>
        /// <param name="start">起始索引，如每页10条则第1页的起始索引为0，第2页的起始索引为10</param>
        /// <param name="count">要取得的数据条数</param>
        /// <returns>返回用于分页的SQL语句</returns>
        private static string GetPagerSql(string tblName, string fldSort, string condition, int start, int count)
        {
            var strSql = new StringBuilder("select * from " + tblName);
            if (!string.IsNullOrEmpty(condition))
            {
                strSql.AppendFormat(" where {0} order by {1}", condition, fldSort);
            }
            else
            {
                strSql.AppendFormat(" order by {0}", fldSort);
            }
            strSql.AppendFormat(" limit {0},{1}", start, count);
            return strSql.ToString();
        }

        /// <summary>
        /// 分页获取数据
        /// </summary>
        /// <param name="connectionString">连接字符串</param>
        /// <param name="tblName">表名</param>
        /// <param name="fldSort">排序字段，例如：id asc或简写id，可写多个字段（如果是可重复字段建议最后带上主键，例如name desc,id）</param>
        /// <param name="condition">条件(不需要where)</param>
        /// <param name="start">起始索引，如每页10条则第1页的起始索引为0，第2页的起始索引为10</param>
        /// <param name="count">要取得的数据条数</param>
        public DbDataReader GetPageList(string connectionString, string tblName, string fldSort, string condition, int start, int count)
        {
            var sql = GetPagerSql(tblName, fldSort, condition, start, count);
            return ExecuteReader(connectionString, CommandType.Text, sql, null);
        }

        /// <summary>
        /// 执行查询，返回DataSet
        /// </summary>
        public DataSet ExecuteQuery(string connectionString, CommandType cmdType, string cmdText, params DbParameter[] cmdParms)
        {
            using (var conn = new MySqlConnection(connectionString))
            {
                using (var cmd = new MySqlCommand())
                {
                    PrepareCommand(cmd, conn, null, cmdType, cmdText, cmdParms);
                    using (var da = new MySqlDataAdapter(cmd))
                    {
                        var ds = new DataSet();
                        da.Fill(ds, "ds");
                        cmd.Parameters.Clear();
                        return ds;
                    }
                }
            }
        }

        /// <summary>
        /// 在事务中执行查询，返回DataSet
        /// </summary>
        public DataSet ExecuteQuery(DbTransaction trans, CommandType cmdType, string cmdText, params DbParameter[] cmdParms)
        {
            var cmd = new MySqlCommand();
            PrepareCommand(cmd, trans.Connection, trans, cmdType, cmdText, cmdParms);
            var da = new MySqlDataAdapter(cmd);
            var ds = new DataSet();
            da.Fill(ds, "ds");
            cmd.Parameters.Clear();
            return ds;
        }

        /// <summary>
        /// 执行 Transact-SQL 语句并返回受影响的行数。
        /// </summary>
        public int ExecuteNonQuery(string connectionString, CommandType cmdType, string cmdText, params DbParameter[] cmdParms)
        {
            var cmd = new MySqlCommand();
            using (var conn = new MySqlConnection(connectionString))
            {
                PrepareCommand(cmd, conn, null, cmdType, cmdText, cmdParms);
                var val = cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();
                return val;
            }
        }

        /// <summary>
        /// 在事务中执行 Transact-SQL 语句并返回受影响的行数。
        /// </summary>
        public int ExecuteNonQuery(DbTransaction trans, CommandType cmdType, string cmdText, params DbParameter[] cmdParms)
        {
            var cmd = new MySqlCommand();
            PrepareCommand(cmd, trans.Connection, trans, cmdType, cmdText, cmdParms);
            var val = cmd.ExecuteNonQuery();
            cmd.Parameters.Clear();
            return val;
        }

        /// <summary>
        /// 执行查询，返回DataReader
        /// </summary>
        public DbDataReader ExecuteReader(string connectionString, CommandType cmdType, string cmdText, params DbParameter[] cmdParms)
        {
            var cmd = new MySqlCommand();
            var conn = new MySqlConnection(connectionString);
            try
            {
                PrepareCommand(cmd, conn, null, cmdType, cmdText, cmdParms);
                var rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                cmd.Parameters.Clear();
                return rdr;
            }
            catch (Exception e)
            {
                conn.Close();
                Logger.Client.Error(Logger.Client.GetMethodInfo(MethodBase.GetCurrentMethod(), new object[] { connectionString, cmdType, cmdText, cmdParms }), e, LogDomain.Db);
                throw;
            }
        }

        /// <summary>
        /// 在事务中执行查询，返回DataReader
        /// </summary>
        public DbDataReader ExecuteReader(DbTransaction trans, CommandType cmdType, string cmdText, params DbParameter[] cmdParms)
        {
            var cmd = new MySqlCommand();
            PrepareCommand(cmd, trans.Connection, trans, cmdType, cmdText, cmdParms);
            var rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
            cmd.Parameters.Clear();
            return rdr;
        }

        /// <summary>
        /// 执行查询，并返回查询所返回的结果集中第一行的第一列。忽略其他列或行。
        /// </summary>
        public object ExecuteScalar(string connectionString, CommandType cmdType, string cmdText, params DbParameter[] cmdParms)
        {
            var cmd = new MySqlCommand();
            using (var connection = new MySqlConnection(connectionString))
            {
                PrepareCommand(cmd, connection, null, cmdType, cmdText, cmdParms);
                var val = cmd.ExecuteScalar();
                cmd.Parameters.Clear();
                return val;
            }
        }

        /// <summary>
        /// 在事务中执行查询，并返回查询所返回的结果集中第一行的第一列。忽略其他列或行。
        /// </summary>
        public object ExecuteScalar(DbTransaction trans, CommandType cmdType, string cmdText, params DbParameter[] cmdParms)
        {
            var cmd = new MySqlCommand();
            PrepareCommand(cmd, trans.Connection, trans, cmdType, cmdText, cmdParms);
            var val = cmd.ExecuteScalar();
            cmd.Parameters.Clear();
            return val;
        }

        /// <summary>
        /// 生成要执行的命令
        /// </summary>
        private static void PrepareCommand(DbCommand cmd, DbConnection conn, DbTransaction trans, CommandType cmdType, string cmdText, DbParameter[] cmdParms)
        {
            // 如果存在参数，则表示用户是用参数形式的SQL语句，可以替换
            if (cmdParms != null && cmdParms.Length > 0) cmdText = cmdText.Replace("@", "?").Replace(":", "?");
            if (conn.State != ConnectionState.Open) conn.Open();
            cmd.Connection = conn;
            cmd.CommandText = cmdText;
            if (trans != null) cmd.Transaction = trans;
            cmd.CommandType = cmdType;
            if (cmdParms == null) return;
            foreach (var parm in cmdParms)
            {
                // 如果存在参数，则表示用户是用参数形式的SQL语句，可以替换
                parm.ParameterName = parm.ParameterName.Replace("@", "?").Replace(":", "?");
                if (Equals(parm.Value, null)) parm.Value = DBNull.Value;
                cmd.Parameters.Add(parm);
            }
        }
    }
}

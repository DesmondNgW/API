using MySql.Data.MySqlClient;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using System.Data.Common;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Data.SQLite;

namespace X.DB.Util
{
    /// <summary>
    /// 数据库操作
    /// </summary>
    public class DbBase
    {
        /// <summary>
        /// 枚举：数据库类型
        /// </summary>
        public enum DatabaseTypes
        {
            Sql, MySql, Oracle, OleDb, SqLite
        }

        private DatabaseTypes _databaseType;
        private IDbBase _dbBase;

        public DbBase() { }

        public DbBase(DatabaseTypes databaseType, string connectionString)
        {
            DatabaseType = databaseType;
            ConnectionString = connectionString;
        }

        /// <summary>
        /// 数据库类型
        /// </summary>
        public DatabaseTypes DatabaseType
        {
            get
            {
                return _databaseType;
            }
            set
            {
                _databaseType = value;
                switch (value)
                {
                    case DatabaseTypes.OleDb:
                        _dbBase = new OleDbBase();
                        break;
                    case DatabaseTypes.MySql:
                        _dbBase = new MySqlBase();
                        break;
                    case DatabaseTypes.Oracle:
                        _dbBase = new OracleBase();
                        break;
                    case DatabaseTypes.SqLite:
                        _dbBase = new SqLiteBase();
                        break;
                    default:
                        _dbBase = new SqlBase();
                        break;
                }
            }
        }

        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        public string ConnectionString { get; set; }

        /// <summary>
        /// 创建数据库连接
        /// </summary>
        public DbConnection CreateConnection()
        {
            switch (_databaseType)
            {
                case DatabaseTypes.MySql:
                    return new MySqlConnection(ConnectionString);
                case DatabaseTypes.Oracle:
                    return new OracleConnection(ConnectionString);
                case DatabaseTypes.OleDb:
                    return new OleDbConnection(ConnectionString);
                case DatabaseTypes.SqLite:
                    return new SQLiteConnection(ConnectionString);
                default:
                    return new SqlConnection(ConnectionString);
            }
        }

        #region === 创造DbParameter的实例 ===
        /// <summary>
        /// 创造输入DbParameter的实例
        /// </summary>
        public DbParameter CreateInDbParameter(string paraName, DbType dbType, int size, object value)
        {
            return CreateDbParameter(paraName, dbType, size, value, ParameterDirection.Input);
        }

        /// <summary>
        /// 创造输入DbParameter的实例
        /// </summary>
        public DbParameter CreateInDbParameter(string paraName, DbType dbType, object value)
        {
            return CreateDbParameter(paraName, dbType, 0, value, ParameterDirection.Input);
        }

        /// <summary>
        /// 创造输出DbParameter的实例
        /// </summary>        
        public DbParameter CreateOutDbParameter(string paraName, DbType dbType, int size)
        {
            return CreateDbParameter(paraName, dbType, size, null, ParameterDirection.Output);
        }

        /// <summary>
        /// 创造输出DbParameter的实例
        /// </summary>        
        public DbParameter CreateOutDbParameter(string paraName, DbType dbType)
        {
            return CreateDbParameter(paraName, dbType, 0, null, ParameterDirection.Output);
        }

        /// <summary>
        /// 创造返回DbParameter的实例
        /// </summary>        
        public DbParameter CreateReturnDbParameter(string paraName, DbType dbType, int size)
        {
            return CreateDbParameter(paraName, dbType, size, null, ParameterDirection.ReturnValue);
        }

        /// <summary>
        /// 创造返回DbParameter的实例
        /// </summary>        
        public DbParameter CreateReturnDbParameter(string paraName, DbType dbType)
        {
            return CreateDbParameter(paraName, dbType, 0, null, ParameterDirection.ReturnValue);
        }

        /// <summary>
        /// 创造DbParameter的实例
        /// </summary>
        public DbParameter CreateDbParameter(string paraName, DbType dbType, int size, object value, ParameterDirection direction)
        {
            DbParameter para;
            switch (_databaseType)
            {
                case DatabaseTypes.MySql:
                    para = new MySqlParameter();
                    break;
                case DatabaseTypes.Oracle:
                    para = new OracleParameter();
                    break;
                case DatabaseTypes.OleDb:
                    para = new OleDbParameter();
                    break;
                case DatabaseTypes.SqLite:
                    para = new SQLiteParameter();
                    break;
                default:
                    para = new SqlParameter();
                    break;
            }
            para.ParameterName = paraName;
            if (size != 0) para.Size = size;
            para.DbType = dbType;
            if (value != null) para.Value = value;
            para.Direction = direction;
            return para;
        }
        #endregion

        #region === 数据库执行方法 ===

        ///<summary>
        /// 分页获取数据
        /// </summary>
        /// <param name="tblName">表名</param>
        /// <param name="fldSort">排序字段</param>
        /// <param name="condition">条件(不需要where)</param>
        /// <param name="first">从第几条数据开始（从1开始）</param>
        /// <param name="last">到第几条结束</param>
        /// <returns></returns>
        public DbDataReader GetPageList(string tblName, string fldSort, string condition, int first, int last)
        {
            return _dbBase.GetPageList(ConnectionString, tblName, fldSort, condition, first, last);
        }

        /// <summary>
        /// 执行 Transact-SQL 语句并返回受影响的行数。
        /// </summary>
        public int ExecuteNonQuery(CommandType cmdType, string cmdText, params DbParameter[] cmdParms)
        {
            return _dbBase.ExecuteNonQuery(ConnectionString, cmdType, cmdText, cmdParms);
        }

        /// <summary>
        /// 在事务中执行 Transact-SQL 语句并返回受影响的行数。
        /// </summary>
        public int ExecuteNonQuery(DbTransaction trans, CommandType cmdType, string cmdText, params DbParameter[] cmdParms)
        {
            return _dbBase.ExecuteNonQuery(trans, cmdType, cmdText, cmdParms);
        }

        /// <summary>
        /// 在事务中执行查询，返回DataSet
        /// </summary>
        public DataSet ExecuteQuery(DbTransaction trans, CommandType cmdType, string cmdText, params DbParameter[] cmdParms)
        {
            return _dbBase.ExecuteQuery(trans, cmdType, cmdText, cmdParms);
        }

        /// <summary>
        /// 执行查询，返回DataSet
        /// </summary>
        public DataSet ExecuteQuery(CommandType cmdType, string cmdText, params DbParameter[] cmdParms)
        {
            return _dbBase.ExecuteQuery(ConnectionString, cmdType, cmdText, cmdParms);
        }

        /// <summary>
        /// 在事务中执行查询，返回DataReader
        /// </summary>
        public DbDataReader ExecuteReader(DbTransaction trans, CommandType cmdType, string cmdText, params DbParameter[] cmdParms)
        {
            return _dbBase.ExecuteReader(trans, cmdType, cmdText, cmdParms);
        }

        /// <summary>
        /// 执行查询，返回DataReader
        /// </summary>
        public DbDataReader ExecuteReader(CommandType cmdType, string cmdText, params DbParameter[] cmdParms)
        {
            return _dbBase.ExecuteReader(ConnectionString, cmdType, cmdText, cmdParms);
        }

        /// <summary>
        /// 在事务中执行查询，并返回查询所返回的结果集中第一行的第一列。忽略其他列或行。
        /// </summary>
        public object ExecuteScalar(DbTransaction trans, CommandType cmdType, string cmdText, params DbParameter[] cmdParms)
        {
            return _dbBase.ExecuteScalar(trans, cmdType, cmdText, cmdParms);
        }

        /// <summary>
        /// 执行查询，并返回查询所返回的结果集中第一行的第一列。忽略其他列或行。
        /// </summary>
        public object ExecuteScalar(CommandType cmdType, string cmdText, params DbParameter[] cmdParms)
        {
            return _dbBase.ExecuteScalar(ConnectionString, cmdType, cmdText, cmdParms);
        }
        #endregion
    }
}

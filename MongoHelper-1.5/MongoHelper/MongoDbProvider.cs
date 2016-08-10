using System;
using MongoDB.Driver;

namespace MongoDbHelper
{
    #region
    /// <summary>
    /// Mongdb数据库实例
    /// </summary>
    public class MongoDbProvider : IDisposable
    {
        #region 对外Api
        public MongoCollection Collection { get; set; } 
        #endregion

        #region 构造函数

        /// <summary>
        /// MongoDBProvider
        /// 自动创建数据库连接
        /// </summary>
        /// <param name="database">数据库名</param>
        /// <param name="collection">Collection</param>
        /// <param name="reloadConfigure">重新加载配置文件（用于初次加载失败后重新加载）</param>
        /// <param name="credentialDataBase"></param>
        public MongoDbProvider(string database, string collection, Action reloadConfigure = null, string credentialDataBase = null)
        {
            var mongoSever = MongoDbConnection.Connection(string.Empty, string.Empty, credentialDataBase, MongoDbConnection.DefaultConnectName, reloadConfigure);
            var dataBase = mongoSever.GetServer().GetDatabase(database);
            Collection = dataBase.GetCollection(collection);
        }

        /// <summary>
        /// MongoDBProvider
        /// 自动创建数据库连接
        /// </summary>
        /// <param name="database"></param>
        /// <param name="collection"></param>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="credentialDataBase"></param>
        /// <param name="reloadConfigure">重新加载配置文件（用于初次加载失败后重新加载）</param>
        public MongoDbProvider(string database, string collection, string userName, string password, string credentialDataBase = null, Action reloadConfigure = null)
        {
            var mongoSever = MongoDbConnection.Connection(userName, password, credentialDataBase, MongoDbConnection.DefaultConnectName, reloadConfigure);
            var dataBase = mongoSever.GetServer().GetDatabase(database);
            Collection = dataBase.GetCollection(collection);
        }

        /// <summary>
        /// MongoDBProvider
        /// 自动创建数据库连接
        /// </summary>
        /// <param name="connectName"></param>
        /// <param name="database"></param>
        /// <param name="collection"></param>
        /// <param name="reloadConfigure">重新加载配置文件（用于初次加载失败后重新加载）</param>
        /// <param name="credentialDataBase"></param>
        public MongoDbProvider(string connectName, string database, string collection, Action reloadConfigure = null, string credentialDataBase = null)
        {
            var mongoSever = MongoDbConnection.Connection(string.Empty, string.Empty, credentialDataBase, connectName, reloadConfigure);
            var dataBase = mongoSever.GetServer().GetDatabase(database);
            Collection = dataBase.GetCollection(collection);
        }

        /// <summary>
        /// MongoDBProvider
        /// 自动创建数据库连接
        /// </summary>
        /// <param name="connectName"></param>
        /// <param name="database"></param>
        /// <param name="collection"></param>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="reloadConfigure">重新加载配置文件（用于初次加载失败后重新加载）</param>
        /// <param name="credentialDataBase"></param>
        public MongoDbProvider(string connectName, string database, string collection, string userName, string password, Action reloadConfigure = null, string credentialDataBase = null)
        {
            var mongoSever = MongoDbConnection.Connection(userName, password, credentialDataBase, connectName, reloadConfigure);
            var dataBase = mongoSever.GetServer().GetDatabase(database);
            Collection = dataBase.GetCollection(collection);
        } 
        #endregion

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {

        }
    }
    #endregion
}

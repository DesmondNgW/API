using System;
using MongoDB.Bson;
using MongoDB.Driver;

namespace MongoDbHelper
{
    #region
    /// <summary>
    /// Mongdb数据库实例
    /// </summary>
    public class MongoDbProvider : IDisposable
    {
        /// <summary>
        /// 数据库
        /// </summary>
        public MongoDatabase DataBase;
        /// <summary>
        /// 集合表
        /// </summary>
        public MongoCollection<BsonDocument> Collection { get; set; }

        /// <summary>
        /// MongoDBProvider
        /// 自动创建数据库连接
        /// </summary>
        /// <param name="database">数据库名</param>
        /// <param name="collection">Collection</param>
        /// <param name="reloadConfigure">重新加载配置文件（用于初次加载失败后重新加载）</param>
        /// <param name="maxConnectionPoolSize">连接池大小</param>
        public MongoDbProvider(string database, string collection, Action reloadConfigure = null, int maxConnectionPoolSize = MongoDbConnection.MaxConnectionPoolSize)
        {
            var mongoSever = MongoDbConnection.Connection(string.Empty, string.Empty, collection, MongoDbConnection.DefaultConnectName, reloadConfigure, maxConnectionPoolSize).GetServer();
            DataBase = mongoSever.GetDatabase(database);
            Collection = DataBase.GetCollection(collection);
        }

        /// <summary>
        /// MongoDBProvider
        /// 自动创建数据库连接
        /// </summary>
        /// <param name="database"></param>
        /// <param name="collection"></param>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="reloadConfigure">重新加载配置文件（用于初次加载失败后重新加载）</param>
        /// <param name="maxConnectionPoolSize">连接池大小</param>
        public MongoDbProvider(string database, string collection, string userName, string password, Action reloadConfigure = null, int maxConnectionPoolSize = MongoDbConnection.MaxConnectionPoolSize)
        {
            var mongoSever = MongoDbConnection.Connection(userName, password, collection, MongoDbConnection.DefaultConnectName, reloadConfigure, maxConnectionPoolSize).GetServer();
            DataBase = mongoSever.GetDatabase(database);
            Collection = DataBase.GetCollection(collection);
        }

        /// <summary>
        /// MongoDBProvider
        /// 自动创建数据库连接
        /// </summary>
        /// <param name="connectName"></param>
        /// <param name="database"></param>
        /// <param name="collection"></param>
        /// <param name="reloadConfigure">重新加载配置文件（用于初次加载失败后重新加载）</param>
        /// <param name="maxConnectionPoolSize">连接池大小</param>
        public MongoDbProvider(string connectName, string database, string collection, Action reloadConfigure, int maxConnectionPoolSize = MongoDbConnection.MaxConnectionPoolSize)
        {
            var mongoSever = MongoDbConnection.Connection(string.Empty, string.Empty, collection, connectName, reloadConfigure, maxConnectionPoolSize).GetServer();
            DataBase = mongoSever.GetDatabase(database);
            Collection = DataBase.GetCollection(collection);
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
        /// <param name="maxConnectionPoolSize">连接池大小</param>
        public MongoDbProvider(string connectName, string database, string collection, string userName, string password, Action reloadConfigure, int maxConnectionPoolSize = MongoDbConnection.MaxConnectionPoolSize)
        {
            var mongoSever = MongoDbConnection.Connection(userName, password, collection, connectName, reloadConfigure, maxConnectionPoolSize).GetServer();
            DataBase = mongoSever.GetDatabase(database);
            Collection = DataBase.GetCollection(collection);
        }

        #region IDisposable 成员
        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {

        }
        #endregion
    }
    #endregion
}

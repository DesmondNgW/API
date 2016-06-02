using System;
using MongoDB.Driver;

namespace MongoDbHelper
{
    #region
    /// <summary>
    /// Mongdb数据库实例
    /// </summary>
    public class MongoDbProvider<TDocument> : IDisposable
    {
        /// <summary>
        /// 数据库
        /// </summary>
        public IMongoDatabase IDataBase;
        [Obsolete("Use the new API instead.")]
        public MongoDatabase DataBase;

        /// <summary>
        /// 集合表
        /// </summary>
        public IMongoCollection<TDocument> ICollection { get; set; }
        [Obsolete("Use the new API instead.")]
        public MongoCollection<TDocument> Collection { get; set; }

        /// <summary>
        /// MongoDBProvider
        /// 自动创建数据库连接
        /// </summary>
        /// <param name="database">数据库名</param>
        /// <param name="collection">Collection</param>
        /// <param name="credentialDataBase"></param>
        /// <param name="reloadConfigure">重新加载配置文件（用于初次加载失败后重新加载）</param>
        /// <param name="maxConnectionPoolSize">连接池大小</param>
        public MongoDbProvider(string database, string collection, string credentialDataBase = "", Action reloadConfigure = null, int maxConnectionPoolSize = MongoDbConnection.MaxConnectionPoolSize)
        {
            var mongoSever = MongoDbConnection.Connection(string.Empty, string.Empty, credentialDataBase, MongoDbConnection.DefaultConnectName, reloadConfigure, maxConnectionPoolSize);
            IDataBase = mongoSever.GetDatabase(database);
            ICollection = IDataBase.GetCollection<TDocument>(collection);
            DataBase = mongoSever.GetServer().GetDatabase(database);
            Collection = DataBase.GetCollection<TDocument>(collection);
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
        /// <param name="maxConnectionPoolSize">连接池大小</param>
        public MongoDbProvider(string database, string collection, string userName, string password, string credentialDataBase = "", Action reloadConfigure = null, int maxConnectionPoolSize = MongoDbConnection.MaxConnectionPoolSize)
        {
            var mongoSever = MongoDbConnection.Connection(userName, password, credentialDataBase, MongoDbConnection.DefaultConnectName, reloadConfigure, maxConnectionPoolSize);
            IDataBase = mongoSever.GetDatabase(database);
            ICollection = IDataBase.GetCollection<TDocument>(collection);
            DataBase = mongoSever.GetServer().GetDatabase(database);
            Collection = DataBase.GetCollection<TDocument>(collection);
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
        /// <param name="maxConnectionPoolSize">连接池大小</param>
        public MongoDbProvider(string connectName, string database, string collection, Action reloadConfigure, string credentialDataBase = "", int maxConnectionPoolSize = MongoDbConnection.MaxConnectionPoolSize)
        {
            var mongoSever = MongoDbConnection.Connection(string.Empty, string.Empty, credentialDataBase, connectName, reloadConfigure, maxConnectionPoolSize);
            IDataBase = mongoSever.GetDatabase(database);
            ICollection = IDataBase.GetCollection<TDocument>(collection);
            DataBase = mongoSever.GetServer().GetDatabase(database);
            Collection = DataBase.GetCollection<TDocument>(collection);
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
        /// <param name="maxConnectionPoolSize">连接池大小</param>
        public MongoDbProvider(string connectName, string database, string collection, string userName, string password, Action reloadConfigure, string credentialDataBase = "", int maxConnectionPoolSize = MongoDbConnection.MaxConnectionPoolSize)
        {
            var mongoSever = MongoDbConnection.Connection(userName, password, credentialDataBase, connectName, reloadConfigure, maxConnectionPoolSize);
            IDataBase = mongoSever.GetDatabase(database);
            ICollection = IDataBase.GetCollection<TDocument>(collection);
            DataBase = mongoSever.GetServer().GetDatabase(database);
            Collection = DataBase.GetCollection<TDocument>(collection);
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

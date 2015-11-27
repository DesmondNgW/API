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
        public MongoDatabase DataBase;
        public MongoCollection<BsonDocument> Collection { get; set; }

        /// <summary>
        /// MongoDBProvider
        /// 自动创建数据库连接
        /// </summary>
        /// <param name="database">数据库名</param>
        /// <param name="collection">Collection</param>
        public MongoDbProvider(string database, string collection)
        {
            var mongoSever = MongoDbConnection.Connection(string.Empty, string.Empty, collection, MongoDbConnection.DefaultConnectName).GetServer();
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
        public MongoDbProvider(string database, string collection, string userName, string password)
        {
            var mongoSever = MongoDbConnection.Connection(userName, password, collection, MongoDbConnection.DefaultConnectName).GetServer();
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
        public MongoDbProvider(string connectName, string database, string collection)
        {
            var mongoSever = MongoDbConnection.Connection(string.Empty, string.Empty, collection, connectName).GetServer();
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
        public MongoDbProvider(string connectName, string database, string collection, string userName, string password)
        {
            var mongoSever = MongoDbConnection.Connection(userName, password, collection, connectName).GetServer();
            DataBase = mongoSever.GetDatabase(database);
            Collection = DataBase.GetCollection(collection);
        }

        #region IDisposable 成员
        public void Dispose()
        {

        }
        #endregion
    }
    #endregion
}

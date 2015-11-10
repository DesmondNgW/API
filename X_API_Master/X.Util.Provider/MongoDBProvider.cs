using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Diagnostics;
using System.Reflection;
using X.Util.Core;

namespace X.Util.Provider
{
    public sealed class MongoDbProvider
    {
        #region 构造函数
        private const string Servername = ConfigurationHelper.MongoDefaultServername;
        private const string Database = "admin";
        private const string CollectionName = "Test";
        private MongoServer MongoSever { get; }
        public MongoCollection<BsonDocument> Collection { get; set; }
        public MongoDbProvider()
        {
            MongoSever = Init(Servername);
            var db = MongoSever.GetDatabase(Database);
            Collection = db.GetCollection(CollectionName);
            _sw.Start();
        }
        public MongoDbProvider(string collection)
        {
            MongoSever = Init(Servername);
            var db = MongoSever.GetDatabase(Database);
            Collection = db.GetCollection(collection);
            _sw.Start();
        }
        public MongoDbProvider(string database, string collection)
        {
            MongoSever = Init(Servername);
            var db = MongoSever.GetDatabase(database);
            Collection = db.GetCollection(collection);
            _sw.Start();
        }
        public MongoDbProvider(string database, string collection, string serverName)
        {
            MongoSever = Init(serverName);
            var db = MongoSever.GetDatabase(database);
            Collection = db.GetCollection(collection);
            _sw.Start();
        }
        #endregion

        #region 实现方法
        private readonly Stopwatch _sw = new Stopwatch();
        /// <summary>
        /// 初始化MongoDBClient
        /// </summary>
        /// <param name="serverName"></param>
        /// <returns></returns>
        private static MongoClient GetMongoClient(string serverName)
        {
            return new MongoClient(ConfigurationHelper.MongoClientConfiguration(serverName));
        }
        /// <summary>
        /// 初始化MongoDBServer的连接
        /// </summary>
        /// <param name="serverName"></param>
        /// <returns></returns>
        private static MongoServer Init(string serverName)
        {
            MongoServer mongoServer = null;
            try
            {
                mongoServer = Core<MongoClient>.Instance(GetMongoClient, serverName, ConfigurationHelper.EndpointFile + serverName).GetServer();
            }
            catch (Exception ex)
            {
                Logger.Error(MethodBase.GetCurrentMethod(), LogDomain.Util, null, ex.ToString());
            }
            return mongoServer;
        }
        /// <summary>
        /// 关闭连接
        /// </summary>
        public void Dispose(MethodBase method, LogDomain eDomain)
        {
            _sw.Stop();
            Core<MongoServer>.Close(method, _sw.ElapsedMilliseconds, eDomain);
            _sw.Reset();
        }
        #endregion
    }
}

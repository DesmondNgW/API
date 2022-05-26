using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Reflection;
using X.Util.Core.Configuration;
using X.Util.Core.Kernel;
using X.Util.Core.Log;
using X.Util.Entities;
using X.Util.Entities.Enum;
using X.Util.Entities.Interface;

namespace X.Util.Provider
{
    public class MongoDbProvider<TDocument> : IProvider<IMongoCollection<TDocument>>
    {
        #region 构造函数
        private readonly string _servername = ConfigurationHelper.MongoDefaultServername;
        private const string Database = "admin";
        private const string CollectionName = "Test";
        private MongoClient MongoClient { get; set; }
        private IMongoCollection<TDocument> Collection { get; set; }
        public MongoDbProvider()
        {
            MongoClient = Init(_servername, Database);
            var db = MongoClient.GetDatabase(Database);
            Collection = db.GetCollection<TDocument>(CollectionName);
        }
        public MongoDbProvider(string collection)
        {
            MongoClient = Init(_servername, Database);
            var db = MongoClient.GetDatabase(Database);
            Collection = db.GetCollection<TDocument>(collection);
        }
        public MongoDbProvider(string database, string collection)
        {
            MongoClient = Init(_servername, database);
            var db = MongoClient.GetDatabase(database);
            Collection = db.GetCollection<TDocument>(collection);
        }
        public MongoDbProvider(string database, string collection, string serverName)
        {
            _servername = serverName;
            MongoClient = Init(serverName, database);
            var db = MongoClient.GetDatabase(database);
            Collection = db.GetCollection<TDocument>(collection);
        }
        public MongoDbProvider(string database, string collection, IEnumerable<Uri> servers, string userName, string password, MongoCredentialType type)
        {
            MongoClient = ConfigurationHelper.MongoClientConfiguration(servers, userName, password, database, type);
            var db = MongoClient.GetDatabase(database);
            Collection = db.GetCollection<TDocument>(collection);
        }
        #endregion

        #region 实现方法

        /// <summary>
        /// 初始化MongoDBClient
        /// </summary>
        /// <param name="serverName"></param>
        /// <param name="credentialDataBase"></param>
        /// <returns></returns>
        private static MongoClient GetMongoClient(string serverName, string credentialDataBase = null)
        {
            return new MongoClient(ConfigurationHelper.MongoClientConfiguration(serverName, credentialDataBase));
        }

        /// <summary>
        /// 初始化MongoDBServer的连接
        /// </summary>
        /// <param name="serverName"></param>
        /// <param name="database"></param>
        /// <returns></returns>
        private static MongoClient Init(string serverName, string database)
        {
            var client = default(MongoClient);
            try
            {
                client = Core<MongoClient>.Instance(GetMongoClient, serverName, database, serverName + database + ConfigurationHelper.EndpointFileModified);
            }
            catch (Exception ex)
            {
                Logger.Client.Error(Logger.Client.GetMethodInfo(MethodBase.GetCurrentMethod(), new object[] { serverName }), ex, LogDomain.Util);
            }
            return client;
        }
        #endregion

        #region 对外公开属性和方法
        public string EndpointAddress => _servername;

        public LogDomain Domain => LogDomain.ThirdParty;

        public IMongoCollection<TDocument> Client => Collection;
        #endregion
    }
}

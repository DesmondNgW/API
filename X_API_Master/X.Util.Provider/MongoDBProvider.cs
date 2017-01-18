using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Reflection;
using X.Util.Core.Configuration;
using X.Util.Core.Kernel;
using X.Util.Core.Log;
using X.Util.Entities;
using X.Util.Entities.Enums;
using X.Util.Entities.Interface;

namespace X.Util.Provider
{
    public class MongoDbProvider<TDocument> : IProvider<MongoCollection<TDocument>>
    {
        #region 构造函数
        private readonly string _servername = ConfigurationHelper.MongoDefaultServername;
        private const string Database = "admin";
        private const string CollectionName = "Test";
        private MongoServer MongoSever { get; set; }
        private MongoCollection<TDocument> Collection { get; set; }
        public MongoDbProvider()
        {
            MongoSever = Init(_servername, Database);
            var db = MongoSever.GetDatabase(Database);
            Collection = db.GetCollection<TDocument>(CollectionName);
        }
        public MongoDbProvider(string collection)
        {
            MongoSever = Init(_servername, Database);
            var db = MongoSever.GetDatabase(Database);
            Collection = db.GetCollection<TDocument>(collection);
        }
        public MongoDbProvider(string database, string collection)
        {
            MongoSever = Init(_servername, database);
            var db = MongoSever.GetDatabase(database);
            Collection = db.GetCollection<TDocument>(collection);
        }
        public MongoDbProvider(string database, string collection, string serverName)
        {
            _servername = serverName;
            MongoSever = Init(serverName, database);
            var db = MongoSever.GetDatabase(database);
            Collection = db.GetCollection<TDocument>(collection);
        }
        public MongoDbProvider(string database, string collection, IEnumerable<Uri> servers, string userName, string password, MongoCredentialType type)
        {
            MongoSever = ConfigurationHelper.MongoClientConfiguration(servers, userName, password, database, type).GetServer();
            var db = MongoSever.GetDatabase(database);
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
        private static MongoServer Init(string serverName, string database)
        {
            var mongoServer = default(MongoServer);
            try
            {
                mongoServer = Core<MongoClient>.Instance(GetMongoClient, serverName, database, Math.Max(ExecutionContext<RequestContext>.Current.Zone, 1) + serverName + database + ConfigurationHelper.EndpointFileModified).GetServer();
            }
            catch (Exception ex)
            {
                Logger.Client.Error(Logger.Client.GetMethodInfo(MethodBase.GetCurrentMethod(), new object[] { serverName }), ex, LogDomain.Util);
            }
            return mongoServer;
        }
        #endregion

        #region 对外公开属性和方法
        public string EndpointAddress
        {
            get { return _servername; }
        }

        public LogDomain Domain
        {
            get { return LogDomain.ThirdParty; }
        }

        public MongoCollection<TDocument> Client
        {
            get { return Collection; }
        }
        #endregion
    }
}

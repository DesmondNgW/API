using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Reflection;
using X.Util.Core.Configuration;
using X.Util.Core.Kernel;
using X.Util.Core.Log;
using X.Util.Entities;
using X.Util.Entities.Interface;

namespace X.Util.Provider
{
    public class MongoDbProvider<TDocument> : IProvider<MongoCollection<TDocument>>
    {
        #region 构造函数
        private readonly string _servername = ConfigurationHelper.MongoDefaultServername;
        private const string Database = "admin";
        private const string CollectionName = "Test";
        private readonly string _credentialDataBase = string.Empty;
        private MongoServer MongoSever { get; set; }
        private MongoCollection<TDocument> Collection { get; set; }
        public MongoDbProvider(string credentialDataBase)
        {
            _credentialDataBase = credentialDataBase;
            MongoSever = Init(_servername);
            var db = MongoSever.GetDatabase(Database);
            Collection = db.GetCollection<TDocument>(CollectionName);
        }
        public MongoDbProvider(string collection, string credentialDataBase)
        {
            _credentialDataBase = credentialDataBase;
            MongoSever = Init(_servername);
            var db = MongoSever.GetDatabase(Database);
            Collection = db.GetCollection<TDocument>(collection);
        }
        public MongoDbProvider(string database, string collection, string credentialDataBase)
        {
            _credentialDataBase = credentialDataBase;
            MongoSever = Init(_servername);
            var db = MongoSever.GetDatabase(database);
            Collection = db.GetCollection<TDocument>(collection);
        }
        public MongoDbProvider(string database, string collection, string serverName, string credentialDataBase)
        {
            _credentialDataBase = credentialDataBase;
            _servername = serverName;
            MongoSever = Init(serverName);
            var db = MongoSever.GetDatabase(database);
            Collection = db.GetCollection<TDocument>(collection);
        }
        public MongoDbProvider(string database, string collection, IEnumerable<Uri> servers, string userName, string password, MongoCredentialType type, string credentialDataBase = "admin")
        {
            MongoSever = ConfigurationHelper.MongoClientConfiguration(servers, userName, password, credentialDataBase, type).GetServer();
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
        /// <returns></returns>
        private MongoServer Init(string serverName)
        {
            MongoServer mongoServer = null;
            try
            {
                mongoServer = string.IsNullOrEmpty(_credentialDataBase) ? Core<MongoClient>.Instance(GetMongoClient, serverName, null, Math.Max(ExecutionContext<RequestContext>.Current.Zone, 1) + serverName + ConfigurationHelper.EndpointFileModified).GetServer() : Core<MongoClient>.Instance(GetMongoClient, serverName, _credentialDataBase, Math.Max(ExecutionContext<RequestContext>.Current.Zone, 1) + serverName + _credentialDataBase + ConfigurationHelper.EndpointFileModified).GetServer();
            }
            catch (Exception ex)
            {
                Logger.Client.Error(MethodBase.GetCurrentMethod(), LogDomain.Util, null, string.Empty, ex.ToString());
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

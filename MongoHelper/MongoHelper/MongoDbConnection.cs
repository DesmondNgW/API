﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Driver;

namespace MongoDbHelper
{
    /// <summary>
    /// MongoDB链接
    /// </summary>
    internal class MongoDbConnection
    {
        private static readonly ConcurrentDictionary<string, MongoDbConfigure> Configures = new ConcurrentDictionary<string, MongoDbConfigure>();
        private static readonly object LockObj = new object();
        internal const string DefaultConnectName = "def";

        /// <summary>
        /// 添加Mongo数据库配置
        /// </summary>
        /// <param name="servers"></param>
        /// <param name="connectName"></param>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        public static void Configure(Uri[] servers, string connectName, string userName = "", string password = "")
        {
            if (Configures.ContainsKey(connectName) && Configures[connectName] != null) return;
            lock (LockObj)
            {
                if (Configures.ContainsKey(connectName) && Configures[connectName] != null) return;
                var config = new MongoDbConfigure();
                if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(password))
                {
                    var credentials = MongoCredential.CreateMongoCRCredential("admin", userName, password);
                    config.MongoClientSettings.Credentials = new List<MongoCredential> {credentials};
                }
                config.MongoClientSettings.MaxConnectionPoolSize = 20;
                config.MongoClientSettings.MinConnectionPoolSize = 1;
                config.MongoClientSettings.WaitQueueSize = 5000;
                var list = new List<MongoServerAddress>();
                if (servers != null && servers.Length > 0)
                {
                    list.AddRange(servers.Select(uri => new MongoServerAddress(uri.Host, uri.Port)));
                    config.Servers = servers;
                }
                config.MongoClientSettings.Servers = list;
                config.MongoClient = new MongoClient(config.MongoClientSettings);
                Configures[connectName] = config;
            }
        }

        /// <summary>
        /// 连接数据库(兼容旧版本)
        /// </summary>
        /// <param name="conn"></param>
        /// <returns></returns>
        public static MongoClient Connection(string conn = DefaultConnectName)
        {
            return Connection(string.Empty, string.Empty, string.Empty, conn);
        }

        /// <summary>
        /// 连接数据库
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <param name="password">密码</param>
        /// <param name="collection">表collection</param>
        /// <param name="connectName">数据库conn</param>
        /// <returns>
        /// 返回数据库连接地址
        /// </returns>
        public static MongoClient Connection(string userName, string password, string collection, string connectName)
        {
            var setting = Configures[connectName];
            if (setting?.MongoClient != null) return setting.MongoClient;
            lock (LockObj)
            {
                if (setting?.MongoClient != null) return Configures[connectName].MongoClient;
                if (setting == null) return null;
                try
                {
                    if (string.IsNullOrWhiteSpace(collection)) collection = "admin";
                    if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(password))
                    {
                        var credentials = MongoCredential.CreateMongoCRCredential(collection, userName, password);
                        setting.MongoClientSettings.Credentials = new List<MongoCredential> { credentials };
                    }
                    setting.MongoClientSettings.MaxConnectionPoolSize = 20;
                    setting.MongoClientSettings.MinConnectionPoolSize = 1;
                    setting.MongoClientSettings.WaitQueueSize = 5000;
                    if (setting.Servers != null && setting.Servers.Length > 0)
                    {
                        var list = new List<MongoServerAddress>();
                        list.AddRange(setting.Servers.Select(uri => new MongoServerAddress(uri.Host, uri.Port)));
                        setting.MongoClientSettings.Servers = list;
                    }
                    setting.MongoClient = new MongoClient(setting.MongoClientSettings);
                    Configures[connectName] = setting;
                    return setting.MongoClient;
                }
                catch (Exception ex)
                {
                    throw (new Exception($"Servers: {setting.Servers.ToJson()} \r\n Exceptions: {ex}"));
                }
            }
        }

        /// <summary>
        /// 连接池状态
        /// </summary>
        public static void ShowStatus()
        {
            foreach (var key in Configures.Keys)
            {
                Console.WriteLine($@"connectName: {key}, Servers: {Configures[key].Servers.ToJson()}, Connnect: {Configures[key].MongoClient != null}");
            }
        }
    }

    /// <summary>
    /// MongoClient配置类
    /// </summary>
    internal class MongoDbConfigure
    {
        public Uri[] Servers { get; set; }

        public MongoClient MongoClient { get; set; }

        public MongoClientSettings MongoClientSettings { get; set; } = new MongoClientSettings();
    }
}

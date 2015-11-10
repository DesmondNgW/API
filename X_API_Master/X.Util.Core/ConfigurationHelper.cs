using Couchbase;
using Couchbase.Configuration;
using MongoDB.Driver;
using ServiceStack.Redis;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web.Caching;
using System.Xml;
using X.Util.Entities;

namespace X.Util.Core
{
    public class ConfigurationHelper
    {
        private const string CacheConfigurationPrefix = "X.Util.Core.CacheConfigurationPrefix";
        public const int MaxPoolSize = 1;
        public const string MongoDefaultServername = "XMongo";
        public const string RedisDefaultServername = "XRedis";
        public const string CouchDefaultServername = "XCouch";
        public static string EndpointFile => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data\\endpoint_" + Math.Max(ExecutionContext<RequestContext>.Current.Zone, 1) + ".xml");
        public static string ConfigFile => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data\\Config.xml");

        #region GetAppSetting From web.config
        public static string GetAppSetting(string name)
        {
            return ConfigurationManager.AppSettings[name] ?? string.Empty;
        }

        public static string GetAppSetting(string name, string defaultValue)
        {
            var result = GetAppSetting(name);
            return string.IsNullOrEmpty(result) ? defaultValue : result;
        }
        #endregion

        #region GetAppSetting From Xml
        public static string GetAppSettingByName(string name, string defaultValue)
        {
            var key = CacheConfigurationPrefix + name + "GetAppSettingByName";
            var result = LocalCache.Get<string>(key);
            if (!string.IsNullOrWhiteSpace(result)) return result;
            result = defaultValue;
            var doc = XmlHelper.GetXmlDocCache(ConfigFile);
            if (Equals(doc, null)) return result;
            var node = doc.SelectSingleNode("/configuration/AppSettings/add[@key='" + name + "']");
            result = XmlHelper.GetXmlAttributeValue(node, "value", defaultValue);
            LocalCache.SlidingExpirationSet(key, result, new CacheDependency(ConfigFile, DateTime.Now), new TimeSpan(1, 0, 0), CacheItemPriority.AboveNormal);
            return result;
        }

        public static void UpdateAppSettingByName(string name, string value)
        {
            var doc = XmlHelper.GetXmlDocCache(ConfigFile);
            if (Equals(doc, null)) return;
            var node = doc.SelectSingleNode("/configuration/AppSettings/add[@key='" + name + "']");
            if (node?.Attributes != null)
            {
                var attr = node.Attributes["value"] ?? doc.CreateAttribute("value");
                attr.Value = value;
                node.Attributes.SetNamedItem(attr);
            }
            doc.Save(ConfigFile);
        }

        public static byte GetAppSettingByName(string name, byte defaultValue)
        {
            return CoreParse.GetByte(GetAppSettingByName(name, string.Empty), defaultValue);
        }

        public static bool GetAppSettingByName(string name, bool defaultValue)
        {
            return CoreParse.GetBoolean(GetAppSettingByName(name, string.Empty), defaultValue);
        }

        public static short GetAppSettingByName(string name, short defaultValue)
        {
            return CoreParse.GetInt16(GetAppSettingByName(name, string.Empty), defaultValue);
        }

        public static int GetAppSettingByName(string name, int defaultValue)
        {
            return CoreParse.GetInt32(GetAppSettingByName(name, string.Empty), defaultValue);
        }

        public static long GetAppSettingByName(string name, long defaultValue)
        {
            return CoreParse.GetInt64(GetAppSettingByName(name, string.Empty), defaultValue);
        }

        public static float GetAppSettingByName(string name, float defaultValue)
        {
            return CoreParse.GetSingle(GetAppSettingByName(name, string.Empty), defaultValue);
        }

        public static double GetAppSettingByName(string name, double defaultValue)
        {
            return CoreParse.GetDouble(GetAppSettingByName(name, string.Empty), defaultValue);
        }

        public static decimal GetAppSettingByName(string name, decimal defaultValue)
        {
            return CoreParse.GetDecimal(GetAppSettingByName(name, string.Empty), defaultValue);
        }

        public static DateTime GetAppSettingByName(string name, DateTime defaultValue)
        {
            return CoreParse.GetDateTime(GetAppSettingByName(name, string.Empty), defaultValue);
        }
        #endregion

        #region Get WCF EndpointAddress Configuration
        public static XmlServiceModel GetEndpointAddressesByName(string name)
        {
            var key = CacheConfigurationPrefix + EndpointFile + name + "GetEndpointAddressesByName";
            var result = LocalCache.Get<XmlServiceModel>(key);
            if (result != null) return result;
            var doc = XmlHelper.GetXmlDocCache(EndpointFile);
            var node = doc?.SelectSingleNode("/configuration/client/endpoint[@name='" + name + "']");
            if (Equals(node, null) || node.ChildNodes.Count <= 0) return null;
            result = new XmlServiceModel
            {
                ConfigurationName = name,
                Index = XmlHelper.GetXmlAttributeValue(node, "index", -1),
                MaxPoolSize = XmlHelper.GetXmlAttributeValue(node, "maxPoolSize", MaxPoolSize),
                Endpoints = new List<string>()
            };
            foreach (var uri in node.ChildNodes.Cast<XmlNode>().Select(child => XmlHelper.GetXmlNodeValue(child, string.Empty)).Where(uri => !string.IsNullOrEmpty(uri)))
            {
                result.Endpoints.Add(uri);
            }
            LocalCache.SlidingExpirationSet(key, result, new CacheDependency(EndpointFile, DateTime.Now), new TimeSpan(1, 0, 0), CacheItemPriority.AboveNormal);
            return result;
        }
        #endregion

        #region CouchBase,Redis And MongoDB Configuration
        public static CouchbaseClientConfiguration CouchbaseConfiguration(string serverName)
        {
            var couchbaseClientConfiguration = new CouchbaseClientConfiguration();
            var doc = XmlHelper.GetXmlDocCache(EndpointFile);
            if (Equals(doc, null)) return couchbaseClientConfiguration;
            var node = doc.SelectSingleNode("/configuration/couchbase/server[@name='" + serverName + "']");
            if (Equals(node, null)) return couchbaseClientConfiguration;
            couchbaseClientConfiguration.Bucket = XmlHelper.GetXmlAttributeValue(node, "bucket", string.Empty);
            couchbaseClientConfiguration.BucketPassword = XmlHelper.GetXmlAttributeValue(node, "password", string.Empty);
            foreach (XmlNode item in node.ChildNodes)
            {
                switch (item.Name)
                {
                    case "urls":
                        foreach (XmlNode uri in item.ChildNodes) couchbaseClientConfiguration.Urls.Add(new Uri(XmlHelper.GetXmlAttributeValue(uri, "uri", string.Empty)));
                        break;
                    case "socketPool":
                        couchbaseClientConfiguration.SocketPool.MinPoolSize = CoreParse.GetInt32((XmlHelper.GetXmlAttributeValue(item, "minPoolSize", string.Empty)), 20);
                        couchbaseClientConfiguration.SocketPool.MaxPoolSize = CoreParse.GetInt32((XmlHelper.GetXmlAttributeValue(item, "maxPoolSize", string.Empty)), 1000);
                        couchbaseClientConfiguration.SocketPool.ConnectionTimeout = TimeSpan.Parse(XmlHelper.GetXmlAttributeValue(item, "connectionTimeout", "00:00:02"));
                        couchbaseClientConfiguration.SocketPool.DeadTimeout = TimeSpan.Parse(XmlHelper.GetXmlAttributeValue(item, "deadTimeout", "00:00:10"));
                        couchbaseClientConfiguration.SocketPool.ReceiveTimeout = TimeSpan.Parse(XmlHelper.GetXmlAttributeValue(item, "receiveTimeout", "00:00:10"));
                        couchbaseClientConfiguration.SocketPool.QueueTimeout = TimeSpan.Parse(XmlHelper.GetXmlAttributeValue(item, "queueTimeout", "00:00:00.1000000"));
                        break;
                }
            }
            return couchbaseClientConfiguration;
        }
        /// <summary>
        /// CouchBase Configuration
        /// </summary>
        public static CouchbaseClient GetCouchbaseClient(string bucket, string password, string[] urls)
        {
            var configuration = new CouchbaseClientConfiguration {Bucket = bucket, BucketPassword = password};
            if (urls != null && urls.Length > 0)
            {
                foreach (var t in urls) configuration.Urls.Add(new Uri(t));
            }
            configuration.SocketPool.ConnectionTimeout = new TimeSpan(0, 0, 2);
            configuration.SocketPool.DeadTimeout = new TimeSpan(0, 0, 10);
            configuration.SocketPool.ReceiveTimeout = new TimeSpan(0, 0, 10);
            configuration.SocketPool.MaxPoolSize = 1000;
            configuration.SocketPool.MinPoolSize = 20;
            return new CouchbaseClient(configuration);
        }

        /// <summary>
        /// MongoDb Configuration
        /// </summary>
        public static MongoClientSettings MongoClientConfiguration(string serverName)
        {
            var configuration = new MongoClientSettings();
            var doc = XmlHelper.GetXmlDocCache(EndpointFile);
            if (Equals(doc, null)) return configuration;
            var node = doc.SelectSingleNode("/configuration/mongodb/server[@name='" + serverName + "']");
            if (Equals(node, null)) return configuration;
            var servers = new List<MongoServerAddress>();
            foreach (XmlNode item in node.ChildNodes)
            {
                switch (item.Name)
                {
                    case "add":
                        var host = XmlHelper.GetXmlAttributeValue(item, "host", "127.0.0.1");
                        var port = XmlHelper.GetXmlAttributeValue(item, "port", 27017);
                        servers.Add(new MongoServerAddress(host, port));
                        break;
                    case "socketPool":
                        configuration.MaxConnectionPoolSize = XmlHelper.GetXmlAttributeValue(item, "maxConnectionPoolSize", 50);
                        configuration.MinConnectionPoolSize = XmlHelper.GetXmlAttributeValue(item, "minConnectionPoolSize", 10);
                        configuration.ConnectTimeout = TimeSpan.Parse(XmlHelper.GetXmlAttributeValue(item, "connectTimeout", "00:00:30"));
                        configuration.MaxConnectionIdleTime = TimeSpan.Parse(XmlHelper.GetXmlAttributeValue(item, "maxConnectionIdleTime", "00:10:00"));
                        configuration.MaxConnectionLifeTime = TimeSpan.Parse(XmlHelper.GetXmlAttributeValue(item, "maxConnectionLifeTime", "00:30:00"));
                        configuration.SocketTimeout = TimeSpan.Parse(XmlHelper.GetXmlAttributeValue(item, "socketTimeout", "00:00:00"));
                        configuration.WaitQueueTimeout = TimeSpan.Parse(XmlHelper.GetXmlAttributeValue(item, "waitQueueTimeout", "00:02:00"));
                        break;
                }
            }
            configuration.Servers = servers;
            return configuration;
        }

        /// <summary>
        /// Redis Configuration
        /// </summary>
        public static PooledRedisClientManager GetRedisClientManager(string serverName)
        {
            var doc = XmlHelper.GetXmlDocCache(EndpointFile);
            var node = doc?.SelectSingleNode("/configuration/redis/server[@name='" + serverName + "']");
            if (Equals(doc, null)) return null;
            var config = new RedisClientManagerConfig();
            var readWriteHosts = new List<string>();
            var readOnlyHosts = new List<string>();
            foreach (XmlNode item in node.ChildNodes)
            {
                switch (item.Name)
                {
                    case "read":
                        readOnlyHosts.AddRange(from XmlNode rcnode in item.ChildNodes select XmlHelper.GetXmlAttributeValue(rcnode, "uri", string.Empty));
                        break;
                    case "write":
                        readWriteHosts.AddRange(from XmlNode rcnode in item.ChildNodes select XmlHelper.GetXmlAttributeValue(rcnode, "uri", string.Empty));
                        break;
                    case "socketPool":
                        config.AutoStart = XmlHelper.GetXmlAttributeValue(item, "autoStart", true);
                        config.MaxReadPoolSize = XmlHelper.GetXmlAttributeValue(item, "maxReadPoolSize", 5);
                        config.MaxWritePoolSize = XmlHelper.GetXmlAttributeValue(item, "maxWritePoolSize", 5);
                        break;
                }
            }
            return new PooledRedisClientManager(readWriteHosts, readOnlyHosts, config);
        }
        #endregion
    }
}

using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Runtime.Caching;
using System.ServiceModel.Configuration;
using System.ServiceModel.Description;
using System.Xml;
using Couchbase;
using Couchbase.Configuration;
using MongoDB.Driver;
using X.Util.Core.Cache;
using X.Util.Core.Kernel;
using X.Util.Core.Xml;
using X.Util.Entities;
using X.Util.Entities.Enums;

namespace X.Util.Core.Configuration
{
    public class ConfigurationHelper
    {
        private const string CacheConfigurationPrefix = "X.Util.Core.Configuration.CacheConfigurationPrefix";
        public const int MaxPoolSize = 1;
        public const string MongoDefaultServername = "XMongo";
        public const string RedisDefaultServername = "XRedis";
        public const string CouchDefaultServername = "XCouch";
        public static string EndpointFile
        {
            get { return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data\\endpoint.xml"); }
        }
        public static string ConfigFile
        {
            get { return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data\\Config.xml"); }
        }

        public static string EndpointFileModified
        {
            get { return new FileInfo(EndpointFile).LastWriteTime.Ticks.ToString(); }
        }

        public static string ConfigFileModified
        {
            get { return new FileInfo(EndpointFile).LastWriteTime.Ticks.ToString(); }
        }

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
            var result = LocalCache.Default.Get<string>(key);
            if (!string.IsNullOrWhiteSpace(result)) return result;
            result = defaultValue;
            var doc = XmlHelper.GetXmlDocCache(ConfigFile);
            if (Equals(doc, null)) return result;
            var node = doc.SelectSingleNode("/configuration/AppSettings/add[@key='" + name + "']");
            result = XmlHelper.GetXmlAttributeValue(node, "value", defaultValue);
            LocalCache.Default.SlidingExpirationSet(key, result, new TimeSpan(1, 0, 0), new HostFileChangeMonitor(new[] { ConfigFile }));
            return result;
        }

        public static void UpdateAppSettingByName(string name, string value)
        {
            var doc = XmlHelper.GetXmlDocCache(ConfigFile);
            if (Equals(doc, null)) return;
            var node = doc.SelectSingleNode("/configuration/AppSettings/add[@key='" + name + "']");
            if (node != null && node.Attributes != null)
            {
                var attr = node.Attributes["value"] ?? doc.CreateAttribute("value");
                attr.Value = value;
                node.Attributes.SetNamedItem(attr);
            }
            doc.Save(ConfigFile);
        }

        public static byte GetAppSettingByName(string name, byte defaultValue)
        {
            return GetAppSettingByName(name, string.Empty).Convert2Byte(defaultValue);
        }

        public static bool GetAppSettingByName(string name, bool defaultValue)
        {
            return GetAppSettingByName(name, string.Empty).Convert2Boolean(defaultValue);
        }

        public static short GetAppSettingByName(string name, short defaultValue)
        {
            return GetAppSettingByName(name, string.Empty).Convert2Int16(defaultValue);
        }

        public static int GetAppSettingByName(string name, int defaultValue)
        {
            return GetAppSettingByName(name, string.Empty).Convert2Int32(defaultValue);
        }

        public static long GetAppSettingByName(string name, long defaultValue)
        {
            return GetAppSettingByName(name, string.Empty).Convert2Int64(defaultValue);
        }

        public static float GetAppSettingByName(string name, float defaultValue)
        {
            return GetAppSettingByName(name, string.Empty).Convert2Single(defaultValue);
        }

        public static double GetAppSettingByName(string name, double defaultValue)
        {
            return GetAppSettingByName(name, string.Empty).Convert2Double(defaultValue);
        }

        public static decimal GetAppSettingByName(string name, decimal defaultValue)
        {
            return GetAppSettingByName(name, string.Empty).Convert2Decimal(defaultValue);
        }

        public static DateTime GetAppSettingByName(string name, DateTime defaultValue)
        {
            return GetAppSettingByName(name, string.Empty).Convert2DateTime(defaultValue);
        }
        #endregion

        #region Get WCF EndpointAddress Configuration
        public static XmlServiceModel GetEndpointAddressesByName(string name)
        {
            var zone = Math.Max(ExecutionContext<RequestContext>.Current.Zone, 1);
            var key = CacheConfigurationPrefix + zone + name + "GetEndpointAddressesByName";
            var result = LocalCache.Default.Get<XmlServiceModel>(key);
            if (result != null) return result;
            var doc = XmlHelper.GetXmlDocCache(EndpointFile);
            var node = doc != null ? doc.SelectSingleNode("/configuration/client/endpoint[@name='" + name + "' and @zone='" + zone + "']") : null;
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
            LocalCache.Default.SlidingExpirationSet(key, result, new TimeSpan(1, 0, 0), new HostFileChangeMonitor(new[] { EndpointFile }));
            return result;
        }

        /// <summary>
        /// 根据元数据发布地址生成代理类
        /// </summary>
        /// <param name="address">元数据地址</param>
        /// <param name="mode">交换元数据方式</param>
        /// <param name="outPutProxyFile">代理文件路径</param>
        /// <param name="outPutConfigFile">配置文件路径</param>
        private static void GenerateWCfProxyAndConfig(string address, Entities.MetadataExchangeClientMode mode, string outPutProxyFile, string outPutConfigFile)
        {
            var mexClient = new MetadataExchangeClient(new Uri(address), (System.ServiceModel.Description.MetadataExchangeClientMode) mode);
            var metadataSet = mexClient.GetMetadata();
            var importer = new WsdlImporter(metadataSet);
            var codeCompileUnit = new CodeCompileUnit();
            var config = ConfigurationManager.OpenMappedExeConfiguration(new ExeConfigurationFileMap { ExeConfigFilename = outPutConfigFile }, ConfigurationUserLevel.None);
            var generator = new ServiceContractGenerator(codeCompileUnit, config);
            foreach (var endpoint in importer.ImportAllEndpoints())
            {
                generator.GenerateServiceContractType(endpoint.Contract);
                ChannelEndpointElement element;
                generator.GenerateServiceEndpoint(endpoint, out element);
            }
            generator.Configuration.Save();
            var provider = CodeDomProvider.CreateProvider("CSharp");
            using (var sw = new StreamWriter(outPutProxyFile))
            {
                var textWriter = new IndentedTextWriter(sw);
                var options = new CodeGeneratorOptions();
                provider.GenerateCodeFromCompileUnit(codeCompileUnit, textWriter, options);
            }
        }

        /// <summary>
        /// 根据元数据发布地址生成代理类
        /// </summary>
        /// <param name="config"></param>
        public static void GenerateWCfProxyAndConfig(WCfConfig config)
        {
            if (config == null) return;
            var file = new FileInfo(config.ConfigPathPath);
            if (file.Exists)
            {
                file.Delete();
            }
            foreach (var item in config.ProxyList)
            {
                GenerateWCfProxyAndConfig(item.Address, item.Mode, item.ProxyFilePath, config.ConfigPathPath);
            }
        }
        #endregion

        #region CouchBase And MongoDB Configuration
        public static CouchbaseClientConfiguration CouchbaseConfiguration(string serverName)
        {
            var zone = Math.Max(ExecutionContext<RequestContext>.Current.Zone, 1);
            var couchbaseClientConfiguration = new CouchbaseClientConfiguration();
            var doc = XmlHelper.GetXmlDocCache(EndpointFile);
            if (Equals(doc, null)) return couchbaseClientConfiguration;
            var node = doc.SelectSingleNode("/configuration/couchbase[@zone='" + zone + "']/server[@name='" + serverName + "']");
            if (Equals(node, null)) return couchbaseClientConfiguration;
            couchbaseClientConfiguration.Bucket = XmlHelper.GetXmlAttributeValue(node, "bucket", "default");
            couchbaseClientConfiguration.BucketPassword = XmlHelper.GetXmlAttributeValue(node, "password", string.Empty);
            foreach (var item in node.ChildNodes.Cast<XmlNode>().Where(uri => uri.NodeType != XmlNodeType.Comment))
            {
                switch (item.Name)
                {
                    case "urls":
                        foreach (var uri in item.ChildNodes.Cast<XmlNode>().Where(uri => uri.NodeType != XmlNodeType.Comment)) couchbaseClientConfiguration.Urls.Add(new Uri(XmlHelper.GetXmlAttributeValue(uri, "uri", string.Empty)));
                        break;
                    case "socketPool":
                        couchbaseClientConfiguration.SocketPool.MinPoolSize = XmlHelper.GetXmlAttributeValue(item, "minPoolSize", string.Empty).Convert2Int32(10);
                        couchbaseClientConfiguration.SocketPool.MaxPoolSize = XmlHelper.GetXmlAttributeValue(item, "maxPoolSize", string.Empty).Convert2Int32(20);
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
            configuration.SocketPool.MaxPoolSize = 20;
            configuration.SocketPool.MinPoolSize = 10;
            return new CouchbaseClient(configuration);
        }

        public static IEnumerable<MongoCredential> GetMongoCredential(string userName, string password, string credentialDataBase, MongoCredentialType type)
        {
            if (string.IsNullOrEmpty(credentialDataBase)) credentialDataBase = "admin";
            return new List<MongoCredential>
            {
                MongoCredentialType.Plain == type
                    ? MongoCredential.CreatePlainCredential(credentialDataBase, userName, password)
                    : MongoCredentialType.ScramSha1 == type
                        ? MongoCredential.CreateScramSha1Credential(credentialDataBase, userName, password)
                        : MongoCredential.CreateMongoCRCredential(credentialDataBase, userName, password)
            };
        }

        /// <summary>
        /// MongoDb Configuration
        /// </summary>
        public static MongoClientSettings MongoClientConfiguration(string serverName, string credentialDataBase = null)
        {
            var zone = Math.Max(ExecutionContext<RequestContext>.Current.Zone, 1);
            var configuration = new MongoClientSettings();
            var doc = XmlHelper.GetXmlDocCache(EndpointFile);
            if (Equals(doc, null)) return configuration;
            var node = doc.SelectSingleNode("/configuration/mongodb[@zone='" + zone + "']/server[@name='" + serverName + "']");
            if (Equals(node, null)) return configuration;
            var servers = new List<MongoServerAddress>();
            var userName = XmlHelper.GetXmlAttributeValue(node, "username", string.Empty);
            var password = XmlHelper.GetXmlAttributeValue(node, "password", string.Empty);
            if (!string.IsNullOrWhiteSpace(userName) && !string.IsNullOrWhiteSpace(password))
            {
                var type = (MongoCredentialType)Enum.Parse(typeof(MongoCredentialType), XmlHelper.GetXmlAttributeValue(node, "credential", "MongoCr"), true);
                configuration.Credentials = GetMongoCredential(userName, password, credentialDataBase ?? XmlHelper.GetXmlAttributeValue(node, "db", "admin"), type);
            }
            foreach (var item in node.ChildNodes.Cast<XmlNode>().Where(uri => uri.NodeType != XmlNodeType.Comment))
            {
                switch (item.Name)
                {
                    case "add":
                        var uri = new Uri(XmlHelper.GetXmlAttributeValue(item, "uri", "mongodb://127.0.0.1:27017"));
                        servers.Add(new MongoServerAddress(uri.Host, uri.Port));
                        break;
                    case "socketPool":
                        configuration.MaxConnectionPoolSize = XmlHelper.GetXmlAttributeValue(item, "maxConnectionPoolSize", 50);
                        configuration.MinConnectionPoolSize = XmlHelper.GetXmlAttributeValue(item, "minConnectionPoolSize", 10);
                        configuration.WaitQueueSize = XmlHelper.GetXmlAttributeValue(item, "waitQueueSize", 10000);
                        configuration.ConnectTimeout = TimeSpan.Parse(XmlHelper.GetXmlAttributeValue(item, "connectTimeout", "00:00:30"));
                        configuration.MaxConnectionIdleTime = TimeSpan.Parse(XmlHelper.GetXmlAttributeValue(item, "maxConnectionIdleTime", "00:10:00"));
                        configuration.MaxConnectionLifeTime = TimeSpan.Parse(XmlHelper.GetXmlAttributeValue(item, "maxConnectionLifeTime", "00:30:00"));
                        configuration.SocketTimeout = TimeSpan.Parse(XmlHelper.GetXmlAttributeValue(item, "socketTimeout", "00:00:10"));
                        configuration.WaitQueueTimeout = TimeSpan.Parse(XmlHelper.GetXmlAttributeValue(item, "waitQueueTimeout", "00:02:00"));
                        break;
                }
            }
            configuration.Servers = servers;
            return configuration;
        }

        /// <summary>
        /// MongoDb Configuration
        /// </summary>
        /// <returns></returns>
        public static MongoClient MongoClientConfiguration(IEnumerable<Uri> servers, string userName, string password, string credentialDataBase, MongoCredentialType type)
        {
            var configuration = new MongoClientSettings
            {
                MaxConnectionPoolSize = 10,
                MinConnectionPoolSize = 1,
                WaitQueueSize = 10000,
                Servers = servers.Select(uri => new MongoServerAddress(uri.Host, uri.Port))
            };
            if (!string.IsNullOrWhiteSpace(userName) && !string.IsNullOrWhiteSpace(password)) configuration.Credentials = GetMongoCredential(userName, password, credentialDataBase, type);
            return new MongoClient(configuration);
        }

        /// <summary>
        /// Redis 链接字符串配置
        /// </summary>
        /// <param name="connectName"></param>
        /// <returns></returns>
        public static string GetRedisConnectString(string connectName)
        {
            var doc = XmlHelper.GetXmlDocCache(EndpointFile);
            if (Equals(doc, null)) return string.Empty;
            var node = doc.SelectSingleNode("/configuration/redis/server[@name='" + connectName + "']");
            return Equals(node, null) ? string.Empty : XmlHelper.GetXmlNodeValue(node, string.Empty);
        }

        #endregion
    }
}

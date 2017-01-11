using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.ServiceModel.Configuration;
using System.Xml;
using X.Util.Core;
using X.Util.Core.Log;
using X.Util.Core.Xml;
using X.Util.Entities.Enums;

namespace X.WFConfig
{
    public class OperateHelper
    {
        private static readonly string AppConfig = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "app.config");
        private static readonly string Dir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data\\");
        private static readonly string AppSettingsConfig = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data\\Config.xml");
        private static readonly string AppConfigCopy = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data\\app.xml");

        #region 内部实现
        private static string GetEndpointPath(int zone)
        {
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data\\endpoint_" + zone + ".xml");
        }

        /// <summary>
        /// 确保目录存在
        /// </summary>
        /// <param name="path"></param>
        private static void EnSureDirExists(string path)
        {
            try
            {
                var dir = new DirectoryInfo(path);
                if (!dir.Exists) dir.Create();
            }
            catch (Exception ex)
            {
                Logger.Client.Error(Logger.Client.GetMethodInfo(MethodBase.GetCurrentMethod(), new object[] { path }), ex, LogDomain.Util);
            }
        }

        /// <summary>
        /// 复制文件
        /// </summary>
        /// <param name="src"></param>
        /// <param name="dest"></param>
        /// <param name="overWrite"></param>
        /// <returns></returns>
        private static void Copy(string src, string dest, bool overWrite = false)
        {
            try
            {
                var file = new FileInfo(src);
                if (!file.Exists) return;
                file.CopyTo(dest, overWrite);
            }
            catch (Exception ex)
            {
                Logger.Client.Error(Logger.Client.GetMethodInfo(MethodBase.GetCurrentMethod(), new object[] { src, dest, overWrite }), ex, LogDomain.Util);
            }
        }

        /// <summary>
        /// 初始化XmlDocument
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        private static XmlDocument GetXmlDocument(XmlDocument doc, string name)
        {
            //configuration
            if (doc != null) return doc;
            doc = new XmlDocument();
            var xd = doc.CreateXmlDeclaration("1.0", "utf-8", null);
            doc.AppendChild(xd);
            var root = doc.CreateElement(name);
            doc.AppendChild(root);
            return doc;
        } 
        #endregion

        #region wcf配置文件
        /// <summary>
        /// 获取wcf配置信息
        /// </summary>
        /// <returns></returns>
        public static List<WcfBindingConfig> GetBindingConfigs()
        {
            EnSureDirExists(Dir);
            var file = new FileInfo(AppConfigCopy);
            if (!file.Exists) Copy(AppConfig, AppConfigCopy);
            var map = new ExeConfigurationFileMap { ExeConfigFilename = AppConfigCopy };
            var config = ConfigurationManager.OpenMappedExeConfiguration(map, ConfigurationUserLevel.None);
            var group = ServiceModelSectionGroup.GetSectionGroup(config);
            if (@group == null) return null;
            return (from ChannelEndpointElement endpoint in @group.Client.Endpoints
                    select new WcfBindingConfig
                    {
                        BindingConfiguration = endpoint.BindingConfiguration,
                        Contract = endpoint.Contract,
                        Address = endpoint.Address
                    }).ToList();
        }

        /// <summary>
        /// 设置wcf绑定配置信息
        /// </summary>
        /// <param name="list"></param>
        public static void SetBindingConfigs(List<WcfBindingConfig> list)
        {
            var map = new ExeConfigurationFileMap { ExeConfigFilename = AppConfigCopy };
            var config = ConfigurationManager.OpenMappedExeConfiguration(map, ConfigurationUserLevel.None);
            var group = ServiceModelSectionGroup.GetSectionGroup(config);
            if (@group == null) return;
            foreach (ChannelEndpointElement endpoint in @group.Client.Endpoints)
            {
                var item = list.FirstOrDefault(p => p.BindingConfiguration == endpoint.BindingConfiguration);
                if (item == null) continue;
                endpoint.Contract = item.Contract;
                endpoint.Address = item.Address;
            }
            config.Save();
        } 
        #endregion

        #region MongoDb
        /// <summary>
        /// 获取MongoDb在配置文件中的节点
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="mongodb"></param>
        /// <returns></returns>
        private static XmlElement GetMongoDbElement(XmlDocument doc, MongoDbConfig mongodb)
        {
            if (mongodb.MaxConnectionPoolSize == default(int)) mongodb.MaxConnectionPoolSize = 50;
            if (mongodb.MinConnectionPoolSize == default(int)) mongodb.MinConnectionPoolSize = 10;
            if (mongodb.WaitQueueSize == default(int)) mongodb.WaitQueueSize = 10000;
            if (mongodb.ConnectTimeout == default(TimeSpan)) mongodb.ConnectTimeout = new TimeSpan(0, 0, 30);
            if (mongodb.MaxConnectionIdleTime == default(TimeSpan)) mongodb.MaxConnectionIdleTime = new TimeSpan(0, 10, 0);
            if (mongodb.MaxConnectionLifeTime == default(TimeSpan)) mongodb.MaxConnectionLifeTime = new TimeSpan(0, 30, 0);
            if (mongodb.SocketTimeout == default(TimeSpan)) mongodb.SocketTimeout = new TimeSpan(0, 0, 0);
            if (mongodb.WaitQueueTimeout == default(TimeSpan)) mongodb.WaitQueueTimeout = new TimeSpan(0, 2, 0);
            var serverElement = doc.CreateElement("server");
            serverElement.SetAttribute("name", mongodb.Name);
            serverElement.SetAttribute("username", mongodb.UserName);
            serverElement.SetAttribute("password", mongodb.PassWord);
            if (mongodb.Uris != null && mongodb.Uris.Count > 0)
            {
                foreach (var uri in mongodb.Uris)
                {
                    var addElement = doc.CreateElement("add");
                    addElement.SetAttribute("uri", uri.ToString());
                    serverElement.AppendChild(addElement);
                }
            }
            var socketPoolElement = doc.CreateElement("socketPool");
            socketPoolElement.SetAttribute("maxConnectionPoolSize", mongodb.MaxConnectionPoolSize.ToString());
            socketPoolElement.SetAttribute("minConnectionPoolSize", mongodb.MinConnectionPoolSize.ToString());
            socketPoolElement.SetAttribute("waitQueueSize", mongodb.WaitQueueSize.ToString());
            socketPoolElement.SetAttribute("connectTimeout", mongodb.ConnectTimeout.ToString());
            socketPoolElement.SetAttribute("maxConnectionIdleTime", mongodb.MaxConnectionIdleTime.ToString());
            socketPoolElement.SetAttribute("maxConnectionLifeTime", mongodb.MaxConnectionLifeTime.ToString());
            socketPoolElement.SetAttribute("socketTimeout", mongodb.SocketTimeout.ToString());
            socketPoolElement.SetAttribute("waitQueueTimeout", mongodb.WaitQueueTimeout.ToString());
            serverElement.AppendChild(socketPoolElement);
            return serverElement;
        }

        /// <summary>
        /// 获取MongoDb在配置文件中的节点
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="mongodbList"></param>
        /// <returns></returns>
        private static IEnumerable<XmlElement> GetMongoDbElement(XmlDocument doc, ICollection<MongoDbConfig> mongodbList)
        {
            var list = new List<XmlElement>();
            if (mongodbList != null && mongodbList.Count > 0)
            {
                list.AddRange(mongodbList.Select(mongodb => GetMongoDbElement(doc, mongodb)));
            }
            return list;
        }

        /// <summary>
        /// MognoDb配置
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="mongodb"></param>
        /// <returns></returns>
        public static XmlDocument SetMongoDbConfig(XmlDocument doc, MongoDbConfig mongodb)
        {
            doc = GetXmlDocument(doc, "configuration");
            if (mongodb == null) return doc;
            var mongoElement = doc.CreateElement("mongodb");
            var serverElement = GetMongoDbElement(doc, mongodb);
            mongoElement.AppendChild(serverElement);
            doc.AppendChild(mongoElement);
            return doc;
        }

        /// <summary>
        /// MognoDb配置
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="mongodbList"></param>
        /// <returns></returns>
        public static XmlDocument SetMongoDbConfig(XmlDocument doc, ICollection<MongoDbConfig> mongodbList)
        {
            doc = GetXmlDocument(doc, "configuration");
            if (mongodbList == null || mongodbList.Count == 0) return doc;
            var mongoElement = doc.CreateElement("mongodb");
            var elemests = GetMongoDbElement(doc, mongodbList);
            foreach (var serverElement in elemests)
            {
                mongoElement.AppendChild(serverElement);
            }
            doc.AppendChild(mongoElement);
            return doc;
        }

        /// <summary>
        /// 读取MognoDb配置
        /// </summary>
        /// <param name="zone"></param>
        /// <returns></returns>
        public static List<MongoDbConfig> GetMongoDbConfig(int zone)
        {
            var result = new List<MongoDbConfig>();
            var path = GetEndpointPath(zone);
            var doc = XmlHelper.GetXmlDocCache(path);
            var nodes = doc.SelectNodes("/configuration/mongodb/server");
            if (nodes == null || nodes.Count <= 0) return result;
            foreach (XmlNode node in nodes)
            {
                var mongoDb = new MongoDbConfig
                {
                    Name = XmlHelper.GetXmlAttributeValue(node, "name", string.Empty),
                    UserName = XmlHelper.GetXmlAttributeValue(node, "username", string.Empty),
                    PassWord = XmlHelper.GetXmlAttributeValue(node, "password", string.Empty),
                    Uris = new List<Uri>()
                };
                foreach (XmlNode item in node.ChildNodes)
                {
                    switch (item.Name)
                    {
                        case "add":
                            var uri = new Uri(XmlHelper.GetXmlAttributeValue(item, "uri", "mongodb://127.0.0.1:27017"));
                            mongoDb.Uris.Add(uri);
                            break;
                        case "socketPool":
                            mongoDb.MaxConnectionPoolSize = XmlHelper.GetXmlAttributeValue(item, "maxConnectionPoolSize", 50);
                            mongoDb.MinConnectionPoolSize = XmlHelper.GetXmlAttributeValue(item, "minConnectionPoolSize", 10);
                            mongoDb.WaitQueueSize = XmlHelper.GetXmlAttributeValue(item, "waitQueueSize", 10000);
                            mongoDb.ConnectTimeout = TimeSpan.Parse(XmlHelper.GetXmlAttributeValue(item, "connectTimeout", "00:00:30"));
                            mongoDb.MaxConnectionIdleTime = TimeSpan.Parse(XmlHelper.GetXmlAttributeValue(item, "maxConnectionIdleTime", "00:10:00"));
                            mongoDb.MaxConnectionLifeTime = TimeSpan.Parse(XmlHelper.GetXmlAttributeValue(item, "maxConnectionLifeTime", "00:30:00"));
                            mongoDb.SocketTimeout = TimeSpan.Parse(XmlHelper.GetXmlAttributeValue(item, "socketTimeout", "00:00:00"));
                            mongoDb.WaitQueueTimeout = TimeSpan.Parse(XmlHelper.GetXmlAttributeValue(item, "waitQueueTimeout", "00:02:00"));
                            break;
                    }
                }
                result.Add(mongoDb);
            }
            return result;


        }
        #endregion

        #region Couchbase
        /// <summary>
        /// 获取Couchbase在配置文件中的节点
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="couchbase"></param>
        /// <returns></returns>
        private static XmlElement GetCouchbaseElement(XmlDocument doc, CouchbaseConfig couchbase)
        {
            if (couchbase.MinPoolSize == default(int)) couchbase.MinPoolSize = 10;
            if (couchbase.MaxPoolSize == default(int)) couchbase.MaxPoolSize = 20;
            if (couchbase.ConnectionTimeout == default(TimeSpan)) couchbase.ConnectionTimeout = new TimeSpan(0, 0, 2);
            if (couchbase.DeadTimeout == default(TimeSpan)) couchbase.DeadTimeout = new TimeSpan(0, 0, 10);
            if (couchbase.ReceiveTimeout == default(TimeSpan)) couchbase.ReceiveTimeout = new TimeSpan(0, 0, 10);
            if (couchbase.QueueTimeout == default(TimeSpan)) couchbase.QueueTimeout = TimeSpan.Parse("00:00:00.1000000");
            var serverElement = doc.CreateElement("server");
            serverElement.SetAttribute("name", couchbase.Name);
            serverElement.SetAttribute("bucket", couchbase.Bucket ?? "default");
            serverElement.SetAttribute("password", couchbase.PassWord);
            if (couchbase.Uris != null && couchbase.Uris.Count > 0)
            {
                var urlsElement = doc.CreateElement("urls");
                foreach (var uri in couchbase.Uris)
                {
                    var addElement = doc.CreateElement("add");
                    addElement.SetAttribute("uri", uri.ToString());
                    urlsElement.AppendChild(addElement);
                }
                serverElement.AppendChild(urlsElement);
            }
            var socketPoolElement = doc.CreateElement("socketPool");
            socketPoolElement.SetAttribute("minPoolSize", couchbase.MinPoolSize.ToString());
            socketPoolElement.SetAttribute("maxPoolSize", couchbase.MaxPoolSize.ToString());
            socketPoolElement.SetAttribute("connectionTimeout", couchbase.ConnectionTimeout.ToString());
            socketPoolElement.SetAttribute("deadTimeout", couchbase.DeadTimeout.ToString());
            socketPoolElement.SetAttribute("receiveTimeout", couchbase.ReceiveTimeout.ToString());
            socketPoolElement.SetAttribute("queueTimeout", couchbase.QueueTimeout.ToString());
            serverElement.AppendChild(socketPoolElement);
            return serverElement;
        }

        /// <summary>
        /// 获取Couchbase在配置文件中的节点
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="couchbaseList"></param>
        /// <returns></returns>
        private static IEnumerable<XmlElement> GetCouchbaseElement(XmlDocument doc, ICollection<CouchbaseConfig> couchbaseList)
        {
            var list = new List<XmlElement>();
            if (couchbaseList != null && couchbaseList.Count > 0)
            {
                list.AddRange(couchbaseList.Select(couchbase => GetCouchbaseElement(doc, couchbase)));
            }
            return list;
        }

        /// <summary>
        /// CouchBase 配置
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="couchbase"></param>
        /// <returns></returns>
        public static XmlDocument SetCouchbaseConfig(XmlDocument doc, CouchbaseConfig couchbase)
        {
            doc = GetXmlDocument(doc, "configuration");
            if (couchbase == null) return doc;
            var couchbaseElement = doc.CreateElement("couchbase");
            var serverElement = GetCouchbaseElement(doc, couchbase);
            couchbaseElement.AppendChild(serverElement);
            doc.AppendChild(couchbaseElement);
            return doc;
        }

        /// <summary>
        /// CouchBase 配置
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="couchbaseList"></param>
        /// <returns></returns>
        public static XmlDocument SetCouchbaseConfig(XmlDocument doc, ICollection<CouchbaseConfig> couchbaseList)
        {
            doc = GetXmlDocument(doc, "configuration");
            if (couchbaseList == null || couchbaseList.Count == 0) return doc;
            var couchbaseElement = doc.CreateElement("couchbase");
            var elemests = GetCouchbaseElement(doc, couchbaseList);
            foreach (var serverElement in elemests)
            {
                couchbaseElement.AppendChild(serverElement);
            }
            doc.AppendChild(couchbaseElement);
            return doc;
        }

        /// <summary>
        /// 读取couchbase配置
        /// </summary>
        /// <param name="zone"></param>
        /// <returns></returns>
        public static List<CouchbaseConfig> GetCouchbaseConfig(int zone)
        {
            var result = new List<CouchbaseConfig>();
            var path = GetEndpointPath(zone);
            var doc = XmlHelper.GetXmlDocCache(path);
            var nodes = doc.SelectNodes("/configuration/couchbase/server");
            if (nodes == null || nodes.Count <= 0) return result;
            foreach (XmlNode node in nodes)
            {
                var couchbase = new CouchbaseConfig
                {
                    Name = XmlHelper.GetXmlAttributeValue(node, "name", string.Empty),
                    Bucket = XmlHelper.GetXmlAttributeValue(node, "bucket", string.Empty),
                    PassWord = XmlHelper.GetXmlAttributeValue(node, "password", string.Empty),
                    Uris = new List<Uri>()
                };
                foreach (XmlNode item in node.ChildNodes)
                {
                    switch (item.Name)
                    {
                        case "urls":
                            foreach (XmlNode uri in item.ChildNodes) couchbase.Uris.Add(new Uri(XmlHelper.GetXmlAttributeValue(uri, "uri", string.Empty)));
                            break;
                        case "socketPool":
                            couchbase.MinPoolSize = XmlHelper.GetXmlAttributeValue(item, "minPoolSize", string.Empty).GetInt32(20);
                            couchbase.MaxPoolSize = XmlHelper.GetXmlAttributeValue(item, "maxPoolSize", string.Empty).GetInt32(1000);
                            couchbase.ConnectionTimeout = TimeSpan.Parse(XmlHelper.GetXmlAttributeValue(item, "connectionTimeout", "00:00:02"));
                            couchbase.DeadTimeout = TimeSpan.Parse(XmlHelper.GetXmlAttributeValue(item, "deadTimeout", "00:00:10"));
                            couchbase.ReceiveTimeout = TimeSpan.Parse(XmlHelper.GetXmlAttributeValue(item, "receiveTimeout", "00:00:10"));
                            couchbase.QueueTimeout = TimeSpan.Parse(XmlHelper.GetXmlAttributeValue(item, "queueTimeout", "00:00:00.1000000"));
                            break;
                    }
                }
                result.Add(couchbase);
            }
            return result;
        }

        #endregion

        #region Redis
        /// <summary>
        /// 获取Redis在配置文件中的节点
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="redis"></param>
        /// <returns></returns>
        private static XmlElement GetRedisElement(XmlDocument doc, RedisConfig redis)
        {
            if (redis.MaxReadPoolSize == default(int)) redis.MaxReadPoolSize = 5;
            if (redis.MaxWritePoolSize == default(int)) redis.MaxWritePoolSize = 5;
            var serverElement = doc.CreateElement("server");
            serverElement.SetAttribute("name", redis.Name);
            if (redis.ReadUris != null && redis.ReadUris.Count > 0)
            {
                var readElement = doc.CreateElement("read");
                foreach (var uri in redis.ReadUris)
                {
                    var addElement = doc.CreateElement("add");
                    addElement.SetAttribute("uri", uri.ToString());
                    readElement.AppendChild(addElement);
                }
                serverElement.AppendChild(readElement);
            }
            if (redis.WriteUris != null && redis.WriteUris.Count > 0)
            {
                var writeElement = doc.CreateElement("write");
                foreach (var uri in redis.WriteUris)
                {
                    var addElement = doc.CreateElement("add");
                    addElement.SetAttribute("uri", uri.ToString());
                    writeElement.AppendChild(addElement);
                }
                serverElement.AppendChild(writeElement);
            }
            var socketPoolElement = doc.CreateElement("socketPool");
            socketPoolElement.SetAttribute("maxReadPoolSize", redis.MaxReadPoolSize.ToString());
            socketPoolElement.SetAttribute("maxWritePoolSize", redis.MaxWritePoolSize.ToString());
            socketPoolElement.SetAttribute("autoStart", redis.AutoStart.ToString());
            serverElement.AppendChild(socketPoolElement);
            return serverElement;
        }

        /// <summary>
        /// 获取Redis在配置文件中的节点
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="redisList"></param>
        /// <returns></returns>
        private static IEnumerable<XmlElement> GetRedisElement(XmlDocument doc, ICollection<RedisConfig> redisList)
        {
            var list = new List<XmlElement>();
            if (redisList != null && redisList.Count > 0)
            {
                list.AddRange(redisList.Select(redis => GetRedisElement(doc, redis)));
            }
            return list;
        }

        /// <summary>
        /// Redis 配置
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="redis"></param>
        /// <returns></returns>
        public static XmlDocument SetRedisConfig(XmlDocument doc, RedisConfig redis)
        {
            doc = GetXmlDocument(doc, "configuration");
            if (redis == null) return doc;
            var serverElement = GetRedisElement(doc, redis);
            var redisElement = doc.CreateElement("redis");
            redisElement.AppendChild(serverElement);
            doc.AppendChild(redisElement);
            return doc;
        }

        /// <summary>
        /// Redis 配置
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="redisList"></param>
        /// <returns></returns>
        public static XmlDocument SetRedisConfig(XmlDocument doc, ICollection<RedisConfig> redisList)
        {
            doc = GetXmlDocument(doc, "configuration");
            if (redisList == null || redisList.Count == 0) return doc;
            var elemests = GetRedisElement(doc, redisList);
            var redisElement = doc.CreateElement("redis");
            foreach (var serverElement in elemests)
            {
                redisElement.AppendChild(serverElement);
            }
            doc.AppendChild(redisElement);
            return doc;
        }

        /// <summary>
        /// 读取Redis配置
        /// </summary>
        /// <param name="zone"></param>
        /// <returns></returns>
        public static List<RedisConfig> GetRedisConfig(int zone)
        {
            var result = new List<RedisConfig>();
            var path = GetEndpointPath(zone);
            var doc = XmlHelper.GetXmlDocCache(path);
            var nodes = doc.SelectNodes("/configuration/redis/server");
            if (nodes == null || nodes.Count <= 0) return result;
            foreach (XmlNode node in nodes)
            {
                var redis = new RedisConfig
                {
                    Name = XmlHelper.GetXmlAttributeValue(node, "name", string.Empty),
                    ReadUris = new List<Uri>(),
                    WriteUris = new List<Uri>()
                };
                foreach (XmlNode item in node.ChildNodes)
                {
                    switch (item.Name)
                    {
                        case "read":
                            redis.ReadUris.AddRange(from XmlNode rcnode in item.ChildNodes select new Uri(XmlHelper.GetXmlAttributeValue(rcnode, "uri", string.Empty)));
                            break;
                        case "write":
                            redis.WriteUris.AddRange(from XmlNode rcnode in item.ChildNodes select new Uri(XmlHelper.GetXmlAttributeValue(rcnode, "uri", string.Empty)));
                            break;
                        case "socketPool":
                            redis.AutoStart = XmlHelper.GetXmlAttributeValue(item, "autoStart", true);
                            redis.MaxReadPoolSize = XmlHelper.GetXmlAttributeValue(item, "maxReadPoolSize", 5);
                            redis.MaxWritePoolSize = XmlHelper.GetXmlAttributeValue(item, "maxWritePoolSize", 5);
                            break;
                    }
                }
                result.Add(redis);
            }
            return result;
        }


        #endregion

        #region wcf终结点地址配置
        /// <summary>
        /// Wcf配置节点
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="configs"></param>
        /// <returns></returns>
        private static XmlDocument SetWcfAddressConfig(XmlDocument doc, ICollection<WcfAddressConfig> configs)
        {
            doc = GetXmlDocument(doc, "configuration");
            var clientElement = doc.CreateElement("client");
            if (configs != null && configs.Count > 0)
            {
                foreach (var config in configs)
                {
                    if (config.Addresses == null || config.Addresses.Count <= 0) continue;
                    var endpointElement = doc.CreateElement("endpoint");
                    endpointElement.SetAttribute("name", config.Contract);
                    foreach (var address in config.Addresses)
                    {
                        var addressElement = doc.CreateElement("address");
                        addressElement.InnerText = address;
                        endpointElement.AppendChild(addressElement);
                    }
                    clientElement.AppendChild(endpointElement);
                }
            }
            doc.AppendChild(clientElement);
            return doc;
        }

        /// <summary>
        /// 读取wcf配置节点
        /// </summary>
        /// <param name="zone"></param>
        /// <returns></returns>
        public static List<WcfAddressConfig> GetWcfAddressConfig(int zone)
        {
            var result = new List<WcfAddressConfig>();
            var path = GetEndpointPath(zone);
            var doc = XmlHelper.GetXmlDocCache(path);
            var nodes = doc.SelectNodes("/configuration/client/endpoint");
            if (nodes == null || nodes.Count <= 0) return result;
            foreach (XmlNode node in nodes)
            {
                var item = new WcfAddressConfig
                {
                    Contract = XmlHelper.GetXmlAttributeValue(node, "name", string.Empty),
                    MaxPoolSize = XmlHelper.GetXmlAttributeValue(node, "maxPoolSize", 1),
                    Addresses = new List<string>()
                };
                foreach (var uri in node.ChildNodes.Cast<XmlNode>().Select(child => XmlHelper.GetXmlNodeValue(child, string.Empty)).Where(uri => !string.IsNullOrEmpty(uri)))
                {
                    item.Addresses.Add(uri);
                }
                result.Add(item);
            }
            return result;
        } 
        #endregion

        #region 终结点配置（包括且不限于wcf，Nosql）
        /// <summary>
        /// 配置终结点
        /// </summary>
        /// <param name="endpoint"></param>
        public static void SetEndpointConfig(EndpointConfig endpoint)
        {
            EnSureDirExists(Dir);
            var path = GetEndpointPath(endpoint.Zone);
            var doc = SetMongoDbConfig(null, endpoint.MongoDb);
            doc = SetCouchbaseConfig(doc, endpoint.Couchbase);
            doc = SetRedisConfig(doc, endpoint.Redis);
            doc = SetWcfAddressConfig(doc, endpoint.WcfAddresses);
            doc.Save(path);
        }

        /// <summary>
        /// 读取配置终结点
        /// </summary>
        /// <param name="zone"></param>
        /// <returns></returns>
        public static EndpointConfig GetEndpointConfig(int zone)
        {
            return new EndpointConfig
            {
                Couchbase = GetCouchbaseConfig(zone),
                MongoDb = GetMongoDbConfig(zone),
                Redis = GetRedisConfig(zone),
                WcfAddresses = GetWcfAddressConfig(zone)
            };
        } 
        #endregion

        #region 通用配置项
        /// <summary>
        /// 设置配置项
        /// </summary>
        /// <param name="configs"></param>
        public static void SetAppSettings(List<AppSettingsConfig> configs)
        {
            var doc = GetXmlDocument(null, "configuration");
            var appSettings = doc.CreateElement("AppSettings");
            if (configs != null && configs.Count > 0)
            {
                foreach (var item in configs)
                {
                    var add = doc.CreateElement("add");
                    add.SetAttribute("key", item.Key);
                    add.SetAttribute("value", item.Value);
                    add.SetAttribute("comment", item.Comment);
                    appSettings.AppendChild(add);
                }
            }
            doc.AppendChild(appSettings);
            doc.Save(AppSettingsConfig);
        }

        /// <summary>
        /// 获取配置文件
        /// </summary>
        /// <returns></returns>
        public static List<AppSettingsConfig> GetAppSettings()
        {
            var list = new List<AppSettingsConfig>();
            var doc = XmlHelper.GetXmlDocCache(AppSettingsConfig);
            var nodes = doc.SelectNodes("/configuration/AppSettings/add");
            if (nodes == null || nodes.Count <= 0) return list;
            list.AddRange(from XmlNode node in nodes
                          select new AppSettingsConfig
                          {
                              Key = XmlHelper.GetXmlAttributeValue(node, "key", string.Empty),
                              Value = XmlHelper.GetXmlAttributeValue(node, "value", string.Empty),
                              Comment = XmlHelper.GetXmlAttributeValue(node, "comment", string.Empty)
                          });
            return list;
        } 
        #endregion
    }
}

using System;
using System.Collections.Generic;

namespace X.WFConfig
{
    public class WcfBindingConfig
    {
        public string Contract { get; set; }

        public Uri Address { get; set; }

        public string BindingConfiguration { get; set; }
    }

    public class WcfAddressConfig
    {
        public string Contract { get; set; }

        public int MaxPoolSize { get; set; }

        public List<string> Addresses { get; set; }
    }

    public class MongoDbConfig
    {
        public string Name { get; set; }

        public List<Uri> Uris { get; set; } 

        public string UserName { get; set; }

        public string PassWord { get; set; }

        public int MaxConnectionPoolSize { get; set; }

        public int MinConnectionPoolSize { get; set; }

        public int WaitQueueSize { get; set; }

        public TimeSpan ConnectTimeout { get; set; }

        public TimeSpan MaxConnectionIdleTime { get; set; }

        public TimeSpan MaxConnectionLifeTime { get; set; }

        public TimeSpan SocketTimeout { get; set; }

        public TimeSpan WaitQueueTimeout { get; set; }
    }

    public class CouchbaseConfig
    {
        public string Name { get; set; }

        public List<Uri> Uris { get; set; } 

        public string Bucket { get; set; }

        public string PassWord { get; set; }

        public int MaxPoolSize { get; set; }

        public int MinPoolSize { get; set; }

        public TimeSpan ConnectionTimeout { get; set; }

        public TimeSpan DeadTimeout { get; set; }

        public TimeSpan ReceiveTimeout { get; set; }

        public TimeSpan QueueTimeout { get; set; }}

    public class RedisConfig
    {
        public string Name { get; set; }

        public List<Uri> ReadUris { get; set; }

        public List<Uri> WriteUris { get; set; }

        public int MaxReadPoolSize { get; set; }

        public int MaxWritePoolSize { get; set; }

        public bool AutoStart { get; set; }
    }

    public class EndpointConfig
    {
        public List<MongoDbConfig> MongoDb { get; set; }

        public List<CouchbaseConfig> Couchbase { get; set; }

        public List<RedisConfig> Redis { get; set; }

        public List<WcfAddressConfig> WcfAddresses { get; set; }

        public int Zone { get; set; }
    }

    public class AppSettingsConfig
    {
        public string Key { get; set; }

        public string Value { get; set; }

        public string Comment { get; set; }
    }
}

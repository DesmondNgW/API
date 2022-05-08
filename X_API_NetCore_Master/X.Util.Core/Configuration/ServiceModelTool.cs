using Microsoft.Extensions.Caching.Memory;
using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using X.Util.Core.Cache;
using X.Util.Core.Kernel;
using X.Util.Entities;
using SD.Toolkits.CoreWCF.Client.Configurations.Behaviors;
using SD.Toolkits.CoreWCF.Client.Configurations.Clients;
using SD.Toolkits.CoreWCF.Client.Configurations.Bindings;

namespace X.Util.Core.Configuration
{
    public class ServiceModelTool
    {
        private static readonly string AppConfigFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data\\app.xml");
        private const string EndpointPrefix = "X.Util.Core.Configuration.EndpointPrefix";

        public static ServiceModelSectionGroup ConfigInit()
        {
            var key = EndpointPrefix + AppConfigFile;
            var cache = LocalCache.Default.Get<ServiceModelSectionGroup>(key);
            if (cache != null) return cache;
            var map = new ExeConfigurationFileMap { ExeConfigFilename = AppConfigFile };
            var config = ConfigurationManager.OpenMappedExeConfiguration(map, ConfigurationUserLevel.None);
            ServiceModelSectionGroup.Initialize(config);
            cache = ServiceModelSectionGroup.Setting;
            LocalCache.Default.SlidingExpirationSet(key, cache, new TimeSpan(0, 1, 0), CacheItemPriority.Normal);
            return cache;
        }

        public static Binding CreateBinding(string bindingName, ServiceModelSectionGroup group)
        {
            StandardBindingElement configurationElement;
            return (configurationElement = group.Bindings.BasicHttpBinding.Bindings[bindingName]) != null
                || (configurationElement = group.Bindings.WSHttpBinding.Bindings[bindingName]) != null
                || (configurationElement = group.Bindings.NetHttpBinding.Bindings[bindingName]) != null
                || (configurationElement = group.Bindings.NetTcpBinding.Bindings[bindingName]) != null
                ? GetBinding(configurationElement) : null;
        }

        private static Binding GetBinding<T>(StandardBindingElement configurationElement) where T : Binding
        {
            var bind = configurationElement.CreateBinding();
            bind.SendTimeout = new TimeSpan(0, 10, 0);
            bind.ReceiveTimeout = new TimeSpan(0, 10, 0);
            bind.CloseTimeout = new TimeSpan(0, 0, 10);
            var names = new[] { "ReaderQuotas", "MaxBufferPoolSize", "MaxBufferSize", "MaxReceivedMessageSize" };
            foreach (var p in names.Select(t => typeof(T).GetProperty(t)).Where(p => p != null))
            {
                if ("ReaderQuotas".Equals(p.Name))
                {
                    var readerQuotas = new System.Xml.XmlDictionaryReaderQuotas
                    {
                        MaxArrayLength = int.MaxValue,
                        MaxBytesPerRead = int.MaxValue,
                        MaxDepth = int.MaxValue,
                        MaxStringContentLength = int.MaxValue,
                        MaxNameTableCharCount = int.MaxValue
                    };
                    p.SetValue(bind, readerQuotas, null);
                }
                else
                {
                    p.SetValue(bind, int.MaxValue, null);
                }
            }
            return bind;
        }

        private static Binding GetBinding(StandardBindingElement configurationElement)
        {
            Binding binding;
            return (configurationElement is NetHttpBindingElement && (binding = GetBinding<NetHttpBinding>(configurationElement)) != null)
                || (configurationElement is BasicHttpBindingElement && (binding = GetBinding<BasicHttpBinding>(configurationElement)) != null)
                || (configurationElement is NetTcpBindingElement && (binding = GetBinding<NetTcpBinding>(configurationElement)) != null)
                || (configurationElement is WSHttpBindingElement && (binding = GetBinding<WSHttpBinding>(configurationElement)) != null)
                ? binding : null;
        }

        public static void AddBehaviors(string behaviorConfiguration, ServiceEndpoint serviceEndpoint, ServiceModelSectionGroup group)
        {
            var behaviorElement = group.Behaviors.BehaviorConfigurations[behaviorConfiguration];
            foreach (EndpointBehaviorElement behavior in behaviorElement.EndpointBehaviors)
            {
                var extension = behavior.CreateEndpointBehavior();
                if (extension != null) serviceEndpoint.EndpointBehaviors.Add(behavior.CreateEndpointBehavior());
            }
        }

        #region [[endpoint unsupport: GetIdentity]] 
        //public static EndpointIdentity GetIdentity(IdentityElement element)
        //{
        //    var properties = element.ElementInformation.Properties;
        //    var userPrincipalName = properties["userPrincipalName"];
        //    if (userPrincipalName != null && userPrincipalName.ValueOrigin != PropertyValueOrigin.Default) return EndpointIdentity.CreateUpnIdentity(element.UserPrincipalName.Value);
        //    var servicePrincipalName = properties["servicePrincipalName"];
        //    if (servicePrincipalName != null && servicePrincipalName.ValueOrigin != PropertyValueOrigin.Default) return EndpointIdentity.CreateSpnIdentity(element.ServicePrincipalName.Value);
        //    var dns = properties["dns"];
        //    if (dns != null && dns.ValueOrigin != PropertyValueOrigin.Default) return EndpointIdentity.CreateDnsIdentity(element.Dns.Value);
        //    var rsa = properties["rsa"];
        //    if (rsa != null && rsa.ValueOrigin != PropertyValueOrigin.Default) return EndpointIdentity.CreateRsaIdentity(element.Rsa.Value);
        //    var certificate = properties["certificate"];
        //    if (Equals(certificate, null) || PropertyValueOrigin.Default.Equals(certificate.ValueOrigin)) return null;
        //    var supportingCertificates = new X509Certificate2Collection();
        //    supportingCertificates.Import(Convert.FromBase64String(element.Certificate.EncodedValue));
        //    if (supportingCertificates.Count.Equals(0)) throw new InvalidOperationException("UnableToLoadCertificateIdentity");
        //    var primaryCertificate = supportingCertificates[0];
        //    supportingCertificates.RemoveAt(0);
        //    return EndpointIdentity.CreateX509CertificateIdentity(primaryCertificate, supportingCertificates);
        //}
        #endregion

        private static string GetEndpointAddress(string configurationName)
        {
            var result = ConfigurationHelper.GetEndpointAddressesByName(configurationName);
            if (Equals(result, null)) return string.Empty;
            if (result.Index >= 0 && result.Index < result.Endpoints.Count)
            {
                return result.Endpoints[result.Index];
            }
            var uid = ExecutionContext<BusinessRequestContext>.Current.CustomerNo;
            if (string.IsNullOrEmpty(uid)) uid = Guid.NewGuid().ToString("N");
            return CoreUtil.GetConsistentHash(result.Endpoints, uid);
        }

        public static string GetConfigurationName<T>()
        {
            var contract = (ServiceContractAttribute)typeof(T).GetCustomAttributes(false)[0];
            return contract != null && !string.IsNullOrEmpty(contract.ConfigurationName) ? contract.ConfigurationName : typeof(T).FullName;
        }

        public static CoreServiceModel GetServiceModel<T>()
        {
            var configurationName = GetConfigurationName<T>();
            var model = ConfigurationHelper.GetEndpointAddressesByName(configurationName);
            if (Equals(model, null)) return GetServiceModel(configurationName);
            var core = new CoreServiceModel
            {
                ConfigurationName = model.ConfigurationName,
                MaxPoolSize = model.MaxPoolSize,
                EndpointAddress = GetEndpointAddress(configurationName)
            };
            return core;
        }

        private static CoreServiceModel GetServiceModel(string configurationName)
        {
            var group = ConfigInit();
            return @group.Clients.Endpoints.Cast<ChannelEndpointElement>().Where(endpoint => Equals(endpoint.Contract, configurationName)).Select(endpoint => new CoreServiceModel
            {
                EndpointAddress = endpoint.Address.ToString(),
                MaxPoolSize = ConfigurationHelper.MaxPoolSize,
                ConfigurationName = configurationName
            }).FirstOrDefault();
        }
    }
}

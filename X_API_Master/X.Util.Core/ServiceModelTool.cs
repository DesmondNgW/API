using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Configuration;
using System.ServiceModel.Description;
using System.Web.Caching;
using X.Util.Entities;

namespace X.Util.Core
{
    public class ServiceModelTool
    {
        private static readonly string AppConfigFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data\\app.xml");
        private const string EndpointPrefix = "X.Util.Core.EndpointPrefix";
        private const string WcfNamePrefix = "X.Data.Core.";

        public static ServiceModelSectionGroup ConfigInit()
        {
            var key = EndpointPrefix + AppConfigFile;
            var cache = LocalCache.Get<ServiceModelSectionGroup>(key);
            if (cache != null) return cache;
            var map = new ExeConfigurationFileMap {ExeConfigFilename = AppConfigFile};
            var config = ConfigurationManager.OpenMappedExeConfiguration(map, ConfigurationUserLevel.None);
            cache = ServiceModelSectionGroup.GetSectionGroup(config);
            LocalCache.SlidingExpirationSet(key, cache, new CacheDependency(AppConfigFile, DateTime.Now), new TimeSpan(1, 0, 0), CacheItemPriority.Normal);
            return cache;
        }

        public static Binding CreateBinding(string bindingName, ServiceModelSectionGroup group)
        {
            var bindingElementCollection = group.Bindings[bindingName];
            if (bindingElementCollection.ConfiguredBindings.Count > 0)
            {
                return GetBinding(bindingElementCollection.ConfiguredBindings[0]);
            }
            return null;
        }

        private static Binding GetBinding<T>(IBindingConfigurationElement configurationElement) where T : Binding
        {
            var bind = Activator.CreateInstance<T>();
            configurationElement.ApplyConfiguration(bind);
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

        private static Binding GetBinding(IBindingConfigurationElement configurationElement)
        {
            Binding binding;
            return (configurationElement is CustomBindingElement && (binding = GetBinding<CustomBinding>(configurationElement)) != null)
                || (configurationElement is BasicHttpBindingElement && (binding = GetBinding<BasicHttpBinding>(configurationElement)) != null)
                || (configurationElement is NetMsmqBindingElement && (binding = GetBinding<NetMsmqBinding>(configurationElement)) != null)
                || (configurationElement is NetNamedPipeBindingElement && (binding = GetBinding<NetNamedPipeBinding>(configurationElement)) != null)
                || (configurationElement is NetPeerTcpBindingElement && (binding = GetBinding<NetPeerTcpBinding>(configurationElement)) != null)
                || (configurationElement is NetTcpBindingElement && (binding = GetBinding<NetTcpBinding>(configurationElement)) != null)
                || (configurationElement is WSDualHttpBindingElement && (binding = GetBinding<WSDualHttpBinding>(configurationElement)) != null)
                || (configurationElement is WSHttpBindingElement && (binding = GetBinding<WSHttpBinding>(configurationElement)) != null)
                || (configurationElement is WSFederationHttpBindingElement && (binding = GetBinding<WSFederationHttpBinding>(configurationElement)) != null)
                ? binding : null;
        }

        public static void AddBehaviors(string behaviorConfiguration, ServiceEndpoint serviceEndpoint, ServiceModelSectionGroup group)
        {
            var behaviorElement = group.Behaviors.EndpointBehaviors[behaviorConfiguration];
            foreach (var extension in behaviorElement.Select(behaviorExtension => behaviorExtension.GetType().InvokeMember("CreateBehavior", BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Instance, null, behaviorExtension, null)).Where(extension => extension != null))
            {
                serviceEndpoint.Behaviors.Add((IEndpointBehavior)extension);
            }
        }

        public static EndpointIdentity GetIdentity(IdentityElement element)
        {
            var properties = element.ElementInformation.Properties;
            var userPrincipalName = properties["userPrincipalName"];
            if (userPrincipalName != null && userPrincipalName.ValueOrigin != PropertyValueOrigin.Default) return EndpointIdentity.CreateUpnIdentity(element.UserPrincipalName.Value);
            var servicePrincipalName = properties["servicePrincipalName"];
            if (servicePrincipalName != null && servicePrincipalName.ValueOrigin != PropertyValueOrigin.Default) return EndpointIdentity.CreateSpnIdentity(element.ServicePrincipalName.Value);
            var dns = properties["dns"];
            if (dns != null && dns.ValueOrigin != PropertyValueOrigin.Default) return EndpointIdentity.CreateDnsIdentity(element.Dns.Value);
            var rsa = properties["rsa"];
            if (rsa != null && rsa.ValueOrigin != PropertyValueOrigin.Default) return EndpointIdentity.CreateRsaIdentity(element.Rsa.Value);
            var certificate = properties["certificate"];
            if (Equals(certificate, null) || PropertyValueOrigin.Default.Equals(certificate.ValueOrigin)) return null;
            var supportingCertificates = new X509Certificate2Collection();
            supportingCertificates.Import(Convert.FromBase64String(element.Certificate.EncodedValue));
            if (supportingCertificates.Count.Equals(0)) throw new InvalidOperationException("UnableToLoadCertificateIdentity");
            var primaryCertificate = supportingCertificates[0];
            supportingCertificates.RemoveAt(0);
            return EndpointIdentity.CreateX509CertificateIdentity(primaryCertificate, supportingCertificates);
        }

        private static string GetConfigurationName(string fullName)
        {
            return fullName.StartsWith(WcfNamePrefix) ? fullName.Substring(fullName.IndexOf('.', WcfNamePrefix.Length) + 1) : fullName;
        }

        private static string GetEndpointAddress(string configurationName)
        {
            var result = ConfigurationHelper.GetEndpointAddressesByName(configurationName);
            if (Equals(result, null)) return string.Empty;
            int count = result.Endpoints.Count, index = result.Index >= 0 && result.Index < count ? result.Index : StringConvert.SysRandom.Next(0, count);
            return result.Endpoints[index];
        }

        public static CoreServiceModel GetServiceModel<T>()
        {
            var configurationName = GetConfigurationName(typeof(T).FullName);
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
            return @group.Client.Endpoints.Cast<ChannelEndpointElement>().Where(endpoint => Equals(endpoint.Contract, configurationName)).Select(endpoint => new CoreServiceModel
            {
                EndpointAddress = endpoint.Address.ToString(), MaxPoolSize = ConfigurationHelper.MaxPoolSize, ConfigurationName = configurationName
            }).FirstOrDefault();
        }
    }
}

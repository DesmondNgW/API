using System;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Configuration;
using System.ServiceModel.Description;
using X.Util.Entities;

namespace X.Util.Core
{
    public interface IProvider<out T>
    {
        /// <summary>
        /// 实例
        /// </summary>
        T Client { get; }

        /// <summary>
        /// 关闭实例
        /// </summary>
        /// <param name="method"></param>
        /// <param name="eDomain"></param>
        void Close(MethodBase method, LogDomain eDomain);

        /// <summary>
        /// 远程uri地址
        /// </summary>
        string EndpointAddress { get; }
    }

    public interface IWcfProvider<out T> : IProvider<T>
    {
        /// <summary>
        /// 回收实例
        /// </summary>
        void Dispose(LogDomain eDomain);
    }

    public class CoreChannel<T> : ChannelFactory<T>
    {
        public CoreServiceModel ServiceModel;
        public CoreChannel(CoreServiceModel serviceModel)
            : base(typeof(T))
        {
            ServiceModel = serviceModel;
            InitializeEndpoint((string)null, null);
        }

        public CoreChannel(ServiceEndpoint serviceEndpoint, CoreServiceModel serviceModel)
            : base(serviceEndpoint)
        {
            ServiceModel = serviceModel;
            InitializeEndpoint(serviceEndpoint);
        }

        public CoreChannel(Binding binding, EndpointAddress endpointAddress, CoreServiceModel serviceModel)
            : base(binding, endpointAddress)
        {
            ServiceModel = serviceModel;
            InitializeEndpoint(binding, endpointAddress);
        }

        public CoreChannel(string configurationName, EndpointAddress endpointAddress, CoreServiceModel serviceModel)
            : base(configurationName, endpointAddress)
        {
            ServiceModel = serviceModel;
            InitializeEndpoint(configurationName, endpointAddress);
        }

        protected override ServiceEndpoint CreateDescription()
        {
            var serviceEndpoint = base.CreateDescription();
            var group = ServiceModelTool.ConfigInit();
            foreach (ChannelEndpointElement endpoint in group.Client.Endpoints)
            {
                if (endpoint.Contract != serviceEndpoint.Contract.ConfigurationName) continue;
                var endpointAddress = endpoint.Address;
                if (ServiceModel != null && !string.IsNullOrEmpty(ServiceModel.EndpointAddress)) endpointAddress = new Uri(ServiceModel.EndpointAddress);
                if (Equals(serviceEndpoint.Binding, null)) serviceEndpoint.Binding = ServiceModelTool.CreateBinding(endpoint.Binding, @group);
                if (Equals(serviceEndpoint.Address, null)) serviceEndpoint.Address = new EndpointAddress(endpointAddress, ServiceModelTool.GetIdentity(endpoint.Identity), endpoint.Headers.Headers);
                if (serviceEndpoint.Behaviors.Count.Equals(0) && !string.IsNullOrEmpty(endpoint.BehaviorConfiguration)) ServiceModelTool.AddBehaviors(endpoint.BehaviorConfiguration, serviceEndpoint, @group);
                serviceEndpoint.Name = endpoint.Contract;
                break;
            }
            return serviceEndpoint;
        }
    }
}

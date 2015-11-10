using System.ServiceModel;

namespace X.Data.Core.CoreReader.Services
{
    [ServiceContract(ConfigurationName = "Services.IAngieOneService")]
    public interface IAngieOneService : Angie.Interface.ICoreServices { }

    [ServiceContract(ConfigurationName = "Services.IBankManager")]
    public interface IBankManager : Em.FundTrade.Business.Contract.IBankManager { }

    [ServiceContract(ConfigurationName = "Services.IBusinAppManager")]
    public interface IBusinAppManager : Em.FundTrade.Business.Contract.IBusinAppManager { }

    [ServiceContract(ConfigurationName = "Services.IBusinCfmManager")]
    public interface IBusinCfmManager : Em.FundTrade.Business.Contract.IBusinCfmManager { }

    [ServiceContract(ConfigurationName = "Services.ICashBagPaymentManager")]
    public interface ICashBagPaymentManager : Em.FundTrade.Business.Contract.ICashBagPaymentManager { }

    [ServiceContract(ConfigurationName = "Services.ICashBagSearchManager")]
    public interface ICashBagSearchManager : Em.FundTrade.Business.Contract.ICashBagSearchManager { }

    [ServiceContract(ConfigurationName = "Services.ICashBagTradeManager")]
    public interface ICashBagTradeManager : Em.FundTrade.Business.Contract.ICashBagTradeManager { }

    [ServiceContract(ConfigurationName = "Services.IComplaintManager")]
    public interface IComplaintManager : Em.FundTrade.Business.Contract.IComplaintManager { }

    [ServiceContract(ConfigurationName = "Services.ICustomerManager")]
    public interface ICustomerManager : Em.FundTrade.Business.Contract.ICustomerManager { }

    [ServiceContract(ConfigurationName = "Services.ICustSecurityManager")]
    public interface ICustSecurityManager : Em.FundTrade.Business.Contract.ICustSecurityManager { }

    [ServiceContract(ConfigurationName = "Services.IFundManager")]
    public interface IFundManager : Em.FundTrade.Business.Contract.IFundManager { }

    [ServiceContract(ConfigurationName = "Services.IFundTransferBusiness")]
    public interface IFundTransferBusiness : Em.FundTrade.Business.Contract.IFundTransferBusiness { }

    [ServiceContract(ConfigurationName = "Services.IPayChannelManager")]
    public interface IPayChannelManager : Em.FundTrade.Business.Contract.IPayChannelManager { }

    [ServiceContract(ConfigurationName = "Services.ISalesApplicabilityManager")]
    public interface ISalesApplicabilityManager : Em.FundTrade.Business.Contract.ISalesApplicabilityManager { }

    [ServiceContract(ConfigurationName = "Services.IShareManager")]
    public interface IShareManager : Em.FundTrade.Business.Contract.IShareManager { }

    [ServiceContract(ConfigurationName = "Services.IShareTransferManager")]
    public interface IShareTransferManager : Em.FundTrade.Business.Contract.IShareTransferManager { }

    [ServiceContract(ConfigurationName = "Services.IRouter")]
    public interface IRouter : Em.FundTrade.Route.Services.Contract.IRouter { }

    [ServiceContract(ConfigurationName = "Services.IRouterManage")]
    public interface IRouterManage : Em.FundTrade.Route.Services.Contract.IRouterManage { }
}

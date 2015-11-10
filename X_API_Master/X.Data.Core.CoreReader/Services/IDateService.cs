using System;
using System.ServiceModel;

namespace X.Data.Core.CoreReader.Services
{
    [ServiceContract(ConfigurationName = "Services.IDateService")]
    public interface IDateService
    {
        [OperationContract]
        DateTime FetchDateTime();
        [OperationContract]
        DateTime FetchCurrWorkDay(DateTime date);
        [OperationContract]
        DateTime FetchLastWorkday(DateTime date);
        [OperationContract]
        DateTime FetchLast2Workday(DateTime date);
        [OperationContract]
        DateTime FetchLast3Workday(DateTime date);
        [OperationContract]
        DateTime FetchNextWorkday(DateTime date);
        [OperationContract]
        DateTime FetchNext2Workday(DateTime date);
        [OperationContract]
        DateTime FetchNext3Workday(DateTime date);
        [OperationContract]
        DateTime FetchPointWorkday(DateTime date, int day);
        [OperationContract]
        DateTime FetchPreCfmDate(DateTime appTime);
    }
}

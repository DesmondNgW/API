using System;
using System.Threading;
using X.Interface.Core;
using X.Interface.Dto;
using X.Interface.Dto.HttpResponse;
using X.Interface.Dto.Interface;
using X.UI.Util.Model;
using X.Util.Core;
using X.Util.Core.Kernel;
using X.Util.Entities;
using X.Util.Provider;

namespace X.UI.Util.Helper
{
    public class InjectRequestContext
    {
        public static HttpRequestContext4Heads InjectHeaders(string clientId)
        {
            var result = new HttpRequestContext4Heads();

            var provider = new InstanceProvider<RequestManagerService>(LogDomain.Ui);
            var iresult = CoreAccess<IRequestManager>.TryCall(provider.Client.GetRequestPre,
                clientId, IpBase.GetLocalIp(), provider, new LogOptions<ApiResult<RequestPreDto>>(CoreService.CallSuccess));
            if (CoreService.CallSuccess(iresult))
            {
                result.Token = iresult.Data.Token;
                result.Version = iresult.Data.Version;
                result.Timestamp = iresult.Data.Timestamp;
            }
            return result;
        }

        public static ApiRequestContext InjectApiRequestContext(string clientId)
        {
            var ApiRequestContext = new ApiRequestContext()
            {
                RequestId = Guid.NewGuid().ToString("N"),
                UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:100.0) Gecko/20100101 Firefox/100.0",
                ServerIp = IpBase.GetLocalIp(),
                Interface = string.Empty,
                ServerTime = DateTime.Now,
                Cid = Thread.CurrentThread.ManagedThreadId.ToString(),
                ActionArgument = string.Empty
            };
            var headersContext = InjectHeaders(clientId);
            ApiRequestContext.Heads = headersContext;
            return ApiRequestContext;
        }

        public static BusinessRequestContext Inject(string clientId, bool verifyClient, bool isLogin)
        {
            var apiContext = InjectApiRequestContext(clientId);
            var businessRequestContext = RequestContextHelper.GetBusinessRequestContext(apiContext, verifyClient, isLogin);
            businessRequestContext.Update(string.Empty, string.Empty);
            return businessRequestContext;
        }
    }
}

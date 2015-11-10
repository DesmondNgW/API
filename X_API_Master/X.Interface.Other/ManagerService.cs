using System;
using System.IO;
using System.Linq;
using X.Interface.Dto;
using X.Interface.Dto.HttpRequest;
using X.Util.Core;

namespace X.Interface.Other
{
    public class ManagerService
    {
        public void DeleteHistoryLog(DeleteLogFilesRequestDto dto, string path)
        {
            var remain = Math.Min(3, dto.Remain);
            var dir = new DirectoryInfo(path);
            if (!dir.Exists) return;
            foreach (var item in dir.GetDirectories().Where(item => (DateTime.Now - DateTime.Parse(item.FullName)).Days > remain * 30))
            {
                item.Delete(true);
            }
        }

        public void ClearCustomerCache(UserRequestDtoBase context)
        {
            ConfigurationHelper.UpdateAppSettingByName("CustomerCacheApp", new Version(AppConfig.CustomerCacheApp).Add(0, 0, 1, 0).ToString(3));
        }

        public void ClearRouterCache(UserRequestDtoBase context)
        {
            ConfigurationHelper.UpdateAppSettingByName("RouterCacheApp", new Version(AppConfig.RouterCacheApp).Add(0, 0, 1, 0).ToString(3));
        }

        public void ClearWhiteListCache(UserRequestDtoBase context)
        {
            ConfigurationHelper.UpdateAppSettingByName("WhiteListCacheApp", new Version(AppConfig.WhiteListCacheApp).Add(0, 0, 1, 0).ToString(3));
        }

        public void ClearFundCache(UserRequestDtoBase context)
        {
            ConfigurationHelper.UpdateAppSettingByName("FundCacheApp", new Version(AppConfig.FundCacheApp).Add(0, 0, 1, 0).ToString(3));
        }
    }
}

using System;
using X.Interface.Dto.Interface;
using X.Util.Core.Configuration;
using X.Util.Entities.Enums;
using X.Util.Provider;

namespace X.UI.Helper
{
    public class StockDealHelper
    {
        public static void Main()
        {
            var provider = new InstanceProvider<IStockManager>(LogDomain.Ui);
            //开盘时间
            var tradeStart = ConfigurationHelper.GetAppSettingByName("TradeStart", new DateTime(2099, 1, 1, 9, 15, 0));
            //收盘时间
            var tradeEnd = ConfigurationHelper.GetAppSettingByName("TradeEnd", new DateTime(2099, 1, 1, 15, 0, 0));
            //运行模式
            var mode = ConfigurationHelper.GetAppSettingByName("mode", 0);
            var topCount = ConfigurationHelper.GetAppSettingByName("topCount", 15);
            var dt = DateTime.Now;
            if (mode == 2 || (mode == 0 && (dt.DayOfWeek == DayOfWeek.Saturday || dt.DayOfWeek == DayOfWeek.Sunday || dt.TimeOfDay > tradeEnd.TimeOfDay || dt.AddMinutes(30).TimeOfDay <= tradeStart.TimeOfDay)))
            {
                provider.Client.Replay();
            }
            else if (mode == 3)
            {
                provider.Client.SelectStock(topCount);
            }
            else if (mode == 5)
            {
                provider.Client.SaveDataBase();
            }
            Console.WriteLine("Program End! Press Any Key!");
            Console.ReadKey();
        }
    }
}

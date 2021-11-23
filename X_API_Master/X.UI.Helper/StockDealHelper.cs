using System;
using System.Linq;
using System.Threading;
using X.UI.Entities;
using X.Util.Core.Configuration;

namespace X.UI.Helper
{
    public class StockDealHelper
    {
        public static void Main()
        {
            //开盘时间
            var tradeStart = ConfigurationHelper.GetAppSettingByName("TradeStart", new DateTime(2099, 1, 1, 9, 15, 0));
            //收盘时间
            var tradeEnd = ConfigurationHelper.GetAppSettingByName("TradeEnd", new DateTime(2099, 1, 1, 15, 0, 0));
            //运行模式
            var mode = ConfigurationHelper.GetAppSettingByName("mode", 0);
            //权重
            var weight = ConfigurationHelper.GetAppSettingByName("weight", "auto");
            //趋势因子weekDDX
            var weekddx = ConfigurationHelper.GetAppSettingByName("weekddx", "auto");

            Console.BackgroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.Black;
            var dt = DateTime.Now;
            //盯盘
            if (mode == 1 || (mode == 0 && dt.DayOfWeek != DayOfWeek.Saturday && dt.DayOfWeek != DayOfWeek.Sunday && dt.TimeOfDay <= tradeEnd.TimeOfDay))
            {
                var AQS = StockDealIO.GetMyStock(MyStockMode.AQS);
                var Wave = StockDealIO.GetMyStock(MyStockMode.Wave);
                var ddx = StockDealIO.GetDDXList2();
                var des = StockDealIO.GetStockDes(StockDesType.DateBase);
                var jx = StockDealIO.GetMyStock(MyStockMode.JX);
                var jx2 = StockDealIO.GetMyStock(MyStockMode.JX2);
                var kernel = StockDealIO.GetMyStock(MyStockMode.Kernel);
                var core = StockDealIO.GetMyMonitorStock(MyStockType.CoreT);
                var spK = StockDealIO.GetModeCompareWithOrder(StockDealBusiness.GetModeCompareAuto(ddx, kernel, core, weight), "Kernel模式-开始", weekddx);
                var sp = spK.Select(p => p.Key).ToList();
                var all = StockDealBase.Union(AQS, Wave);
                while (dt.TimeOfDay >= tradeStart.TimeOfDay && dt.TimeOfDay <= tradeEnd.TimeOfDay)
                {
                    StockDealIO.MonitorIndex();
                    StockDealIO.MonitorStock(AQS, all, jx, jx2, sp, des);
                    StockDealIO.GetStockResult(jx, jx2, core);
                    Thread.Sleep(6000);
                    dt = DateTime.Now;
                }
            }
            //复盘
            if (mode == 2 || (mode == 0 && (dt.DayOfWeek == DayOfWeek.Saturday || dt.DayOfWeek == DayOfWeek.Sunday ||
                dt.TimeOfDay > tradeEnd.TimeOfDay || dt.AddMinutes(30).TimeOfDay <= tradeStart.TimeOfDay)))
            {
                var AQS = StockDealIO.GetMyStock(MyStockMode.AQS);
                var Wave = StockDealIO.GetMyStock(MyStockMode.Wave);
                StockDealIO.Deal(AQS, Wave);
            }
            else if (mode == 3)
            {
                //首板
                var first = StockDealIO.GetMyMonitorStock(MyStockType.First);
                //涨停
                var zt = StockDealIO.GetMyMonitorStock(MyStockType.ZT);
                var kernel = StockDealIO.GetMyMonitorStock(MyStockType.Kernel);
                var kernelH = StockDealIO.GetMyMonitorStock(MyStockType.KernelH);
                var kernelL = StockDealIO.GetMyMonitorStock(MyStockType.KernelL);
                var coreT = StockDealIO.GetMyMonitorStock(MyStockType.CoreT);
                var coreT2 = StockDealIO.GetMyMonitorStock(MyStockType.CoreT2);
                var coreT3 = StockDealIO.GetMyMonitorStock(MyStockType.CoreT3);
                var ddx = StockDealIO.GetDDXList();
                var AQS = StockDealIO.GetMyStock(MyStockMode.AQS);
                var Wave = StockDealIO.GetMyStock(MyStockMode.Wave);
                var all = StockDealBase.Union(AQS, Wave);
                StockDealIO.FilterStock(first, zt, kernel, kernelH, kernelL, coreT, coreT2, coreT3, ddx, AQS, all);
            }
            else if (mode == 4)
            {
                var jx = StockDealIO.GetMyStock(MyStockMode.JX);
                var jx2 = StockDealIO.GetMyStock(MyStockMode.JX2);
                var kernel = StockDealIO.GetMyStock(MyStockMode.Kernel);
                var core = StockDealIO.GetMyMonitorStock(MyStockType.CoreT);
                var des = StockDealIO.GetStockDes(StockDesType.DateBase);
                var a2 = StockDealIO.GetFilterListFromFile();
                var ddxList = StockDealIO.GetDDXList2();
                var iret1 = StockDealIO.GetModeCompareWithOrder(StockDealBusiness.GetModeCompareAuto(ddxList, jx, core, weight), "精选模式-开始", weekddx);
                StockDealIO.GetResultFromMode(iret1, des);

                var iret2 = StockDealIO.GetModeCompareWithOrder(StockDealBusiness.GetModeCompareAuto(ddxList, jx2, core, weight), "精选2模式-开始", weekddx);
                StockDealIO.GetResultFromMode(iret2, des);

                var iret3 = StockDealIO.GetModeCompareWithOrder(StockDealBusiness.GetModeCompareAuto(ddxList, kernel, core, weight), "Kernel模式-开始", weekddx);
                StockDealIO.GetResultFromMode(iret3, des);

                StockDealIO.GetModeCompareWithOrder(StockDealBusiness.GetModeCompare(ddxList, a2, kernel, weight), "板块-开始", weekddx);
                StockDealIO.GetStockResult(jx, jx2, core);
            }
            else if (mode == 5)
            {
                StockDealIO.SaveDataBase();
            }
            else if (mode == 6)
            {
                var des = StockDealIO.GetStockDes(StockDesType.DateBase);
                var ddxList = StockDealIO.GetDDXList2();
                var kernel = StockDealIO.GetMyStock(MyStockMode.Kernel);
                var core = StockDealIO.GetMyMonitorStock(MyStockType.CoreT);
                var iret1 = StockDealIO.GetModeCompareWithOrder(StockDealBusiness.GetModeCompareAuto(ddxList, kernel, core, weight), "最强资金流", weekddx);
                StockDealIO.GetResultFromMode(iret1, des);
                for (var i = 1; i < 4; i++)
                {
                    iret1 = StockDealIO.GetModeCompareWithOrder(iret1, "最强资金流-" + i, weekddx);
                    StockDealIO.GetResultFromMode(iret1, des);
                }
            }

            Console.WriteLine("Program End! Press Any Key!");
            Console.ReadKey();
        }
    }
}

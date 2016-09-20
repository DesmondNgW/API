using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using X.Stock.Service.Model;
using X.Stock.Service.Utils;

namespace X.Stock.Service
{
    public class Work
    {
        public const string CustomerNo = "test1234567890";

        public const string CustomerName = "testMyProgram";

        public const double CoinAsset = 100000;

        public const int T1 = 10*1000*60;

        public const int T2 = 3*1000;

        protected static Thread[] Pool { get; set; }
        public static void CreateThreads()
        {
            Pool = new Thread[3];
            for (var i = 0; i < 3; i++)
            {
                Pool[i] = new Thread(LoopCall) { IsBackground = true };
                Pool[i].Start(i);
            }
        }

        public static void AbortThreads()
        {
            if (Pool == null || Pool.Length <= 0) return;
            foreach (var t in Pool)
            {
                t.Abort();
            }
        }

        public static void LoopCall(object obj)
        {
            var i = Convert.ToInt32(obj);
            while (true)
            {
                var cantrade = StockTradeService.IsCanTrade();
                switch (i)
                {
                    case 0:
                        if (!cantrade)
                        {
                            StockPoolService.ImportStockPool("gb2312");
                        }
                        Thread.Sleep(T1);
                        break;
                    case 1:
                        if (!cantrade)
                        {
                            CustomerService.UpdateStockShares(CustomerNo);
                        }
                        Thread.Sleep(T1);
                        break;
                    case 2:
                        if (cantrade)
                        {
                            var info = CustomerService.GetAssetInfo(CustomerNo);
                            int count = 0, sellcount = 0;
                            if (info.Shares != null)
                            {
                                count = info.Shares.Count(p => p.TotalVol > 0);
                                sellcount = info.Shares.Count(p => p.TotalVol > 0 && p.CreateTime.Date != DateTime.Now.Date);
                            }
                            var stocks = default(List<StockInfo>);
                            if (count < 4)
                            {
                                stocks = StockPoolService.GetStockInfoFromPool();
                                if (stocks != null)
                                {
                                    StockTradeService.BuyStock(stocks, info);
                                }
                            }
                            if (stocks != null)
                            {
                                MonitorService.MonitorBuy(stocks);
                            }
                            if (sellcount > 0)
                            {
                                StockTradeService.SellStock(info);
                            }
                        }
                        Thread.Sleep(T2);
                        break;
                }
            }
        }
    }
}

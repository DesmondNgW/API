using System;
using System.Threading;
using X.Stock.Monitor.Utils;

namespace X.Stock.Monitor
{
    public class Work
    {
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
                        Thread.Sleep(10 * 1000 * 60);
                        break;
                    case 1:
                        if (!cantrade)
                        {
                            CustomerService.UpdateStockShares(CustomerService.CustomerNo);
                        }
                        Thread.Sleep(10 * 1000 * 60);
                        break;
                    case 2:
                        if (cantrade)
                        {
                            var info = CustomerService.GetAssetInfo(CustomerService.CustomerNo);
                            var count = info.Shares != null ? info.Shares.Count : 0;
                            if (count < 4)
                            {
                                var stocks = StockPoolService.GetStockInfoFromPool();
                                StockTradeService.BuyStock(stocks, info);
                            }
                            if (count > 0)
                            {
                                StockTradeService.SellStock(info);
                            }
                        }
                        Thread.Sleep(3 * 1000);
                        break;
                }
            }
        }
    }
}

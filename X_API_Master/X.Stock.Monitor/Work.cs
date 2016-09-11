using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
                            var stocks = StockPoolService.GetStockInfoFromPool();
                            var info = CustomerService.GetAssetInfo(CustomerService.CustomerNo);
                            StockTradeService.BuyStock(stocks, info);
                            StockTradeService.SellStock(stocks, info);
                        }
                        Thread.Sleep(3 * 1000);
                        break;
                }
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using X.Stock.DB;
using X.Stock.Model;

namespace X.Stock.Service
{
    public class MonitorService
    {
        private static string GetId(string stockCode, MonitorState state)
        {
            return stockCode + "_" + DateTime.Now.ToString("yyyyMMdd") + "_" + state;
        }

        public static void MonitorBuy(List<StockInfo> stocks)
        {
            const MonitorState state = MonitorState.Buy;
            var targets = stocks.Where(p => p.StockKm2 >= 3.3 && p.StockKm2 <= 8.8);
            var list = MonitorTable.GetStockMonitor();
            foreach (var target in targets.Where(target => list.Count(p => p.Id == GetId(target.StockCode, state)) == 0))
            {
                var msg = string.Format("Stock Code {0}({1}) has at price {2},inc {3}%", target.StockCode, target.StockName, target.StockPrice, target.StockKm2);
                SmtpMailHelper.Send("Stock." + target.StockCode, msg);
                MonitorTable.MonitorTrade(target, GetId(target.StockCode, state), state);
            }
        }

        public static void MonitorSell(List<StockInfo> stocks)
        {
            const MonitorState state = MonitorState.Sell;
            var targets = stocks.Where(p => p.StockKm2 <=-3.3);
            var list = MonitorTable.GetStockMonitor();
            foreach (var target in targets.Where(target => list.Count(p => p.Id == GetId(target.StockCode, state)) == 0))
            {
                var msg = string.Format("Stock Code {0}({1}) has at price {2},inc {3}%", target.StockCode, target.StockName, target.StockPrice, target.StockKm2);
                SmtpMailHelper.Send("Stock." + target.StockCode, msg);
                MonitorTable.MonitorTrade(target, GetId(target.StockCode, state), state);
            }
        }
    }
}

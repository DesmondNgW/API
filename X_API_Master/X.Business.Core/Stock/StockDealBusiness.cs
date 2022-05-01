using System;
using System.Collections.Generic;
using System.Linq;
using X.Business.Entities.Stock;
using X.Business.Helper.Stock;

namespace X.Business.Core.Stock
{
    public class StockDealBusiness
    {
        #region 复盘逻辑
        /// <summary>
        /// 前排股票
        /// </summary>
        /// <param name="list"></param>
        /// <param name="top"></param>
        /// <param name="f"></param>
        /// <returns></returns>
        public static string[] GetStockName(IEnumerable<MyStock> list, int top, Func<MyStock, bool> f, Func<MyStock, string> o)
        {
            var a1 = list.Where(f).OrderByDescending(p => p.S1).Take(top);
            var a2 = list.Where(f).OrderByDescending(p => p.S2).Take(top);
            var a3 = list.Where(f).OrderByDescending(p => p.S3).Take(top);
            var a4 = list.Where(f).OrderByDescending(p => p.S4).Take(top);
            var b1 = a1.Union(a2);
            var b2 = a3.Union(a4);
            var b = b1.Where(p => b2.Count(q => q.Code == p.Code) > 0).Distinct();
            var ret = new List<string>() { StockDealBase.Calc(b).ToString("0.00") };
            return ret.Union(b.OrderByDescending(p => p.K1 + p.K2 + p.K3 + p.K4).Select(o)).ToArray();
        }
        #endregion
    }
}

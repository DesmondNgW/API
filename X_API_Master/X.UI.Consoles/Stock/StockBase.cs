using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace X.UI.Consoles.Stock
{
    public class StockBase
    {
        /// <summary>
        /// 代码
        /// </summary>
        public string StockCode { get; set; }

        /// <summary>
        /// 名名称
        /// </summary>
        public string StockName { get; set; }

        /// <summary>
        /// 行业
        /// </summary>
        public string Industry { get; set; }

        /// <summary>
        /// 总股本
        /// </summary>
        public decimal GeneralCapital { get; set; }

        /// <summary>
        /// 流通股本
        /// </summary>
        public decimal NegotiableCapital { get; set; }
    }

    public class StockPrice
    {
        /// <summary>
        /// 代码
        /// </summary>
        public string StockCode { get; set; }

        /// <summary>
        /// 最高价
        /// </summary>
        public decimal MaxPrice { get; set; }

        /// <summary>
        /// 最小价
        /// </summary>
        public decimal MinPrice { get; set; }

        /// <summary>
        /// 开盘价
        /// </summary>
        public decimal OpenPrice { get; set; }

        /// <summary>
        /// 最新价
        /// </summary>
        public decimal CurrentPrice { get; set; }

        /// <summary>
        /// 昨日收盘
        /// </summary>
        public decimal LastClosePrice { get; set; }

        /// <summary>
        /// 涨跌幅
        /// </summary>
        public decimal Inc
        {
            get { return CurrentPrice/LastClosePrice*100 - 100; }
        }

        /// <summary>
        /// 最大涨幅
        /// </summary>
        public decimal MaxInc
        {
            get { return MaxPrice / LastClosePrice * 100 - 100; }
        }

        /// <summary>
        /// 最小涨幅
        /// </summary>
        public decimal MinInc
        {
            get { return MinPrice / LastClosePrice * 100 - 100; }
        }
    }
}

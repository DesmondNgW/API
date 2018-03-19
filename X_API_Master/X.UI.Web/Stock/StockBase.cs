
namespace X.UI.Web.Stock
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

        public string StockName { get; set; }
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
        public decimal Inc { get; set; }

        /// <summary>
        /// 最大涨幅
        /// </summary>
        public decimal MaxInc { get; set; }

        /// <summary>
        /// 最小涨幅
        /// </summary>
        public decimal MinInc { get; set; }
    }
}

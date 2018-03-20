
namespace X.UI.Entities
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
}

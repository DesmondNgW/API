

namespace X.UI.Entities
{
    public class StockKLineExtend : StockKLine
    {
        public decimal Inc { get; set; }

        /// <summary>
        /// 一字板分钟数
        /// </summary>
        public int CountOfOnePrice { get; set; }

        /// <summary>
        /// 一字板成交量
        /// </summary>
        public decimal VolOfOnePrice { get; set; }

        /// <summary>
        /// 一字板成交额
        /// </summary>
        public decimal AmountOfOnePrice { get; set; }

        /// <summary>
        /// 封板分钟数
        /// </summary>
        public int CountOfHighPrice { get; set; }

        /// <summary>
        /// 封板成交量
        /// </summary>
        public decimal VolOfHighPrice { get; set; }

        /// <summary>
        /// 封板成交额
        /// </summary>
        public decimal AmountOfHighPrice { get; set; }

        /// <summary>
        /// 封板金额占比
        /// </summary>
        public decimal AHP { get; set; }

        /// <summary>
        /// 封板量能占比
        /// </summary>
        public decimal VHP { get; set; }

        /// <summary>
        /// 一字板金额占比
        /// </summary>
        public decimal AOP { get; set; }

        /// <summary>
        /// 一字板量能占比
        /// </summary>
        public decimal VOP { get; set; }

        /// <summary>
        /// 首次封板时间
        /// </summary>
        public int FirstTime { get; set; }

        /// <summary>
        /// 最后封板时间
        /// </summary>
        public int LastTime { get; set; }
    }
}

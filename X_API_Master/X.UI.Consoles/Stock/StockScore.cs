namespace X.UI.Consoles.Stock
{
    /// <summary>
    /// 计算证券的得分,量化股票的强度,
    /// 追高策略（阳）：选新不选旧, 寻找有持续性的热点板块, 从中选取强度大的股票; 其中目前最有持续性的热点板块：半导体芯片、5G、次新；次之的有雄安新区和周期股
    /// 低吸策略（阴）：选旧不选新, 寻找有持续性的热点板块，前N周期出现高强度，当前周期弱强度; 其中目前最有持续性的热点板块：半导体芯片、5G、次新；次之的有雄安新区和周期股
    /// </summary>
    public class StockScore
    {
        public StockScore()
        {
            Item1 = 1;
            Item2 = 1;
            Item3 = 1;
            Item4 = 1;
            Item5 = 1;
            Item6 = 1;
            Item7 = 1;
            Item8 = 1;
            Item9 = 1;
            Item10 = 1;
        }
        public double Item1 { get; set; }

        public double Item2 { get; set; }

        public double Item3 { get; set; }

        public double Item4 { get; set; }

        public double Item5 { get; set; }

        public double Item6 { get; set; }

        public double Item7 { get; set; }

        public double Item8 { get; set; }

        public double Item9 { get; set; }

        public double Item10 { get; set; }

        public double Value
        {
            get { return Item1*Item2*Item3*Item4*Item5*Item6*Item7*Item8*Item9*Item10; }
        }

        public Stock Stock { get; set; }
    }
}

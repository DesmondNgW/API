namespace X.UI.Consoles.Stock
{
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

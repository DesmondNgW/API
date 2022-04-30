using System.Collections.Generic;
using X.Business.Entities.Enums;

namespace X.Business.Entities.Stock
{
    public class MyStockMonitor
    {
        public string StockCode { get; set; }
        public string StockName { get; set; }
        public decimal Inc { get; set; }
        public decimal Price { get; set; }
        public double S { get; set; }
        public int SLevel { get; }
        public decimal K { get; set; }
        public int KLevel { get; }
        public decimal L { get; set; }
        public int LLevel { get; }
        public MyStockType MyStockType { get; set; }
        public decimal Amount { get; set; }
        public double VolRate { get; set; }
        public double AmountRate { get; set; }
        public decimal Buy1 { get; set; }
        public decimal Sell1 { get; set; }
        public string Remark { get; set; }
        public string OrderRemark { get; set; }
        public string OrderRemark2 { get; set; }
        public string OrderRemark3 { get; set; }

        public string LDX { get; set; }

        public string NF { get; set; }
        public string KLL { get; set; }
        public bool IsHigh { get; set; }

        public List<string> BK { get; set; }
    }
}

using System;

namespace X.UI.Consoles.Stock
{
    public class StockL2
    {
        public long TranId { get; set; }
        public DateTime Time { get; set; }
        public decimal Price { get; set; }
        public decimal Volume { get; set; }
        public decimal SaleOrderVolume { get; set; }
        public decimal BuyOrderVolume { get; set; }
        public string Type { get; set; }
        public long SaleOrderId { get; set; }
        public decimal SaleOrderPrice { get; set; }
        public long BuyOrderId { get; set; }
        public decimal BuyOrderPrice { get; set; }
    }
}

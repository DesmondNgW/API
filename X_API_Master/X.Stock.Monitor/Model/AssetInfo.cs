using System.Collections.Generic;

namespace X.Stock.Monitor.Model
{
    public class AssetInfo
    {
        public string CustomerNo { get; set; }

        public string CustomerName { get; set; }

        public decimal CoinAsset { get; set; }

        public List<StockShare> Shares { get; set; } 
    }
}

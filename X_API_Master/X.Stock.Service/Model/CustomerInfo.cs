using X.Util.Entities;

namespace X.Stock.Service.Model
{
    public class CustomerInfo : MongoBaseModel
    {
        public string CustomerNo { get; set; }

        public string CustomerName { get; set; }

        public decimal CoinAsset { get; set; }
    }
}

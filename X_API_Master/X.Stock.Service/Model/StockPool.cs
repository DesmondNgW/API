using System;
using X.Util.Entities;

namespace X.Stock.Service.Model
{
    public class StockPool : MongoBaseModel
    {
        public string StockCode { get; set; }

        public string StockName { get; set; }

        public DateTime CreateTime { get; set; }
    }
}

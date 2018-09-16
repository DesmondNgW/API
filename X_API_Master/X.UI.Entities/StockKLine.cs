using System;
using System.Collections.Generic;
using X.Util.Entities;

namespace X.UI.Entities
{
    public class StockKLine : MongoBaseModel
    {
        public string StockCode { get; set; }

        public string StockName { get; set; }

        [MongoDB.Bson.Serialization.Attributes.BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime Date { get; set; }

        public decimal High { get; set; }

        public decimal Low { get; set; }

        public decimal Open { get; set; }

        public decimal Close { get; set; }

        public decimal LastClose { get; set; }

        public decimal Vol { get; set; }

        public decimal Amount { get; set; }

        public int Time { get; set; }

        public decimal Hsl { get; set; }
    }

    public class StockKLineCombine
    {
        public string Id { get; set; }

        public List<StockKLine> Minutes { get; set; }

        public StockKLine Day { get; set; }
    }
}

using System;
using System.Collections.Generic;
using X.Util.Entities;

namespace X.UI.Entities
{
    public class summary
    {
        public int size { get; set; }

        public DateTime time { get; set; }
    }

    public class JRJDataItem : MongoBaseModel
    {
        [MongoDB.Bson.Serialization.Attributes.BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime DateTime { get; set; }

        public string StockCode { get; set; }

        public string StockName { get; set; }

        public decimal Price { get; set; }

        public decimal PriceLimit { get; set; }

        public decimal FCB { get; set; }

        public decimal FLB { get; set; }

        public decimal FDMoney { get; set; }

        [MongoDB.Bson.Serialization.Attributes.BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime FirstZtTime { get; set; }
        [MongoDB.Bson.Serialization.Attributes.BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime LastZtTime { get; set; }

        public int OpenTime { get; set; }

        public decimal ZF { get; set; }

        public double Force { get; set; }

        public decimal Amount
        {
            get
            {
                return FCB != 0 ? FDMoney / FCB : 0;
            }
        }
    }

    public class JRJData
    {
        public summary summary { get; set; }

        public List<List<string>> Data { get; set; }
    }

    public class Continue
    {
        public DateTime Start { get; set; }

        public DateTime End { get; set; }

        public string StockCode { get; set; }

        public string StockName { get; set; }

        public int Count { get; set; }

        public bool Enable { get; set; }
    }
}

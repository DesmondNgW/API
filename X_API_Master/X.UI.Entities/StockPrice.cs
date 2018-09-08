using System;
using System.Collections.Generic;
using X.Util.Entities;

namespace X.UI.Entities
{
    public class StockPrice: MongoBaseModel
    {
        /// <summary>
        /// 代码
        /// </summary>
        public string StockCode { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string StockName { get; set; }
        /// <summary>
        /// 最高价
        /// </summary>
        public decimal MaxPrice { get; set; }

        /// <summary>
        /// 最小价
        /// </summary>
        public decimal MinPrice { get; set; }

        /// <summary>
        /// 开盘价
        /// </summary>
        public decimal OpenPrice { get; set; }

        /// <summary>
        /// 最新价
        /// </summary>
        public decimal CurrentPrice { get; set; }

        /// <summary>
        /// 昨日收盘
        /// </summary>
        public decimal LastClosePrice { get; set; }

        /// <summary>
        /// 涨跌幅
        /// </summary>
        public decimal Inc { get; set; }

        /// <summary>
        /// 最大涨幅
        /// </summary>
        public decimal MaxInc { get; set; }

        /// <summary>
        /// 最小涨幅
        /// </summary>
        public decimal MinInc { get; set; }

        /// <summary>
        /// 日期
        /// </summary>
        [MongoDB.Bson.Serialization.Attributes.BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime Datetime { get; set; }

        /// <summary>
        /// 指数
        /// </summary>
        public List<string> Indexs { get; set; } 
    }
}

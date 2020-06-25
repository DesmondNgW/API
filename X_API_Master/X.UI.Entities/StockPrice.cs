﻿using System;
using System.Collections.Generic;
using X.Util.Entities;

namespace X.UI.Entities
{
    public enum MyStockType
    {
        //龙头
        Top,
        //接力
        Continie,
        //趋势
        Trend,
        //中线强势股
        MiddleTop,
        //短线强势3D
        ShortTop3D,
        //短线强势D
        ShortTopD,
        //短线强势H，
        ShortTopH,
    }

    public class StockPrice : MongoBaseModel
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

        public decimal Amount { get; set; }

        public decimal Vol { get; set; }

        public decimal MY { get; set; }

        public MyStockType MyStockType { get; set; }
    }
}

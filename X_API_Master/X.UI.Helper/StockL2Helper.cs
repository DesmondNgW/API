using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using X.UI.Entities;
using X.Util.Core;

namespace X.UI.Helper
{
    public class StockL2Helper
    {
        public List<StockL2> Data; 
        public StockL2Helper(string path)
        {
            Data = new List<StockL2>();
            var sr = new StreamReader(path, Encoding.UTF8);
            string content;
            while ((content = sr.ReadLine()) != null)
            {
                if (!content.Contains("TranID"))
                {
                    var arr = content.Split(',');
                    Data.Add(new StockL2
                    {
                        TranId = arr[0].Convert2Int64(0),
                        Time = arr[1].Convert2DateTime(DateTime.MinValue),
                        Price = arr[2].Convert2Decimal(0),
                        Volume = arr[3].Convert2Decimal(0),
                        SaleOrderVolume = arr[4].Convert2Decimal(0),
                        BuyOrderVolume = arr[5].Convert2Decimal(0),
                        Type = arr[6],
                        SaleOrderId = arr[7].Convert2Int64(0),
                        SaleOrderPrice = arr[8].Convert2Decimal(0),
                        BuyOrderId = arr[9].Convert2Int64(0),
                        BuyOrderPrice = arr[10].Convert2Decimal(0)
                    });
                }
            }
            sr.Close();
        }

        public List<decimal> BuyAmount()
        {
            var t = from p in Data
                group p by new
                {
                    p.BuyOrderId
                }
                into g
                select new
                {
                    g.Key,
                    amount = g.Sum(p => p.Price*p.Volume)
                };
            return t.Select(p => p.amount).OrderByDescending(p => p).ToList();
        }

        public List<decimal> SellAmount()
        {
            var t = from p in Data
                    group p by new
                    {
                        p.SaleOrderId
                    }
                        into g
                        select new
                        {
                            g.Key,
                            amount = g.Sum(p => p.Price * p.Volume)
                        };
            return t.Select(p => p.amount).OrderByDescending(p => p).ToList();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace X.UI.Entities
{
    public enum MyStockMode
    {
        Index,
        AQS,
        Wave,
        AR,
        HHS,
        HS,
        THS,
        DS,
        TDS,
        WS,
        TWS,
        HB,
        THB,
        DB,
        TDB,
        WB,
        TWB,
        MB,
        IndexWave,
        MS
    }


    public class MyStock
    {
        public string Code { get; set; }

        public string Name { get; set; }

        public double Inc { get; set; }

        public double Close { get; set; }

        public double LastClose
        {
            get
            {
                return Close / (Inc / 100 + 1);
            }
        }

        public double Vol { get; set; }

        public double Amount { get; set; }

        public double S1 { get; set; }

        public double S2 { get; set; }

        public double S3 { get; set; }

        public double S4 { get; set; }

        public double K1 { get; set; }

        public double K2 { get; set; }

        public double K3 { get; set; }

        public double K4 { get; set; }

        public double Cap { get; set; }

        public int MT { get; set; }

        public decimal Order
        {
            get
            {
                return (Order1 + Order2 + Order3 + Order4 + Order5 + Order6 + Order7 + Order8) / 8;
            }
        }

        public decimal Order1 { get; set; }

        public decimal Order2 { get; set; }

        public decimal Order3 { get; set; }

        public decimal Order4 { get; set; }

        public decimal Order5 { get; set; }

        public decimal Order6 { get; set; }

        public decimal Order7 { get; set; }

        public decimal Order8 { get; set; }

        public MyStockMode MyStockMode { get; set; }
    }
}

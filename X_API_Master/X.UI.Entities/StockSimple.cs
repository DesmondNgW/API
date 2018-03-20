using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace X.UI.Entities
{

    public class StockSimple
    {
        public double Open { get; set; }

        public double Close { get; set; }

        public double High { get; set; }

        public double Low { get; set; }

        public DateTime Date { get; set; }

        public string StockCode { get; set; }

        public string StockName { get; set; }
    }
}

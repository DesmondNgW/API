using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace X.Business.Entities.Stock
{
    public class StockCompare
    {
        public string Code { get; set; }

        public string Name { get; set; }

        public decimal Inc { get; set; }

        public decimal DDX { get; set; }

        public decimal DDXWeek { get; set; }

        public decimal Amount { get; set; }

        public int DDXOrder { get; set; }

        public string Mode { get; set; }
    }
}

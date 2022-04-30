using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace X.Business.Entities.Stock
{
    public class StockDes
    {
        public string Code { get; set; }

        public string Name { get; set; }

        public List<string> Bk { get; set; }
    }
}

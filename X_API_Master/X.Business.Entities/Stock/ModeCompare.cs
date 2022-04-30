using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace X.Business.Entities.Stock
{
    public class ModeCompare
    {
        public string Name { get; set; }

        public List<StockCompare> CodeList { get; set; }

        public decimal DDX
        {
            get
            {
                var sumAmount = CodeList.Sum(p => p.Amount);
                return CodeList.Sum(p => p.Amount / sumAmount * p.DDX);
            }
        }

        public decimal DDXWeek
        {
            get
            {
                var sumAmount = CodeList.Sum(p => p.Amount);
                return CodeList.Sum(p => p.Amount / sumAmount * p.DDXWeek);
            }
        }


        public double DDXOrder
        {
            get
            {
                return CodeList.Average(p => p.DDXOrder);
            }
        }

        public decimal Inc
        {
            get
            {
                var sumAmount = CodeList.Sum(p => p.Amount);
                return CodeList.Sum(p => p.Amount / sumAmount * p.Inc);
            }
        }

        public string Remark { get; set; }
    }
}

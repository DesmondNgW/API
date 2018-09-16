using System;
using System.Globalization;
using System.Linq;
using System.Text;
using X.UI.Helper;

namespace X.UI.Web
{
    public partial class Test : System.Web.UI.Page
    {
        public static StringBuilder sb = new StringBuilder();

        public string GetString(int count, int total)
        {
            return string.Format("{0}({1})", count, ((double)count / (double)total).ToString("0.00"));
        }

        public void Deal()
        {
            sb.AppendFormat("<tr>" +
                "<td>{0}</td>" +
                "<td>{1}</td><td>{2}</td><td>{3}</td><td>{4}</td><td>{5}</td><td>{6}</td><td>{7}</td><td>{8}</td>" +
                "<td>{9}</td><td>{10}</td><td>{11}</td><td>{12}</td><td>{13}</td><td>{14}</td><td>{15}</td><td>{16}</td>" +
                "<td>{17}</td><td>{18}</td><td>{19}</td><td>{20}</td><td>{21}</td><td>{22}</td><td>{23}</td><td>{24}</td><td>{25}</td>" +
                "<td>{26}</td>" +
                "</tr>",
                "时间", "First9:30-10:00", "10:00-10:30", "10:30-11:00", "11:00-11:30", "13:00-13:30", "13:30-14:00", "14:00-14:30", "14:30-15:00",
                "Last9:30-10:00", "10:00-10:30", "10:30-11:00", "11:00-11:30", "13:00-13:30", "13:30-14:00", "14:00-14:30", "14:30-15:00",
                "<1亿", "1-2亿", "2-3亿", "3-4亿", "4-5亿", "5-6亿", "6-7亿", "7-8亿", ">8亿", "总");
            var data = StockHelper.GetDataFromMongo();
            var d = from p in data group p by p.Date into g select g;
            foreach (var item in d)
            {
                int Total = item.Count();
                int t1 = item.Count(p => p.FirstTime > 0 && p.FirstTime <= 1000);
                int t2 = item.Count(p => p.FirstTime > 1000 && p.FirstTime <= 1030);
                int t3 = item.Count(p => p.FirstTime > 1030 && p.FirstTime <= 1100);
                int t4 = item.Count(p => p.FirstTime > 1100 && p.FirstTime <= 1130);
                int t5 = item.Count(p => p.FirstTime >= 1300 && p.FirstTime <= 1330);
                int t6 = item.Count(p => p.FirstTime > 1330 && p.FirstTime <= 1400);
                int t7 = item.Count(p => p.FirstTime > 1400 && p.FirstTime <= 1430);
                int t8 = item.Count(p => p.FirstTime > 1430 && p.FirstTime <= 1500);

                int tt1 = item.Count(p => p.LastTime > 0 && p.LastTime <= 1000);
                int tt2 = item.Count(p => p.LastTime > 1000 && p.LastTime <= 1030);
                int tt3 = item.Count(p => p.LastTime > 1030 && p.LastTime <= 1100);
                int tt4 = item.Count(p => p.LastTime > 1100 && p.LastTime <= 1130);
                int tt5 = item.Count(p => p.LastTime >= 1300 && p.LastTime <= 1330);
                int tt6 = item.Count(p => p.LastTime > 1330 && p.LastTime <= 1400);
                int tt7 = item.Count(p => p.LastTime > 1400 && p.LastTime <= 1430);
                int tt8 = item.Count(p => p.LastTime > 1430 && p.LastTime <= 1500);

                int a1 = item.Count(p => p.Amount <= 100000000);
                int a2 = item.Count(p => p.Amount > 100000000 && p.Amount <= 200000000);
                int a3 = item.Count(p => p.Amount > 200000000 && p.Amount <= 300000000);
                int a4 = item.Count(p => p.Amount > 300000000 && p.Amount <= 400000000);
                int a5 = item.Count(p => p.Amount > 400000000 && p.Amount <= 500000000);
                int a6 = item.Count(p => p.Amount > 500000000 && p.Amount <= 600000000);
                int a7 = item.Count(p => p.Amount > 600000000 && p.Amount <= 700000000);
                int a8 = item.Count(p => p.Amount > 700000000 && p.Amount <= 800000000);
                int a9 = item.Count(p => p.Amount > 800000000);
                int none = item.Count(p => p.High == p.Low);
                int lan = item.Count(p => p.CountOfHighPrice > 2);
                sb.AppendFormat("<tr>" +
                                "<td>{0}</td>" +
                                "<td>{1}</td><td>{2}</td><td>{3}</td><td>{4}</td><td>{5}</td><td>{6}</td><td>{7}</td><td>{8}</td>" +
                                "<td>{9}</td><td>{10}</td><td>{11}</td><td>{12}</td><td>{13}</td><td>{14}</td><td>{15}</td><td>{16}</td>" +
                                "<td>{17}</td><td>{18}</td><td>{19}</td><td>{20}</td><td>{21}</td><td>{22}</td><td>{23}</td><td>{24}</td><td>{25}</td>" +
                                "<td>{26}</td>" +
                                "</tr>",
                                item.Key,
                                GetString(t1,Total), GetString(t2, Total), GetString(t3, Total), GetString(t4, Total),
                                GetString(t5, Total), GetString(t6, Total), GetString(t7, Total), GetString(t8, Total),
                                GetString(tt1, Total), GetString(tt2, Total), GetString(tt3, Total), GetString(tt4, Total),
                                GetString(tt5, Total), GetString(tt6, Total), GetString(tt7, Total), GetString(tt8, Total),
                                a1, a2, a3, a4, a5, a6, a7, a8, a9, Total);
            }
        }

        public void Deal2()
        {
            sb.AppendFormat("<tr>" +
                "<td>{0}</td>" +
                //"<td>{1}</td><td>{2}</td><td>{3}</td><td>{4}</td><td>{5}</td><td>{6}</td><td>{7}</td><td>{8}</td>" +
                //"<td>{9}</td><td>{10}</td><td>{11}</td><td>{12}</td><td>{13}</td><td>{14}</td><td>{15}</td><td>{16}</td>" +
                //"<td>{17}</td><td>{18}</td><td>{19}</td><td>{20}</td><td>{21}</td><td>{22}</td><td>{23}</td><td>{24}</td><td>{25}</td>" +
                "<td>{26}</td>" +
                "</tr>",
                "时间", "First9:30-10:00", "10:00-10:30", "10:30-11:00", "11:00-11:30", "13:00-13:30", "13:30-14:00", "14:00-14:30", "14:30-15:00",
                "Last9:30-10:00", "10:00-10:30", "10:30-11:00", "11:00-11:30", "13:00-13:30", "13:30-14:00", "14:00-14:30", "14:30-15:00",
                "<1亿", "1-2亿", "2-3亿", "3-4亿", "4-5亿", "5-6亿", "6-7亿", "7-8亿", ">8亿", "总");
            var data = StockHelper.GetDataFromMongo();
            var d = from p in data group p by p.Date into g select g;
            foreach (var item in d)
            {
                int Total = item.Count();
                int t1 = item.Count(p => p.FirstTime > 0 && p.FirstTime <= 1000);
                int t2 = item.Count(p => p.FirstTime > 1000 && p.FirstTime <= 1030);
                int t3 = item.Count(p => p.FirstTime > 1030 && p.FirstTime <= 1100);
                int t4 = item.Count(p => p.FirstTime > 1100 && p.FirstTime <= 1130);
                int t5 = item.Count(p => p.FirstTime >= 1300 && p.FirstTime <= 1330);
                int t6 = item.Count(p => p.FirstTime > 1330 && p.FirstTime <= 1400);
                int t7 = item.Count(p => p.FirstTime > 1400 && p.FirstTime <= 1430);
                int t8 = item.Count(p => p.FirstTime > 1430 && p.FirstTime <= 1500);

                int tt1 = item.Count(p => p.LastTime > 0 && p.LastTime <= 1000);
                int tt2 = item.Count(p => p.LastTime > 1000 && p.LastTime <= 1030);
                int tt3 = item.Count(p => p.LastTime > 1030 && p.LastTime <= 1100);
                int tt4 = item.Count(p => p.LastTime > 1100 && p.LastTime <= 1130);
                int tt5 = item.Count(p => p.LastTime >= 1300 && p.LastTime <= 1330);
                int tt6 = item.Count(p => p.LastTime > 1330 && p.LastTime <= 1400);
                int tt7 = item.Count(p => p.LastTime > 1400 && p.LastTime <= 1430);
                int tt8 = item.Count(p => p.LastTime > 1430 && p.LastTime <= 1500);

                int a1 = item.Count(p => p.Amount <= 100000000);
                int a2 = item.Count(p => p.Amount > 100000000 && p.Amount <= 200000000);
                int a3 = item.Count(p => p.Amount > 200000000 && p.Amount <= 300000000);
                int a4 = item.Count(p => p.Amount > 300000000 && p.Amount <= 400000000);
                int a5 = item.Count(p => p.Amount > 400000000 && p.Amount <= 500000000);
                int a6 = item.Count(p => p.Amount > 500000000 && p.Amount <= 600000000);
                int a7 = item.Count(p => p.Amount > 600000000 && p.Amount <= 700000000);
                int a8 = item.Count(p => p.Amount > 700000000 && p.Amount <= 800000000);
                int a9 = item.Count(p => p.Amount > 800000000);
                int none = item.Count(p => p.High == p.Low);
                int lan = item.Count(p => p.CountOfHighPrice > 2);
                sb.AppendFormat("<tr>" +
                                "<td>{0}</td>" +
                                "<td>{1}</td><td>{2}</td><td>{3}</td><td>{4}</td><td>{5}</td><td>{6}</td><td>{7}</td><td>{8}</td>" +
                                "<td>{9}</td><td>{10}</td><td>{11}</td><td>{12}</td><td>{13}</td><td>{14}</td><td>{15}</td><td>{16}</td>" +
                                "<td>{17}</td><td>{18}</td><td>{19}</td><td>{20}</td><td>{21}</td><td>{22}</td><td>{23}</td><td>{24}</td><td>{25}</td>" +
                                "<td>{26}</td>" +
                                "</tr>",
                                item.Key,
                                GetString(t1, Total), GetString(t2, Total), GetString(t3, Total), GetString(t4, Total),
                                GetString(t5, Total), GetString(t6, Total), GetString(t7, Total), GetString(t8, Total),
                                GetString(tt1, Total), GetString(tt2, Total), GetString(tt3, Total), GetString(tt4, Total),
                                GetString(tt5, Total), GetString(tt6, Total), GetString(tt7, Total), GetString(tt8, Total),
                                a1, a2, a3, a4, a5, a6, a7, a8, a9, Total);
            }
        }


        protected void Page_Load(object sender, EventArgs e)
        {
            sb.Append("<table>");
            Deal();
            sb.Append("</table>");
        }
    }
}
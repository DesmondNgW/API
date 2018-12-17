using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using X.UI.Entities;
using X.UI.Helper;
using X.Util.Other;

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
            var context = new System.Web.HttpContextWrapper(Context);
            var tab = QueryInfo.GetQueryInt(context, "tab", 0);
            var dt = QueryInfo.GetQueryDateTime(context, "dt", "yyyyMMdd", DateTime.Now.Date);
            sb.AppendFormat("<tr>" +
                            "<td>{0}</td>" +
                            "<td>{1}</td><td>{2}</td><td>{3}</td><td>{4}</td>" +
                            "<td>{5}</td><td>{6}</td><td>{7}</td><td>{8}</td>" +
                            "<td>{9}</td><td>{10}</td>" +
                            "<td>{11}</td><td>{12}</td>" +
                "</tr>",
                "时间",
                "代码", "名称", "价格", "涨幅",
                "封成比", "封流比", "封单金额<亿元>", "金额<亿元>",
                "第一次涨停", "最后一次涨停",
                "打开次数", "涨停强度");

            var idata = JRJDataHelper.WebUI(DateTime.Now.AddMonths(-6), new TimeSpan(15, 0, 0));
            foreach (var item in idata.OrderByDescending(p => p.DateTime))
            {
                sb.AppendFormat("<tr>" +
                "<td>{0}</td>" +
                "<td>{1}</td><td>{2}</td><td>{3}</td><td>{4}</td>" +
                "<td>{5}</td><td>{6}</td><td>{7}</td><td>{8}</td>" +
                "<td>{9}</td><td>{10}</td>" +
                "<td>{11}</td><td>{12}</td>" +
                "</tr>",
                item.DateTime.ToString("yyyy-MM-dd"),
                item.StockCode, item.StockName, item.Price, item.PriceLimit,
                item.FCB.ToString("0.0000"), (item.FLB * 100).ToString("0.0000"), (item.FDMoney / 1E8M).ToString("0.0000"), (item.Amount / 1E8M).ToString("0.0000"),
                item.FirstZtTime.ToString("HH:mm:dd"), item.LastZtTime.ToString("HH:mm:dd"),
                item.OpenTime, item.Force.ToString("0.0000"));
            }
        }


        protected void Page_Load(object sender, EventArgs e)
        {
            sb = new StringBuilder();
            var context = new System.Web.HttpContextWrapper(Context);
            var dt = QueryInfo.GetQueryString(context, "dt", string.Empty);
            var tab = QueryInfo.GetQueryInt(context, "tab", 0);
            sb.AppendFormat("<nav><a class=\"{0}\" href=\"/Test.aspx?tab=0&dt={1}\">涨停</a>", tab == 0 ? "active" : "", dt);
            sb.AppendFormat("<a class=\"{0}\" href=\"/Test.aspx?tab=1&dt={1}\">烂板</a>", tab == 1 ? "active" : "", dt);
            sb.AppendFormat("<a class=\"{0}\" href=\"/Test.aspx?tab=2&dt={1}\">加速</a>", tab == 2 ? "active" : "", dt);
            sb.AppendFormat("<a class=\"{0}\" href=\"/Test.aspx?tab=3&dt={1}\">加速烂板</a>", tab == 3 ? "active" : "", dt);
            sb.AppendFormat("<a class=\"{0}\" href=\"/Test.aspx?tab=4&dt={1}\">加速小成交</a>", tab == 4 ? "active" : "", dt);
            sb.AppendFormat("<a class=\"{0}\" href=\"/Test.aspx?tab=5&dt={1}\">涨停小成交</a></nav>", tab == 5 ? "active" : "", dt);
            sb.AppendFormat("<table>");
            Deal();
            sb.AppendFormat("</table>");
        }
    }
}
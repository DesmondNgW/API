using System;
using System.Globalization;
using System.Linq;
using System.Text;
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
                "<td>{0}</td><td>{1}</td><td>{2}</td><td>{3}</td>" +
                "<td>{4}</td><td>{5}</td><td>{6}</td><td>{7}</td>" +
                "<td>{8}</td><td>{9}</td>" +
                "<td>{10}</td><td>{11}</td>"+
                "</tr>",
                "代码", "名称", "价格", "涨幅", 
                "封成比", "封流比", "封单金额", "金额",
                "第一次涨停", "最后一次涨停",
                "打开次数", "涨停强度");
            var data = JRJDataHelper.GetTab(dt, (EnumTab)tab);
            foreach (var item in data)
            {
                sb.AppendFormat("<tr>" +
                                "<td>{0}</td><td>{1}</td><td>{2}</td><td>{3}</td>" +
                                "<td>{4}</td><td>{5}</td><td>{6}</td><td>{7}</td>" +
                                "<td>{8}</td><td>{9}</td>" +
                                "<td>{10}</td><td>{11}</td>" +
                                "</tr>",
                                item.StockCode, item.StockName, item.Price, item.PriceLimit,
                                item.FCB, item.FLB, item.FDMoney, item.Amount,
                                item.FirstZtTime.ToString("HH:mm:dd"), item.LastZtTime.ToString("HH:mm:dd"),
                                item.OpenTime, item.Force);

            }
        }


        protected void Page_Load(object sender, EventArgs e)
        {
            sb = new StringBuilder();
            sb.Append("<a href=\"/Test.aspx?tab=0\">涨停</a>");
            sb.Append("<a href=\"/Test.aspx?tab=1\">烂板</a>");
            sb.Append("<a href=\"/Test.aspx?tab=2\">加速</a>");
            sb.Append("<a href=\"/Test.aspx?tab=3\">加速烂板</a>");
            sb.Append("<a href=\"/Test.aspx?tab=4\">加速小成交</a>");
            sb.Append("<a href=\"/Test.aspx?tab=5\">涨停小成交</a>");
            sb.Append("<table>");
            Deal();
            sb.Append("</table>");
        }
    }
}
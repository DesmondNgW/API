using System;
using System.Globalization;
using X.UI.Helper;

namespace X.UI.Web
{
    public partial class NoCache : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Response.AddHeader("Pragma", "no-cache");
            Response.AddHeader("Cache-Control", "no-cache, must-revalidate, no-store");
            Response.AddHeader("Expires", "-1");
            var list = StockDataHelper.History_Top_D();
            foreach (var item in list)
            {
                var dt = DateTime.ParseExact(item.Key.Split('-')[1], "yyyyMMdd", CultureInfo.InvariantCulture);
                var sk = StockHelper.GetStockMinute(item.Key, dt, item.Value);
            }
            Response.Cache.SetNoStore();
        }
    }
}
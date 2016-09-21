using System;
using System.Text;
using X.Stock.DB;
using X.Stock.Service;

namespace X.Web.Form
{
    public partial class Test : System.Web.UI.Page
    {
        public StringBuilder SbTable = new StringBuilder(); 

        protected void Page_Load(object sender, EventArgs e)
        {
            var list = ShareTable.GetStockShare(Work.CustomerNo);
            double stockDecimal = 0;
            if (list != null && list.Count > 0)
            {
                foreach (var item in list)
                {
                    SbTable.Append("<tr>");
                    var stock = StockService.GetStockInfo(StockService.GetStockId(item.StockCode));
                    stockDecimal += (stock.StockPrice)*item.AvailableVol;
                    SbTable.Append("<td>" + item.StockCode + "</td>");
                    SbTable.Append("<td>" + item.StockName + "</td>");
                    SbTable.Append("<td>" + item.CostValue + "</td>");
                    SbTable.Append("<td>" + item.CurrentStockPrice + "</td>");
                    SbTable.Append("<td>" + stock.StockKm2 + "%</td>");
                    SbTable.Append("<td>" + StockService.GetBenifit(item.CostValue, stock.StockPrice) + "</td>");
                    SbTable.Append("</tr>");
                }
            }
            else
            {
                SbTable.Append("<tr><td colspan='6' style='text-align:center'>暂无数据</td></tr>");
            }
            var cus = CustomerService.GetAssetInfo(Work.CustomerNo);
            var b = 100*((cus.CoinAsset + stockDecimal)/Work.CoinAsset - 1);
            coin.InnerHtml = cus.CoinAsset.ToString("0.00");
            total.InnerHtml = (cus.CoinAsset + stockDecimal).ToString("0.00");
            benifit.InnerHtml = b.ToString("0.00") + "%";
        }
    }
}
using System;
using System.Text;
using X.Stock.Service;
using X.Stock.Service.Utils;

namespace X.Web.Form
{
    public partial class Test : System.Web.UI.Page
    {
        public StringBuilder sbCus = new StringBuilder();
        public StringBuilder sbTable = new StringBuilder(); 

        protected void Page_Load(object sender, EventArgs e)
        {
            var list = CustomerService.GetStockShare(Work.CustomerNo);
            var stockDecimal = 0M;
            if (list != null)
            {
                sbTable.Append("<tr>");
                foreach (var item in list)
                {
                    var stock = StockService.GetStockInfo(StockService.GetStockId(item.StockCode));
                    stockDecimal += (stock.StockPrice - item.CostValue)*item.AvailableVol;
                    sbTable.Append("<td>" + item.StockCode + "</td>");
                    sbTable.Append("<td>" + item.StockName + "</td>");
                    sbTable.Append("<td>" + item.CostValue + "</td>");
                    sbTable.Append("<td>" + stock.StockKm2 + "%</td>");
                    sbTable.Append("<td>" + StockService.GetBenifit(item.CostValue, stock.StockPrice) + "</td>");
                }
                sbTable.Append("</tr>");
            }
            var cus = CustomerService.GetAssetInfo(Work.CustomerNo);
            sbCus.Append(string.Format("余额：{0}，总资产:{1},收益：{2}", cus.CoinAsset, cus.CoinAsset + stockDecimal,
                (cus.CoinAsset + stockDecimal)/Work.CoinAsset - 1));

        }
    }
}
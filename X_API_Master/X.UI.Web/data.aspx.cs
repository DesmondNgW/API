using System;
using System.Web;
using Em.FundTrade.EncryptHelper;
using X.Util.Core;
using X.Util.Other;

namespace X.UI.Web
{
    public partial class Data : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var context = new HttpContextWrapper(Context);
            var f = QueryInfo.GetQueryInt(context, "f", 1);
            var p1 = QueryInfo.GetQueryString(context, "p1", string.Empty);
            Response.Clear();
            switch (f)
            {
                case 1:
                    Response.Write(EncryptHelper.Instance.MobileEncrypt(p1).ToJson());
                    break;
                case 2:
                    Response.Write(ChineseConvert.Get(p1).ToJson());
                    break;
                default:
                    break;
            }
        }
    }
}
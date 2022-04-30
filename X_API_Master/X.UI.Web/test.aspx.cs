using System;
using X.Util.Other;

namespace X.UI.Web
{
    public partial class Test : System.Web.UI.Page
    {
        public string GetString(int count, int total)
        {
            return string.Format("{0}({1})", count, ((double)count / (double)total).ToString("0.00"));
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            var context = new System.Web.HttpContextWrapper(Context);
            var dt = QueryInfo.GetQueryString(context, "dt", string.Empty);
            var tab = QueryInfo.GetQueryInt(context, "tab", 0);
        }
    }
}
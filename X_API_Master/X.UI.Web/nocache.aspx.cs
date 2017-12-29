using System;
using System.Web;
using X.Util.Other;

namespace X.UI.Web
{
    public partial class NoCache : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Response.AddHeader("Cache-Control", "no-cache,must-revalidate");
            Response.Cache.SetNoStore();
            CookieHelper.SetCookie(new HttpContextWrapper(Context), "test", Guid.NewGuid().ToString("N"));
        }
    }
}
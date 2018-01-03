using System;
using System.Web;
using X.Util.Core;
using X.Util.Other;

namespace X.UI.Web
{
    public partial class NoCache : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Response.AddHeader("Pragma", "no-cache");
            Response.AddHeader("Cache-Control", "no-cache, must-revalidate, no-store");
            Response.AddHeader("Expires", "-1");
            Response.Cache.SetNoStore();
        }
    }
}
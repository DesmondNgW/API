using System;
using System.Web.Routing;
using X.Util.Core.Log;
using X.Util.Entities;

namespace X.Web.Form
{
    public class Global : System.Web.HttpApplication
    {
        public void RegisterRoutes(RouteCollection routes)
        {
            routes.Ignore("{resource}.axd/{*pathInfo}");
            routes.MapPageRoute("WebFormPage", "{webform}", "~/{webform}.aspx", false, new RouteValueDictionary { { "webform", "detail" } }, new RouteValueDictionary { { "webform", "[a-zA-Z_0-9]+" } });
            routes.MapPageRoute("WebFormPage2", "{folder}/{webform}", "~/{folder}/{webform}.aspx", false, new RouteValueDictionary { { "folder", "defaultfolder" }, { "webform", "default" } }, new RouteValueDictionary { { "webform", "[a-zA-Z_0-9]+" } });
        }

        protected void Application_Start(object sender, EventArgs e)
        {
            RegisterRoutes(RouteTable.Routes);
        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {
            var ex = Server.GetLastError();
            Logger.Client.Error(System.Reflection.MethodBase.GetCurrentMethod(), LogDomain.Ui, null, string.Empty, ex.ToString());
            if (Context.IsDebuggingEnabled) throw ex;
        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {

        }
    }
}
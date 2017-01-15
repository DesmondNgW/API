using System;
using System.Reflection;
using System.Web.Routing;
using X.Util.Core.Log;
using X.Util.Entities.Enums;

namespace X.UI.Update
{
    public class Global : System.Web.HttpApplication
    {
        public void RegisterRoutes(RouteCollection routes)
        {
            routes.Ignore("{resource}.axd/{*pathInfo}");
            routes.MapPageRoute(
                "WebForm",
                "{webform}",
                "~/{webform}.aspx",
                false,
                new RouteValueDictionary { { "webform", "default" } },
                new RouteValueDictionary { { "webform", "[a-zA-Z_0-9]+" } }
                );
            routes.MapPageRoute(
                "WebForm2",
                "{folder}/{webform}",
                "~/{folder}/{webform}.aspx",
                false,
                new RouteValueDictionary { { "folder", "index" }, { "webform", "default" } },
                new RouteValueDictionary { { "folder", "[a-zA-Z_0-9]+" }, { "webform", "[a-zA-Z_0-9]+" } }
                );
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
            Logger.Client.Error(Logger.Client.GetMethodInfo(MethodBase.GetCurrentMethod(), new object[] { }), ex, LogDomain.Ui);
            throw ex;
        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {

        }
    }
}
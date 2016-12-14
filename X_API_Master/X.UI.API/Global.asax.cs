﻿using System;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using X.Util.Core.Log;
using X.Util.Entities;
using X.Util.Entities.Enums;

namespace X.UI.API
{
	// 注意: 有关启用 IIS6 或 IIS7 经典模式的说明，
	// 请访问 http://go.microsoft.com/?LinkId=9394801

	public class WebApiApplication : System.Web.HttpApplication
	{
		protected void Application_Start()
		{
			AreaRegistration.RegisterAllAreas();
			WebApiConfig.Register(GlobalConfiguration.Configuration);
			FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
			RouteConfig.RegisterRoutes(RouteTable.Routes);
			BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        protected void Application_Error(object sender, EventArgs e)
		{
			Exception ex = Server.GetLastError();
            Logger.Client.Error(System.Reflection.MethodBase.GetCurrentMethod(), LogDomain.Ui, null, string.Empty, ex.ToString());
			throw ex;
		}
	}
}
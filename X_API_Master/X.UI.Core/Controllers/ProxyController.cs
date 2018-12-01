using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using X.Util.Other;

namespace X.UI.Core.Controllers
{
    [Route("api/[controller]")]
    public class ProxyController : Controller
    {
        private static string uri = "http://localhost/data.aspx";

        [HttpGet("[action]")]
        public string GetStockData(string dt, string tab)
        {
            if (string.IsNullOrEmpty(dt))
            {
                var now = DateTime.Now;
                if (now.DayOfWeek == DayOfWeek.Saturday)
                {
                    now = DateTime.Now.AddDays(-1);
                }
                else if (now.DayOfWeek == DayOfWeek.Sunday)
                {
                    now = DateTime.Now.AddDays(-2);
                }
                dt = now.ToString("yyyyMMdd");
            }
            var arguments = new Dictionary<string, string>()
            {
                {"dt",dt },
                {"tab",tab }
            };
            return ApiData.GetContent(uri, arguments, "application/json", null);
        }
    }
}

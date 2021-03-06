﻿using System;
using System.Linq;
using System.Web;
using Em.FundTrade.EncryptHelper;
using X.UI.Helper;
using X.Util.Core;
using X.Util.Core.Xml;
using X.Util.Other;

namespace X.UI.Web
{
    public partial class Data : System.Web.UI.Page
    {
        public void Out<T>(string contentType, T obj)
        {
            Response.Write(contentType.Contains("json") ? obj.ToJson() : XmlHelper.WriteXml(obj).ToString());
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            var context = new HttpContextWrapper(Context);
            var f = QueryInfo.GetQueryInt(context, "f", 0);
            var p1 = QueryInfo.GetQueryString(context, "p1", string.Empty);
            Response.Clear();
            var contentType = Request.ContentType;
            Response.ContentType = contentType.Contains("json") ? "application/json" : "application/xml";
            switch (f)
            {
                case 0:
                    var tab = QueryInfo.GetQueryInt(context, "tab", 0);
                    var dt = QueryInfo.GetQueryDateTime(context, "dt", "yyyyMMdd", DateTime.Now.Date);
                    var ret = JRJDataHelper.GetTab(dt, (EnumTab)tab);
                    Out(contentType, ret);
                    break;
                case 1:
                    Out(contentType, EncryptHelper.Instance.MobileEncrypt(p1));
                    break;
                case 2:
                    Out(contentType, ChineseConvert.Get(p1));
                    break;
                case 3:
                    var dic = StockMonitor.GetStockPrice();
                    var special = StockMonitor.GetSpecialStockPrice(dic);
                    special.AddRange(dic.Select(item => item.Value).OrderByDescending(p => p.Inc));
                    Out(contentType, special);
                    break;
                default:
                    break;
            }
        }
    }
}
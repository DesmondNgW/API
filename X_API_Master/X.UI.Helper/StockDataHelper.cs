using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using X.DataBase.Helper;
using X.UI.Entities;
using X.Util.Other;

namespace X.UI.Helper
{
    public class StockDataHelper
    {
        private static readonly DbHelper DbHelper = DalHelper.DbHelper;

        private static decimal GetValue(decimal? value)
        {
            return value != null ? value.Value : 0;
        }

        private static int GetValue(int? value)
        {
            return value != null ? value.Value : 0;
        }

        private static DateTime GetValue(DateTime? value)
        {
            return value != null ? value.Value : DateTime.MinValue;
        }
        #region 数据库数据

        /// <summary>
        /// 数据库数据
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public static List<Stock> GetRealStockData(string code)
        {
            var result = new List<Stock>();
            var ret = DbHelper.GetPageList("tradedata_tdx", "tdate desc", "scode='" + code + "'", 0, 10000);
            while (ret.Read())
            {
                var item = new Stock
                {
                    StockCode = DalHelper.GetString(ret["Scode"]),
                    StockName = DalHelper.GetString(ret["Sname"]),
                    Date = DalHelper.GetDateTime(ret["Tdate"]),
                    Open = DalHelper.GetDouble(ret["OpenPrice"], 0),
                    High = DalHelper.GetDouble(ret["HighPrice"], 0),
                    Close = DalHelper.GetDouble(ret["ClosePrice"], 0),
                    Low = DalHelper.GetDouble(ret["LowPrice"], 0),
                    StockSimple = new Dictionary<int, StockSimple>()
                };
                result.Add(item);
            }
            ret.Close();
            return result;
        }

        /// <summary>
        /// 数据库分钟线
        /// </summary>
        /// <param name="code"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        public static List<StockKLine> GetMinuteData(string code, DateTime date)
        {
            var result = new List<StockKLine>();
            var ret = DbHelper.GetPageList("TDXMinData", "TTime asc", "sCode='" + code + "' and  TDate='" + date.ToString("yyyy-MM-dd") + "'", 0, 300);
            while (ret.Read())
            {
                var item = new StockKLine
                {
                    StockCode = DalHelper.GetString(ret["SCode"]),
                    StockName = DalHelper.GetString(ret["SName"]),
                    Date = GetValue(DalHelper.GetDateTime(ret["TDate"])),
                    Open = GetValue(DalHelper.GetDecimal(ret["Open"])),
                    High = GetValue(DalHelper.GetDecimal(ret["High"])),
                    Close = GetValue(DalHelper.GetDecimal(ret["Close"])),
                    Low = GetValue(DalHelper.GetDecimal(ret["Low"])),
                    Time = GetValue(DalHelper.GetInt(ret["TTime"])),
                    Vol = GetValue(DalHelper.GetDecimal(ret["Vol"])),
                    Amount = GetValue(DalHelper.GetDecimal(ret["Amt"])),
                };
                result.Add(item);
            }
            ret.Close();
            return result;
        }

        /// <summary>
        /// 数据库日线
        /// </summary>
        /// <returns></returns>
        public static List<StockKLine> GetDayData()
        {
            var result = new List<StockKLine>();
            var ret = DbHelper.GetPageList("SKDay", "tdate asc", "chg>9.8", 0, 10000000);
            while (ret.Read())
            {
                var code = DalHelper.GetString(ret["SCode"]);
                var dt = GetValue(DalHelper.GetDateTime(ret["TDate"]));
                var item = new StockKLine
                {
                    Id = string.Format("{0}-{1}", code, dt.ToString("yyyyMMdd")),
                    StockCode = code,
                    StockName = DalHelper.GetString(ret["SName"]),
                    Date = dt,
                    Open = GetValue(DalHelper.GetDecimal(ret["OpenPrice"])),
                    High = GetValue(DalHelper.GetDecimal(ret["HighPrice"])),
                    Close = GetValue(DalHelper.GetDecimal(ret["ClosePrice"])),
                    Low = GetValue(DalHelper.GetDecimal(ret["LowPrice"])),
                    LastClose = GetValue(DalHelper.GetDecimal(ret["lastclose"])),
                    Hsl = GetValue(DalHelper.GetInt(ret["Hsl"])),
                    Vol = GetValue(DalHelper.GetDecimal(ret["Vol"])),
                    Amount = GetValue(DalHelper.GetDecimal(ret["amount"])),
                };
                result.Add(item);
            }
            ret.Close();
            return result;
        }

        /// <summary>
        /// 日线涨停，分钟数据
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, StockKLineCombine> StockKLineCombine()
        {
            var list = new Dictionary<string, StockKLineCombine>();
            using (var ret = DbHelper.ExecuteQuery(CommandType.Text, "select * from tdxmindata as a join (select * from skday where chg>9.8) as b on a.tdate=b.tdate and a.scode=b.scode order by a.tdate desc,a.scode,ttime asc;"))
            {
                foreach (DataTable dt in ret.Tables)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        var code = DalHelper.GetString(dr["SCode"]);
                        var name = DalHelper.GetString(dr["SName"]);
                        var time = GetValue(DalHelper.GetDateTime(dr["TDate"]));
                        var id = string.Format("{0}-{1}", code, time.ToString("yyyyMMdd"));
                        var item = new StockKLine
                        {
                            Id = id,
                            StockCode = code,
                            StockName = name,
                            Date = time,
                            Open = GetValue(DalHelper.GetDecimal(dr["Open"])),
                            High = GetValue(DalHelper.GetDecimal(dr["High"])),
                            Close = GetValue(DalHelper.GetDecimal(dr["Close"])),
                            Low = GetValue(DalHelper.GetDecimal(dr["Low"])),
                            Time = GetValue(DalHelper.GetInt(dr["TTime"])),
                            Vol = GetValue(DalHelper.GetDecimal(dr["Vol"])),
                            Amount = GetValue(DalHelper.GetDecimal(dr["Amt"])),
                        };
                        if (!list.ContainsKey(id))
                        {
                            var day = new StockKLine
                            {
                                Id = id,
                                StockCode = code,
                                StockName = name,
                                Date = time,
                                Open = GetValue(DalHelper.GetDecimal(dr["openprice"])),
                                High = GetValue(DalHelper.GetDecimal(dr["highprice"])),
                                Close = GetValue(DalHelper.GetDecimal(dr["closeprice"])),
                                Low = GetValue(DalHelper.GetDecimal(dr["lowprice"])),
                                LastClose = GetValue(DalHelper.GetDecimal(dr["lastclose"])),
                                Hsl = GetValue(DalHelper.GetDecimal(dr["hsl"])),
                                Vol = GetValue(DalHelper.GetDecimal(dr["vol"])),
                                Amount = GetValue(DalHelper.GetDecimal(dr["amount"])),
                            };
                            list[id] = new StockKLineCombine
                            {
                                Id = id,
                                Day = day,
                                Minutes = new List<StockKLine>() { item }
                            };
                        }
                        else
                        {
                            list[id].Minutes.Add(item);
                        }
                    }
                }
            }
            return list;
        }
        #endregion

        #region http 接口数据
        /// <summary>
        /// 股票代码列表
        /// </summary>
        /// <returns></returns>
        public static List<string> GetStockList()
        {
            var ret = new List<string>();
            var regex = new Regex("<li><a target=\"_blank\" href=\"http://quote.eastmoney.com/(.+?).html\">(.+?)</a></li>");
            var content = HttpRequestBase.GetHttpInfo("http://quote.eastmoney.com/stocklist.html", "gb2312", "application/json", null, string.Empty);
            var list = regex.Matches(content.Content);
            if (list != null)
            {
                foreach (Match Match in list)
                {
                    var target = Match.Groups[1].ToString();
                    if (target.StartsWith("sh60") || target.StartsWith("sz002") || target.StartsWith("sz300"))
                    {
                        ret.Add(target.Substring(2));
                    }
                }
            }
            return ret;
        }

        /// <summary>
        /// 单个股票行情实时
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public static StockPrice GetStockPrice(string code)
        {
            var uri = code.StartsWith("6") ? "http://hq.sinajs.cn/list=sh" + code : "http://hq.sinajs.cn/list=sz" + code;
            var ret = HttpRequestBase.GetHttpInfo(uri, "gb2312", "application/json", null, string.Empty);
            if (string.IsNullOrEmpty(ret.Content)) return default(StockPrice);
            var content = ret.Content.Split('\"')[1];
            var arr = content.Split(',');
            if (string.IsNullOrEmpty(content) || arr.Length == 1) return default(StockPrice);
            var result = new StockPrice
            {
                Id = string.Format("{0}-{1}", code, arr[30]),
                StockCode = code,
                StockName = arr[0],
                CurrentPrice = decimal.Parse(arr[3]),
                MaxPrice = decimal.Parse(arr[4]),
                MinPrice = decimal.Parse(arr[5]),
                OpenPrice = decimal.Parse(arr[1]),
                LastClosePrice = decimal.Parse(arr[2]),
                Datetime = DateTime.Parse(arr[30] + " " + arr[31]),
                Vol = decimal.Parse(arr[8]),
                Amount = decimal.Parse(arr[9]) / 100000000M,
                MY = decimal.Parse(arr[10]),
                Indexs = new List<string>()
            };
            if (result.CurrentPrice != 0) result.Inc = result.CurrentPrice / result.LastClosePrice * 100 - 100;
            if (result.MaxPrice != 0) result.MaxInc = result.MaxPrice / result.LastClosePrice * 100 - 100;
            if (result.MinPrice != 0) result.MinInc = result.MinPrice / result.LastClosePrice * 100 - 100;
            return result;
        }

        /// <summary>
        /// 获取指数价格
        /// </summary>
        /// <returns></returns>
        public static StockPrice GetIndexPrice(string code)
        {
            var uri = "http://hq.sinajs.cn/list=" + code;
            var ret = HttpRequestBase.GetHttpInfo(uri, "gb2312", "application/json", null, string.Empty);
            if (string.IsNullOrEmpty(ret.Content)) return default(StockPrice);
            var content = ret.Content.Split('\"')[1];
            var arr = content.Split(',');
            if (string.IsNullOrEmpty(content) || arr.Length == 1) return default(StockPrice);
            var result = new StockPrice
            {
                Id = string.Format("{0}-{1}", code, arr[30]),
                StockCode = code,
                StockName = arr[0],
                CurrentPrice = decimal.Parse(arr[3]),
                MaxPrice = decimal.Parse(arr[4]),
                MinPrice = decimal.Parse(arr[5]),
                OpenPrice = decimal.Parse(arr[1]),
                LastClosePrice = decimal.Parse(arr[2]),
                Datetime = DateTime.Parse(arr[30] + " " + arr[31]),
                Amount = decimal.Parse(arr[9]) / 100000000M,
                MY = decimal.Parse(arr[10]),
                Indexs = new List<string>()
            };
            if (result.CurrentPrice != 0) result.Inc = result.CurrentPrice / result.LastClosePrice * 100 - 100;
            if (result.MaxPrice != 0) result.MaxInc = result.MaxPrice / result.LastClosePrice * 100 - 100;
            if (result.MinPrice != 0) result.MinInc = result.MinPrice / result.LastClosePrice * 100 - 100;
            return result;
        }

        /// <summary>
        /// 所有股票行情
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, StockPrice> GetStockPrice(IEnumerable<MyStock> list)
        {
            var ret = new Dictionary<string, StockPrice>();
            foreach (var item in list.Where(item => !ret.ContainsKey(item.Code) && !string.IsNullOrEmpty(item.Code)))
            {
                var sp = GetStockPrice(item.Code);
                if (sp != null)
                {
                    ret[item.Code] = GetStockPrice(item.Code);
                }
            }
            return ret;
        }
        #endregion
    }
}

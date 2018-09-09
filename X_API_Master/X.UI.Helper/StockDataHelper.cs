using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using X.DataBase.Helper;
using X.UI.Entities;
using X.Util.Core;
using X.Util.Core.Common;
using X.Util.Extend.Mongo;

namespace X.UI.Helper
{
    public class StockDataHelper
    {
        private static readonly DbHelper DbHelper = DalHelper.DbHelper;

        public static double Std(List<Stock> list)
        {
            var ma = list.Average(p => p.Close);
            var std = Math.Pow(list.Sum(p => Math.Pow(p.Close - ma, 2))/list.Count, 0.5);
            return std;
        }

        public static double Score(Stock current, Stock previous)
        {
            return current.High/previous.High*
                   current.Open/previous.Close*
                   current.Close/previous.Close*
                   current.Close/previous.Close*
                   2*current.Close/(current.High + current.Low)*
                   current.Low/previous.Low*
                   current.Close/previous.High;
        }

        public static double ScoreMax(Stock current, Stock previous)
        {
            return current.High / previous.High *
                   current.Open / previous.Close *
                   current.High / previous.Close *
                   current.High / previous.Close *
                   2 * current.High / (current.High + current.Low) *
                   current.Low / previous.Low *
                   current.High / previous.High;
        }

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

        public static Stock GetStock(Stock t, double prev, int count)
        {
            var f = 0.2*(StringConvert.SysRandom.NextDouble() - 0.5)*prev;
            t.Open = f;
            t.High = f;
            t.Low = f;
            t.Close = f;
            while (count-- > 0)
            {
                t.Close = 0.2*(StringConvert.SysRandom.NextDouble() - 0.5)*prev;
                t.High = Math.Max(t.High, 0.2*(StringConvert.SysRandom.NextDouble() - 0.5)*prev);
                t.Low = Math.Min(t.Low, 0.2*(StringConvert.SysRandom.NextDouble() - 0.5)*prev);
            }
            return t;
        }

        public static List<Stock> GetTestStockData(int count)
        {
            var result = new List<Stock>();
            var ret = default(Stock);
            while (count-- > 0)
            {
                var prev = ret != null ? ret.Close : 14;
                ret = new Stock
                {
                    StockSimple = new Dictionary<int, StockSimple>(),
                    StockCode = "TestCode",
                    StockName = "TestName",
                    Date = DateTime.MaxValue
                };
                ret = GetStock(ret, prev, 240);
                result.Add(ret);
            }
            return result;
        }

        public static List<Stock> StockData(string code, bool test = false)
        {
            var result = test ? GetTestStockData(25000) : GetRealStockData(code);
            result = result.OrderBy(p => p.Date).ToList();
            for (var i = 0; i < result.Count; i++)
            {
                result[i].HeiKinAShiOpen = i == 0 ? 0 : (result[i - 1].Open + result[i - 1].Close)/2;
                result[i].Inc = i == 0 ? 0 : (result[i].Close - result[i - 1].Close)/result[i - 1].Close;
                result[i].Ma = i < 4 ? 0 : result.Skip(i - 4).Take(5).Average(p => p.Close);
                result[i].Std = i < 4 ? 0 : Std(result.Skip(i - 4).Take(5).ToList());
                result[i].ZScoreMa = result.Take(i + 1).Average(p => p.ZScore);
                result[i].Score = i == 0 ? 0 : Score(result[i], result[i - 1]);
                result[i].ScoreMax = i == 0 ? 0 : ScoreMax(result[i], result[i - 1]);
                for (var j = 1; j <= 20; j++)
                {
                    if (i + j < result.Count)
                    {
                        result[i].StockSimple[j] = new StockSimple
                        {
                            Low = result[i + j].Low,
                            High = result[i + j].High,
                            Open = result[i + j].Open,
                            Close = result[i + j].Close,
                            StockCode = result[i + j].StockCode,
                            StockName = result[i + j].StockName,
                            Date = result[i + j].Date ?? DateTime.MinValue
                        };
                    }
                }
                for (var j = 1; j <= 20; j++)
                {
                    if (!result[i].StockSimple.ContainsKey(j))
                    {
                        result[i].StockSimple[j] = default(StockSimple);
                    }
                }
            }
            return result.OrderBy(p=>p.Date).ToList();
        }

        public static decimal GetValue(decimal? value)
        {
            return value!=null? value.Value:0;
        }

        public static int GetValue(int? value)
        {
            return value != null ? value.Value : 0;
        }

        public static DateTime GetValue(DateTime? value)
        {
            return value != null ? value.Value : DateTime.MinValue;
        }

        public static List<StockKLine> GetMinuteData(string code ,DateTime date)
        {
            var result = new List<StockKLine>();
            var ret = DbHelper.GetPageList("TDXMinData", "TTime asc", "sCode='" + code + "' and  TDate='"+ date.ToString("yyyy-MM-dd") + "'", 0, 300);
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

        public static Dictionary<string, List<StockKLine>> History_Top_D()
        {
            var ret = DbHelper.ExecuteQuery(CommandType.Text, "select * from tdxmindata as a join (select chg,tdate,scode from skday where chg>9.8) as b on a.tdate=b.tdate and a.scode=b.scode order by a.tdate desc,a.scode,ttime asc;");
            var list = new Dictionary<string,List<StockKLine>>();
            foreach (DataTable dt in ret.Tables)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    var code = DalHelper.GetString(dr["SCode"]);
                    var time = GetValue(DalHelper.GetDateTime(dr["TDate"]));
                    var item = new StockKLine
                    {
                        Id = string.Format("{0}-{1}", code, time.ToString("yyyyMMdd")),
                        StockCode = code,
                        StockName = DalHelper.GetString(dr["SName"]),
                        Date = time,
                        Open = GetValue(DalHelper.GetDecimal(dr["Open"])),
                        High = GetValue(DalHelper.GetDecimal(dr["High"])),
                        Close = GetValue(DalHelper.GetDecimal(dr["Close"])),
                        Low = GetValue(DalHelper.GetDecimal(dr["Low"])),
                        Time = GetValue(DalHelper.GetInt(dr["TTime"])),
                        Vol = GetValue(DalHelper.GetDecimal(dr["Vol"])),
                        Amount = GetValue(DalHelper.GetDecimal(dr["Amt"])),
                    };
                    if (!list.ContainsKey(item.Id))
                    {
                        list[item.Id] = new List<StockKLine>();
                    }
                    else
                    {
                        list[item.Id].Add(item);
                    }
                }
            }
            return list;
        }
    }
}

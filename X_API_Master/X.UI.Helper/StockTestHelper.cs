using System;
using System.Collections.Generic;
using System.Linq;
using X.UI.Entities;
using X.Util.Core.Common;
using X.Util.Core.Log;
using X.Util.Entities.Enums;

namespace X.UI.Helper
{
    //基本分析 => 确认热点题材以及龙头
    //技术分析 => 平均理念/摆荡理念/分时强度

    public class StockTestHelper
    {
        #region 模拟随机行情数据
        public static Stock GetStock(Stock t, double prev, int count)
        {
            var f = 0.2 * (StringConvert.SysRandom.NextDouble() - 0.5) * prev;
            t.Open = f;
            t.High = f;
            t.Low = f;
            t.Close = f;
            while (count-- > 0)
            {
                t.Close = 0.2 * (StringConvert.SysRandom.NextDouble() - 0.5) * prev;
                t.High = Math.Max(t.High, 0.2 * (StringConvert.SysRandom.NextDouble() - 0.5) * prev);
                t.Low = Math.Min(t.Low, 0.2 * (StringConvert.SysRandom.NextDouble() - 0.5) * prev);
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
            var result = test ? GetTestStockData(25000) : StockDataHelper.GetRealStockData(code);
            result = result.OrderBy(p => p.Date).ToList();
            for (var i = 0; i < result.Count; i++)
            {
                result[i].HeiKinAShiOpen = i == 0 ? 0 : (result[i - 1].Open + result[i - 1].Close) / 2;
                result[i].Inc = i == 0 ? 0 : (result[i].Close - result[i - 1].Close) / result[i - 1].Close;
                result[i].Ma = i < 4 ? 0 : result.Skip(i - 4).Take(5).Average(p => p.Close);
                result[i].Std = i < 4 ? 0 : StockScoreHelper.Std(result.Skip(i - 4).Take(5).ToList());
                result[i].ZScoreMa = result.Take(i + 1).Average(p => p.ZScore);
                result[i].Score = i == 0 ? 0 : StockScoreHelper.Score(result[i], result[i - 1]);
                result[i].ScoreMax = i == 0 ? 0 : StockScoreHelper.ScoreMax(result[i], result[i - 1]);
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
            return result.OrderBy(p => p.Date).ToList();
        }
        #endregion

        public static void Test(string code, Func<Stock, bool> condition)
        {
            var result = new Dictionary<int, StockResult>();
            var g = StockData(code);
            var ret = g.Where(condition).ToList();
            foreach (var t in ret)
            {
                for (var j = 1; j <= 20; j++)
                {
                    if (result.ContainsKey(j))
                    {
                        if (result[j].NextDateTime < t.Date)
                        {
                            if (t.StockSimple[j] != null)
                            {
                                result[j].Nav1 *= t.StockSimple[j].Low/t.Low;
                                result[j].Nav2 *= t.StockSimple[j].Low/t.Close;
                                result[j].Nav3 *= t.StockSimple[j].Low/t.High;
                                result[j].Nav4 *= t.StockSimple[j].Close/t.Low;
                                result[j].Nav5 *= t.StockSimple[j].Close/t.Close;
                                result[j].Nav6 *= t.StockSimple[j].Close/t.High;
                                result[j].Nav7 *= t.StockSimple[j].High/t.Low;
                                result[j].Nav8 *= t.StockSimple[j].High/t.Close;
                                result[j].Nav9 *= t.StockSimple[j].High/t.High;
                                result[j].Times++;
                            }
                        }
                    }
                    else
                    {
                        result[j] = new StockResult
                        {
                            Nav1 = t.StockSimple[j].Low / t.Low,
                            Nav2 = t.StockSimple[j].Low / t.Close,
                            Nav3 = t.StockSimple[j].Low / t.High,
                            Nav4 = t.StockSimple[j].Close / t.Low,
                            Nav5 = t.StockSimple[j].Close / t.Close,
                            Nav6 = t.StockSimple[j].Close / t.High,
                            Nav7 = t.StockSimple[j].High / t.Low,
                            Nav8 = t.StockSimple[j].High / t.Close,
                            Nav9 = t.StockSimple[j].High / t.High,
                            StockCode = t.StockCode,
                            StockName = t.StockName,
                            Times = 1,
                            NextDateTime = t.StockSimple[j].Date
                        };
                    } 
                }
            }
            foreach (var item in result)
            {
                //if (item.Value.Nav1 > 1)
                {
                    Console.WriteLine("最低价买入、最低价卖出 {0}日净值:{1},操作次数:{2}", item.Key, item.Value.Nav1, item.Value.Times);
                    Logger.Client.Debug(
                        string.Format("最低价买入、最低价卖出 {0}日净值:{1},操作次数:{2}", item.Key, item.Value.Nav1, item.Value.Times),
                        LogDomain.Ui);
                }

                //if (item.Value.Nav2 > 1)
                {
                    Console.WriteLine("收盘价买入、最低价卖出 {0}日净值:{1},操作次数:{2}", item.Key, item.Value.Nav2, item.Value.Times);
                    Logger.Client.Debug(
                        string.Format("收盘价买入、最低价卖出 {0}日净值:{1},操作次数:{2}", item.Key, item.Value.Nav2, item.Value.Times),
                        LogDomain.Ui);
                }

                //if (item.Value.Nav3 > 1)
                {
                    Console.WriteLine("最高价买入、最低价卖出 {0}日净值:{1},操作次数:{2}", item.Key, item.Value.Nav3, item.Value.Times);
                    Logger.Client.Debug(
                        string.Format("最高价买入、最低价卖出 {0}日净值:{1},操作次数:{2}", item.Key, item.Value.Nav3, item.Value.Times),
                        LogDomain.Ui);
                }

                //if (item.Value.Nav4 > 1)
                {
                    Console.WriteLine("最低价买入、收盘价卖出 {0}日净值:{1},操作次数:{2}", item.Key, item.Value.Nav4, item.Value.Times);
                    Logger.Client.Debug(
                        string.Format("最低价买入、收盘价卖出 {0}日净值:{1},操作次数:{2}", item.Key, item.Value.Nav4, item.Value.Times),
                        LogDomain.Ui);
                }

                //if (item.Value.Nav5 > 1)
                {
                    Console.WriteLine("收盘价买入、收盘价卖出 {0}日净值:{1},操作次数:{2}", item.Key, item.Value.Nav5, item.Value.Times);
                    Logger.Client.Debug(
                        string.Format("收盘价买入、收盘价卖出 {0}日净值:{1},操作次数:{2}", item.Key, item.Value.Nav5, item.Value.Times),
                        LogDomain.Ui);
                }

                //if (item.Value.Nav6 > 1)
                {
                    Console.WriteLine("最高价买入、收盘价卖出 {0}日净值:{1},操作次数:{2}", item.Key, item.Value.Nav6, item.Value.Times);
                    Logger.Client.Debug(
                        string.Format("最高价买入、收盘价卖出 {0}日净值:{1},操作次数:{2}", item.Key, item.Value.Nav6, item.Value.Times),
                        LogDomain.Ui);
                }

                //if (item.Value.Nav7 > 1)
                {
                    Console.WriteLine("最低价买入、最高价卖出 {0}日净值:{1},操作次数:{2}", item.Key, item.Value.Nav7, item.Value.Times);
                    Logger.Client.Debug(
                        string.Format("最低价买入、最高价卖出 {0}日净值:{1},操作次数:{2}", item.Key, item.Value.Nav7, item.Value.Times),
                        LogDomain.Ui);
                }

                //if (item.Value.Nav8 > 1)
                {
                    Console.WriteLine("收盘价买入、最高价卖出 {0}日净值:{1},操作次数:{2}", item.Key, item.Value.Nav8, item.Value.Times);
                    Logger.Client.Debug(
                        string.Format("收盘价买入、最高价卖出 {0}日净值:{1},操作次数:{2}", item.Key, item.Value.Nav8, item.Value.Times),
                        LogDomain.Ui);
                }

                //if (item.Value.Nav9 > 1)
                {
                    Console.WriteLine("最高价买入、最高价卖出 {0}日净值:{1},操作次数:{2}", item.Key, item.Value.Nav9, item.Value.Times);
                    Logger.Client.Debug(
                        string.Format("最高价买入、最高价卖出 {0}日净值:{1},操作次数:{2}", item.Key, item.Value.Nav9, item.Value.Times),
                        LogDomain.Ui);
                }
            }
        }
    }
}

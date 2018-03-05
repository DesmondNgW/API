using System;
using System.Collections.Generic;
using System.Linq;
using X.Util.Core.Log;
using X.Util.Entities.Enums;

namespace X.UI.Consoles.Stock
{
    //基本分析 => 确认热点题材以及龙头
    //技术分析 => 平均理念/摆荡理念/量能理念
    
    //趋势选股:
    
    //策略1:10日内出现涨停收盘, 5日真实波动率大于4%, 5日均线向上、股价位于5日均线上方, 回踩5日线位置（盘中计算5日线动态位置）时买入;次日高抛.
    //策略2:连续5日趋势向上, 5日真实波动率大于4%, 5日均线向上、股价位于5日均线上方, 回踩5日线位置（盘中计算5日线动态位置）时买入;次日高抛.
    //策略3:趋势向上, 热点题材突破甚至涨停。

    public class StockResult
    {
        public string StockCode { get; set; }

        public string StockName { get; set; }

        public double Nav1 { get; set; }
        public double Nav2 { get; set; }
        public double Nav3 { get; set; }
        public double Nav4 { get; set; }
        public double Nav5 { get; set; }
        public double Nav6 { get; set; }
        public double Nav7 { get; set; }
        public double Nav8 { get; set; }
        public double Nav9 { get; set; }

        public int Times { get; set; }

        public DateTime NextDateTime { get; set; }
    }

    public class StockTestHelper
    {
        public static void Test(string code, Func<Stock, bool> condition)
        {
            var result = new Dictionary<int, StockResult>();
            var g = StockDataHelper.StockData(code);
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

        public static void TestP(string code)
        {
            var g = StockDataHelper.StockData(code);
            var max = g.Max(p => p.K);
            var min = g.Min(p => p.K);
            var count = (max - min)*10;
            for (var i = 0; i < count; i++)
            {
                var st = min + i*0.1;
                var et = min + i*0.1 + 0.1;
                var r = g.Count(k => k.K >= st && k.K <= et);
                var p = (double)r/(double)g.Count;
                Console.WriteLine("{0}-{1}:{2}", st, et, p);
            }
        }


    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using X.Util.Core.Log;
using X.Util.Entities.Enums;

namespace X.UI.Consoles.Stock
{ 
    public class StockTestHelper
    {
        public static void Test(string code, Func<List<Stock>, List<Stock>> compute, Func<Stock, bool> condition)
        {
            var result1 = new Dictionary<int, double>();
            var result2 = new Dictionary<int, double>();
            var result3 = new Dictionary<int, double>();
            var result4 = new Dictionary<int, double>();
            var result5 = new Dictionary<int, double>();
            var result6 = new Dictionary<int, double>();
            var result7 = new Dictionary<int, double>();
            var result8 = new Dictionary<int, double>();
            var result9 = new Dictionary<int, double>();
            var g = compute(StockDataHelper.StockData(code));
            var ret = g.Where(condition).ToList();
            foreach (var t in ret)
            {
                for (var j = 1; j <= 20; j++)
                {
                    if (result1.ContainsKey(j))
                    {
                        result1[j] *= t.PriceL[j]/t.Low;
                    }
                    else
                    {
                        result1[j] = t.PriceL[j]/t.Low;
                    }

                    if (result2.ContainsKey(j))
                    {
                        result2[j] *= t.PriceL[j]/t.Close;
                    }
                    else
                    {
                        result2[j] = t.PriceL[j]/t.Close;
                    }

                    if (result3.ContainsKey(j))
                    {
                        result3[j] *= t.PriceL[j]/t.High;
                    }
                    else
                    {
                        result3[j] = t.PriceL[j]/t.High;
                    }

                    if (result4.ContainsKey(j))
                    {
                        result4[j] *= t.PriceC[j]/t.Low;
                    }
                    else
                    {
                        result4[j] = t.PriceC[j]/t.Low;
                    }

                    if (result5.ContainsKey(j))
                    {
                        result5[j] *= t.PriceC[j]/t.Close;
                    }
                    else
                    {
                        result5[j] = t.PriceC[j]/t.Close;
                    }

                    if (result6.ContainsKey(j))
                    {
                        result6[j] *= t.PriceC[j]/t.High;
                    }
                    else
                    {
                        result6[j] = t.PriceC[j]/t.High;
                    }

                    if (result7.ContainsKey(j))
                    {
                        result7[j] *= t.PriceH[j]/t.Low;
                    }
                    else
                    {
                        result7[j] = t.PriceH[j]/t.Low;
                    }

                    if (result8.ContainsKey(j))
                    {
                        result8[j] *= t.PriceH[j]/t.Close;
                    }
                    else
                    {
                        result8[j] = t.PriceH[j]/t.Close;
                    }

                    if (result9.ContainsKey(j))
                    {
                        result9[j] *= t.PriceH[j]/t.High;
                    }
                    else
                    {
                        result9[j] = t.PriceH[j]/t.High;
                    }

                }
            }
            foreach (var item in result1)
            {
                if (item.Value > 1)
                {
                    Console.WriteLine("最低价买入、最低价卖出 {0}日净值:{1},操作次数:{2}", item.Key, item.Value, ret.Count);
                    Logger.Client.Debug(
                        string.Format("最低价买入、最低价卖出 {0}日净值:{1},操作次数:{2}", item.Key, item.Value, result1.Count),
                        LogDomain.Ui);
                }
            }
            foreach (var item in result2)
            {
                if (item.Value > 1)
                {
                    Console.WriteLine("收盘价买入、最低价卖出 {0}日净值:{1},操作次数:{2}", item.Key, item.Value, ret.Count);
                    Logger.Client.Debug(
                        string.Format("收盘价买入、最低价卖出 {0}日净值:{1},操作次数:{2}", item.Key, item.Value, result2.Count),
                        LogDomain.Ui);
                }
            }
            foreach (var item in result3)
            {
                if (item.Value > 1)
                {
                    Console.WriteLine("最高价买入、最低价卖出 {0}日净值:{1},操作次数:{2}", item.Key, item.Value, ret.Count);
                    Logger.Client.Debug(
                        string.Format("最高价买入、最低价卖出 {0}日净值:{1},操作次数:{2}", item.Key, item.Value, result3.Count),
                        LogDomain.Ui);
                }
            }

            foreach (var item in result4)
            {
                if (item.Value > 1)
                {
                    Console.WriteLine("最低价买入、收盘价卖出 {0}日净值:{1},操作次数:{2}", item.Key, item.Value, ret.Count);
                    Logger.Client.Debug(
                        string.Format("最低价买入、收盘价卖出 {0}日净值:{1},操作次数:{2}", item.Key, item.Value, result4.Count),
                        LogDomain.Ui);
                }
            }
            foreach (var item in result5)
            {
                if (item.Value > 1)
                {
                    Console.WriteLine("收盘价买入、收盘价卖出 {0}日净值:{1},操作次数:{2}", item.Key, item.Value, ret.Count);
                    Logger.Client.Debug(
                        string.Format("收盘价买入、收盘价卖出 {0}日净值:{1},操作次数:{2}", item.Key, item.Value, result5.Count),
                        LogDomain.Ui);
                }
            }
            foreach (var item in result6)
            {
                if (item.Value > 1)
                {
                    Console.WriteLine("最高价买入、收盘价卖出 {0}日净值:{1},操作次数:{2}", item.Key, item.Value, ret.Count);
                    Logger.Client.Debug(
                        string.Format("最高价买入、收盘价卖出 {0}日净值:{1},操作次数:{2}", item.Key, item.Value, result6.Count),
                        LogDomain.Ui);
                }
            }

            foreach (var item in result7)
            {
                if (item.Value > 1)
                {
                    Console.WriteLine("最低价买入、最高价卖出 {0}日净值:{1},操作次数:{2}", item.Key, item.Value, ret.Count);
                    Logger.Client.Debug(
                        string.Format("最低价买入、最高价卖出 {0}日净值:{1},操作次数:{2}", item.Key, item.Value, result7.Count),
                        LogDomain.Ui);
                }
            }
            foreach (var item in result8)
            {
                if (item.Value > 1)
                {
                    Console.WriteLine("收盘价买入、最高价卖出 {0}日净值:{1},操作次数:{2}", item.Key, item.Value, ret.Count);
                    Logger.Client.Debug(
                        string.Format("收盘价买入、最高价卖出 {0}日净值:{1},操作次数:{2}", item.Key, item.Value, result8.Count),
                        LogDomain.Ui);
                }
            }
            foreach (var item in result9)
            {
                if (item.Value > 1)
                {
                    Console.WriteLine("最高价买入、最高价卖出 {0}日净值:{1},操作次数:{2}", item.Key, item.Value, ret.Count);
                    Logger.Client.Debug(
                        string.Format("最高价买入、最高价卖出 {0}日净值:{1},操作次数:{2}", item.Key, item.Value, result9.Count),
                        LogDomain.Ui);
                }
            }
        }

        public static List<Stock> Compute1(List<Stock> data)
        {
            for (var i = 0; i < data.Count; i++)
            {
                if (i > 0)
                {
                    data[i].Compute1 = data[i].High/data[i - 1].High;
                    data[i].Compute1 *= data[i].Open/data[i - 1].Close;
                    data[i].Compute1 *= data[i].Close/data[i - 1].Close;
                    data[i].Compute1 *= data[i].Close/data[i - 1].Close;
                    data[i].Compute1 *= data[i].Low/data[i - 1].Low;
                    data[i].Compute1 *= data[i].Close/data[i - 1].High;
                    data[i].Compute1 *= 2*data[i].Close/(data[i].High + data[i].Low);
                }
            }
            for (var i = 0; i < data.Count; i++)
            {
                if (i > 1)
                {
                    data[i].Compute2 = data[i].Compute1*data[i - 1].Compute1*data[i - 2].Compute1;
                }
                var j = i;
                while (j > 0 && data[j - 1].Compute1 >= 1.33)
                {
                    data[i].Compute3++;
                    j--;
                }
                var k = i;
                while (k > 0 && data[k - 1].Compute2 >= 1)
                {
                    data[i].Compute4++;
                    k--;
                }
            }
            return data;
        }
    }
}

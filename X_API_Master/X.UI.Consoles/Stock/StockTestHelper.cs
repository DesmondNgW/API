using System;
using System.Collections.Generic;
using System.Linq;
using X.Util.Core.Log;
using X.Util.Entities.Enums;

namespace X.UI.Consoles.Stock
{

/* 接力模式
N>2:{
S1:=HIGH/REF(HIGH,1)*100;
S2:=OPEN/REF(CLOSE,1)*100;
S3:=CLOSE/REF(CLOSE,1)*100;
S4:=CLOSE*2/(HIGH+LOW)*100;
S5:=CLOSE/OPEN*100;
S6:=LOW/REF(LOW,1)*100;
S7:=CLOSE/REF(HIGH,1)*100;
S:S1*S2*S3*S4*S5*S6*S7/100/100/100/100/100/100/100;
N:BARSLASTCOUNT(S>=1.3);
}

N>2:{
R:(C-O)/(H-L);
N:BARSLASTCOUNT(R>0.8);
}

N>2|N>1:{
DR:CLOSE/REF(CLOSE, 1)*100-100;
N:BARSLASTCOUNT(DR>9.8);
}

*/







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
                    data[i].Compute1 = (data[i].Close - data[i].Open) / (data[i].High - data[i].Low);
                }
            }
            for (var i = 0; i < data.Count; i++)
            {
                if (i > 0) data[i].Compute2 = data[i - 1].Compute1;
                if (i > 1) data[i].Compute3 = data[i - 2].Compute1;
                if (i > 2) data[i].Compute4 = data[i - 3].Compute1;
                if (i > 3) data[i].Compute5 = data[i - 4].Compute1;
            }
            return data;
        }
    }
}

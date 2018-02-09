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




/*
    
S:HIGH/REF(HIGH,1)*
    OPEN/REF(CLOSE,1)*
    CLOSE/REF(CLOSE,1)*
    CLOSE*2/(HIGH+LOW)*
    CLOSE/OPEN*
    LOW/REF(LOW,1)*
    CLOSE/REF(HIGH,1);
    
MAC:AMOUNT/VOL/100;
    
MAS:HIGH/REF(HIGH,1)*
    OPEN/REF(CLOSE,1)*
    MAC/REF(CLOSE,1)*
    MAC*2/(HIGH+LOW)*
    MAC/OPEN*
    LOW/REF(LOW,1)*
    MAC/REF(HIGH,1);
    
NS:BARSLASTCOUNT(S>=1.3);
NMAS:BARSLASTCOUNT(MAS>=1.3);
N1:IF(MAS>=1.3,REF(NS,1)+1,0);

R:(C-O)/(H-L);
MAR:(MAC-O)/(H-L);
    
NR:BARSLASTCOUNT(R>=0.8);
NMAR:BARSLASTCOUNT(MAR>=0.8);
N2:IF(MAR>=0.8,REF(NR,1)+1,0);
    
DR:CLOSE/REF(CLOSE, 1)*100-100;
N3:REF(BARSLASTCOUNT(DR>=9.8),1);

K1:IF(NS>2,1,0)+IF(NR>2,1,0);
K2:IF(NMAS>2,1,0)+IF(N1>2,1,0)+IF(NMAR>2,1,0)+IF(N2>2,1,0);
*/










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
    }
}

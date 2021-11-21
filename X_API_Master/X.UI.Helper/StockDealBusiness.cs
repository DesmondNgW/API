using System;
using System.Collections.Generic;
using System.Linq;
using X.UI.Entities;

namespace X.UI.Helper
{
    public class StockDealBusiness
    {
        #region 复盘逻辑
        /// <summary>
        /// 前排股票
        /// </summary>
        /// <param name="list"></param>
        /// <param name="top"></param>
        /// <param name="f"></param>
        /// <returns></returns>
        public static string[] GetStockName(IEnumerable<MyStock> list, int top, Func<MyStock, bool> f, Func<MyStock, string> o)
        {
            var a1 = list.Where(f).OrderByDescending(p => p.S1).Take(top);
            var a2 = list.Where(f).OrderByDescending(p => p.S2).Take(top);
            var a3 = list.Where(f).OrderByDescending(p => p.S3).Take(top);
            var a4 = list.Where(f).OrderByDescending(p => p.S4).Take(top);
            var b1 = a1.Union(a2);
            var b2 = a3.Union(a4);
            var b = b1.Where(p => b2.Count(q => q.Code == p.Code) > 0).Distinct();
            var ret = new List<string>() { StockDealBase.Calc(b).ToString("0.00") };
            return ret.Union(b.OrderByDescending(p => p.K1 + p.K2 + p.K3 + p.K4).Select(o)).ToArray();
        }

        /// <summary>
        /// GetS
        /// </summary>
        /// <param name="list"></param>
        /// <param name="top"></param>
        /// <returns></returns>
        public static IEnumerable<MyStock> GetS(List<MyStock> list, int top)
        {
            var list1 = list.Where(StockDealBase.F1).OrderByDescending(p => p.S1).Take(top);
            var list2 = list.Where(StockDealBase.F1).OrderByDescending(p => p.S2).Take(top);
            var list3 = list.Where(StockDealBase.F1).OrderByDescending(p => p.S3).Take(top);
            var list4 = list.Where(StockDealBase.F1).OrderByDescending(p => p.S4).Take(top);
            return list1.Union(list2).Union(list3).Union(list4).Distinct();
        }
        #endregion

        #region 复盘数据加工
        /// <summary>
        /// 对数据进行模式自动分类
        /// </summary>
        /// <param name="ddxList"></param>
        /// <param name="JX"></param>
        /// <param name="Core"></param>
        /// <returns></returns>
        public static Dictionary<string, ModeCompare> GetModeCompareAuto(List<StockCompare> ddxList, List<MyStock> JX, List<StockPrice> Core, string weight)
        {
            Dictionary<string, ModeCompare> ret = new Dictionary<string, ModeCompare>();
            foreach (var item in JX.Where(p => !p.Name.Contains(StockConstHelper.KZZ)))
            {
                if (item.SP > 0)
                {
                    string key = item.SP.ToString();
                    if (Core.Exists(p => p.StockCode == item.Code))
                    {
                        key = (item.SP + 3).ToString();
                    }
                    if (!ret.ContainsKey(key))
                    {
                        ret[key] = new ModeCompare()
                        {
                            CodeList = new List<StockCompare>()
                        };
                    }
                    var ddx = ddxList.FirstOrDefault(p => p.Code == item.Code);
                    if (ddx == null)
                    {
                        Console.WriteLine(item.Name + "找不到ddx");
                        continue;
                    }
                    ddx.Mode = key;
                    if (item.Amount <= 0) continue;
                    ddx.Amount = weight == StockConstHelper.AUTO ? (decimal)item.Amount : 1M;
                    ret[key].CodeList.Add(ddx);
                    ret[key].Name = key;
                }
            }
            return ret;
        }

        /// <summary>
        /// 对数据进行模式10cm或20cm分类
        /// </summary>
        /// <param name="ddxList"></param>
        /// <param name="JX"></param>
        /// <returns></returns>
        public static Dictionary<string, ModeCompare> GetModeCompareAutoByBk(IEnumerable<KeyValuePair<string, ModeCompare>> iRet)
        {
            var list = new List<StockCompare>();
            foreach (var item in iRet)
            {
                foreach (var cl in item.Value.CodeList)
                {
                    if (!list.Exists(p => p.Code == cl.Code))
                    {
                        list.Add(cl);
                    }
                }
            }

            Dictionary<string, ModeCompare> ret = new Dictionary<string, ModeCompare>();
            foreach (var item in list)
            {
                string key = item.Code.StartsWith("30") || item.Code.StartsWith("68") ? StockConstHelper.CYB
                    : StockConstHelper.ZB;
                if (!ret.ContainsKey(key))
                {
                    ret[key] = new ModeCompare()
                    {
                        CodeList = new List<StockCompare>()
                    };
                }

                item.Mode = key;
                ret[key].CodeList.Add(item);
                ret[key].Name = key;
            }
            return ret;
        }

        /// <summary>
        /// 对数据进行模式题材分类
        /// </summary>
        /// <param name="ddxList"></param>
        /// <param name="JX"></param>
        /// <param name="Des"></param>
        /// <returns></returns>
        public static Dictionary<string, ModeCompare> GetModeCompareAutoByBk(IEnumerable<KeyValuePair<string, ModeCompare>> iRet, List<StockDes> Des)
        {
            var list = new List<StockCompare>();
            foreach (var item in iRet)
            {
                foreach (var cl in item.Value.CodeList)
                {
                    if (!list.Exists(p => p.Code == cl.Code))
                    {
                        list.Add(cl);
                    }
                }
            }
            Dictionary<string, ModeCompare> ret = new Dictionary<string, ModeCompare>();
            foreach (var item in list)
            {
                var desItem = Des.FirstOrDefault(p => p.Code == item.Code);
                if (desItem != null && desItem.Bk != null)
                {
                    foreach (var bkItem in desItem.Bk)
                    {
                        string key = bkItem;
                        if (!ret.ContainsKey(key))
                        {
                            ret[key] = new ModeCompare()
                            {
                                CodeList = new List<StockCompare>()
                            };
                        }
                        item.Mode = key;
                        ret[key].CodeList.Add(item);
                        ret[key].Name = key;
                    }
                }
            }
            return ret;
        }

        /// <summary>
        /// 对外置模式数据统一格式
        /// </summary>
        /// <param name="ddxList"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        public static Dictionary<string, ModeCompare> GetModeCompare(List<StockCompare> ddxList, List<Tuple<string, string[]>> filter, List<MyStock> Kernel, string weight)
        {
            Dictionary<string, ModeCompare> ret = new Dictionary<string, ModeCompare>();
            for (var i = 0; i < filter.Count; i++)
            {
                if (filter[i].Item2.Length > 0)
                {
                    var mode = new ModeCompare()
                    {
                        CodeList = new List<StockCompare>()
                    };
                    foreach (var item in filter[i].Item2)
                    {
                        if (!string.IsNullOrEmpty(item))
                        {
                            var select = ddxList.FirstOrDefault(p => p.Name == item);
                            if (select == null)
                            {
                                select = ddxList.FirstOrDefault(p => p.Code == item);
                            }
                            var kernelItem = Kernel.FirstOrDefault(p => p.Code == select.Code);
                            select.Mode = filter[i].Item1;
                            select.Amount = weight == StockConstHelper.AUTO ? (decimal)kernelItem.Amount : 1M;
                            mode.CodeList.Add(select);
                            mode.Name = filter[i].Item1;
                        }
                    }
                    ret[filter[i].Item1] = mode;
                }
            }
            return ret;
        }
        #endregion
    }
}

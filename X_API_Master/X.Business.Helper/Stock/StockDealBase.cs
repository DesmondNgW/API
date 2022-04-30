using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using X.Business.Entities.Enums;
using X.Business.Entities.Stock;

namespace X.Business.Helper.Stock
{
    public class StockDealBase
    {
        #region 工具算法
        /// <summary>
        /// 非ST的有效股票
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public static bool F1(MyStock p)
        {
            return p.Inc > -100 && !p.Name.Contains(StockConstHelper.ST);
        }

        /// <summary>
        /// 非ST，中轨上方, 量价齐增的有效股票
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public static bool F3(MyStock p)
        {
            return p.Inc > -100 && !p.Name.Contains(StockConstHelper.ST) && (p.K1 + p.K2 + p.K3 + p.K4) / 4 > 7.5 && p.K4 > 0;
        }

        /// <summary>
        /// 非ST，成交在3.82亿以上的股票
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public static bool F4(MyStock p, double t)
        {
            return p.Inc > -100 && !p.Name.Contains(StockConstHelper.ST) && p.Amount >= t;// 3.82 * 1e8;
        }

        /// <summary>
        /// 输出代码和名称
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public static string O1(MyStock p)
        {
            return p.Code + " " + p.Name;
        }

        /// <summary>
        /// 输出代码、名称和量价排序
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public static string O2(MyStock p)
        {
            return p.Code + " " + p.Name + " " + (p.K1 + p.K2 + p.K3 + p.K4) / 4 + " " + (p.MyStockMode == MyStockMode.AQS ? StockConstHelper.AQS : StockConstHelper.WAVE);
        }

        /// <summary>
        /// 加权均值
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static double Calc(IEnumerable<MyStock> list)
        {
            double b = 0;
            double c = 0;
            foreach (var item in list)
            {
                b += item.Cap * item.LastClose;
                c += item.Cap * item.Close;
            }
            return c / b * 100 - 100;
        }

        /// <summary>
        /// GetAnswer
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static Tuple<List<int>, List<int>> GetAnswer(List<Tuple<double, double>> list)
        {
            var ret = new Tuple<List<int>, List<int>>(new List<int>(), new List<int>());
            if (list.Count >= 5)
            {
                for (var i = 2; i < list.Count; i++)
                {
                    //高低点
                    if ((list[i - 1].Item1 >= list[i - 2].Item1 && list[i - 1].Item1 >= list[i].Item1) ||
                        (list[i - 1].Item1 <= list[i - 2].Item1 && list[i - 1].Item1 <= list[i].Item1))
                    {
                        ret.Item1.Add(i * 25);
                    }
                    //高低点
                    if ((list[i - 1].Item2 >= list[i - 2].Item2 && list[i - 1].Item2 >= list[i].Item2) ||
                        (list[i - 1].Item2 <= list[i - 2].Item2 && list[i - 1].Item2 <= list[i].Item2))
                    {
                        ret.Item2.Add(i * 25);
                    }
                }
            }
            return ret;
        }

        /// <summary>
        /// 合并多个List
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static List<MyStock> Union(params List<MyStock>[] list)
        {
            var ret = new List<MyStock>();
            foreach (List<MyStock> item in list)
            {
                foreach (var current in item.Where(p => !ret.Exists(q => q.Code == p.Code)))
                {
                    ret.Add(current);
                }
            }
            return ret;
        }

        /// <summary>
        /// 正则分割字段
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static string[] SplitFiledByRegex(string item, int length, int start)
        {
            var list = Regex.Matches(item, StockConstHelper.REGEXSPACE).Cast<Match>().Select(p => p.Groups[0].Value).ToArray();
            if (list.Length == length) return list;
            var ret = new string[length];
            var tmp = list.Length - length;
            for (var i = 0; i < list.Length; i++)
            {
                if (i < start)
                {
                    ret[i] = list[i];
                }
                else if (i <= start + tmp)
                {
                    ret[start] += list[i];
                }
                else
                {
                    ret[i - tmp] = list[i];
                }
            }
            return ret.ToArray();
        }
        #endregion
    }
}

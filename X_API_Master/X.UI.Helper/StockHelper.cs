
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using X.UI.Entities;
using X.Util.Core;
using X.Util.Entities.Enums;
using X.Util.Other;

namespace X.UI.Helper
{
    public class StockHelper
    {
        public static StockMinute GetStockMinute(string code, DateTime dt)
        {
            var ret = new StockMinute();
            var minutes = StockDataHelper.GetMinuteData(code, dt);
            if (minutes != null && minutes.Count > 0)
            {
                ret.Id = string.Format("{0}-{1}", code, dt.ToString("yyyyMMdd"));
                ret.DateTime = dt;
                ret.CountOfDay = minutes.Count;
                ret.AmountOfDay = minutes.Sum(p => p.Amount);
                ret.VolOfDay = minutes.Sum(p => p.Vol);
                ret.PriceOfDay = ret.AmountOfDay / ret.VolOfDay;
                var high = minutes.Max(p => p.High);
                foreach (var minute in minutes)
                {
                    ret.StockCode = minute.StockCode;
                    ret.StockName = minute.StockName;
                    if (minute.Close == high && minute.High > minute.Low)
                    {
                        ret.CountOfHighPrice++;
                        ret.AmountOfHighPrice += minute.Amount;
                        ret.VolOfHighPrice += minute.Vol;
                    }
                    if (minute.Close == high && minute.High == minute.Low)
                    {
                        ret.CountOfOnePrice++;
                        ret.AmountOfOnePrice += minute.Amount;
                        ret.VolOfOnePrice += minute.Vol;
                    }
                }
                ret.PriceOfOnePrice = ret.VolOfOnePrice != 0 ? ret.AmountOfOnePrice / ret.VolOfOnePrice : 0;
                ret.PriceOfHighPrice = ret.VolOfHighPrice != 0 ? ret.AmountOfHighPrice / ret.VolOfHighPrice : 0;
                ret.AHP = ret.AmountOfHighPrice / ret.AmountOfDay * 100;
                ret.VHP = ret.VolOfHighPrice / ret.VolOfDay * 100;
                ret.AOP = ret.AmountOfOnePrice / ret.AmountOfDay * 100;
                ret.VOP = ret.VolOfOnePrice / ret.VolOfDay * 100;
            }
            return ret;
        }

        public static StockMinute GetStockMinute(string id, DateTime dt, List<StockKLine> minutes)
        {
            var ret = new StockMinute();
            if (minutes != null && minutes.Count > 0)
            {
                ret.Id = id;
                ret.DateTime = dt;
                ret.CountOfDay = minutes.Count;
                ret.AmountOfDay = minutes.Sum(p => p.Amount);
                ret.VolOfDay = minutes.Sum(p => p.Vol);
                ret.PriceOfDay = ret.AmountOfDay / ret.VolOfDay;
                var high = minutes.Max(p => p.High);
                foreach (var minute in minutes)
                {
                    ret.StockCode = minute.StockCode;
                    ret.StockName = minute.StockName;
                    if (minute.Close == high && minute.High > minute.Low)
                    {
                        ret.CountOfHighPrice++;
                        ret.AmountOfHighPrice += minute.Amount;
                        ret.VolOfHighPrice += minute.Vol;
                    }
                    if (minute.Close == high && minute.High == minute.Low)
                    {
                        ret.CountOfOnePrice++;
                        ret.AmountOfOnePrice += minute.Amount;
                        ret.VolOfOnePrice += minute.Vol;
                    }
                }
                ret.PriceOfOnePrice = ret.VolOfOnePrice != 0 ? ret.AmountOfOnePrice / ret.VolOfOnePrice : 0;
                ret.PriceOfHighPrice = ret.VolOfHighPrice != 0 ? ret.AmountOfHighPrice / ret.VolOfHighPrice : 0;
                ret.AHP = ret.AmountOfHighPrice / ret.AmountOfDay * 100;
                ret.VHP = ret.VolOfHighPrice / ret.VolOfDay * 100;
                ret.AOP = ret.AmountOfOnePrice / ret.AmountOfDay * 100;
                ret.VOP = ret.VolOfOnePrice / ret.VolOfDay * 100;
            }
            return ret;
        }
    }
}
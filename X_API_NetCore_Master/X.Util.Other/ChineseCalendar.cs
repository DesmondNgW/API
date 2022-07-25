using System;
using System.Linq;

namespace X.Util.Other
{
    /// <summary>
    /// 中国日历
    /// </summary>
    //-------------------------------------------------------------------------------
    //调用:
    //ChineseCalendar c = new ChineseCalendar(new DateTime(1990, 01, 15));
    //StringBuilder dayInfo = new StringBuilder();
    //dayInfo.Append("阳历：" + c.DateString + "\r\n");
    //dayInfo.Append("农历：" + c.ChineseDateString + "\r\n");
    //dayInfo.Append("星期：" + c.WeekDayStr);
    //dayInfo.Append("时辰：" + c.ChineseHour + "\r\n");
    //dayInfo.Append("属相：" + c.AnimalString + "\r\n");
    //dayInfo.Append("节气：" + c.ChineseTwentyFourDay + "\r\n");
    //dayInfo.Append("前一个节气：" + c.ChineseTwentyFourPrevDay + "\r\n");
    //dayInfo.Append("下一个节气：" + c.ChineseTwentyFourNextDay + "\r\n");
    //dayInfo.Append("节日：" + c.DateHoliday + "\r\n");
    //dayInfo.Append("干支：" + c.GanZhiDateString + "\r\n");
    //dayInfo.Append("星宿：" + c.ChineseConstellation + "\r\n");
    //dayInfo.Append("星座：" + c.Constellation + "\r\n");
    //-------------------------------------------------------------------------------
    public class ChineseCalendar
    {
        #region Inner Method
        /// <summary>
        /// 取农历年一年的天数
        /// </summary>
        private int GetChineseYearDays(int year)
        {
            int i = 0x8000, sumDay = 348; //29天*12个月
            var info = ChineseCalendarData.LunarDateArray[year - ChineseCalendarData.MinYear] & 0x0FFFF;
            //计算12个月中有多少天为30天
            for (int m = 0; m < 12; m++)
            {
                if ((info & i) != 0)
                {
                    sumDay++;
                }
                i >>= 1;
            }
            return sumDay + GetChineseLeapMonthDays(year);
        }

        /// <summary>
        /// 传回农历y年闰月的天数
        /// </summary>
        private int GetChineseLeapMonthDays(int year)
        {
            return GetChineseLeapMonth(year) != 0
                ? (ChineseCalendarData.LunarDateArray[year - ChineseCalendarData.MinYear] & 0x10000) != 0 ? 30 : 29
                : 0;
        }

        /// <summary>
        /// 传回农历 y年闰哪个月 1-12 , 没闰传回 0
        /// </summary>
        private int GetChineseLeapMonth(int year)
        {
            return ChineseCalendarData.LunarDateArray[year - ChineseCalendarData.MinYear] & 0xF;
        }

        /// <summary>
        /// 检查农历日期是否合理
        /// </summary>
        private void CheckChineseDateLimit(int year, int month, int day, bool leapMonth)
        {
            if ((year < ChineseCalendarData.MinYear) || (year > ChineseCalendarData.MaxYear) || (month < 1) || (month > 12) || (day < 1) || (day > 30))
            {
                throw new Exception("非法农历日期");
            }
            int leap = GetChineseLeapMonth(year);// 计算该年应该闰哪个月
            if ((leapMonth == true) && (month != leap))
            {
                throw new Exception("非法农历日期");
            }
        }

        /// <summary>
        /// //传回农历y年m月的总天数
        /// </summary>
        private int GetChineseMonthDays(int year, int month)
        {
            return BitTest32(ChineseCalendarData.LunarDateArray[year - ChineseCalendarData.MinYear] & 0x0000FFFF, 16 - month) ? 30 : 29;
        }

        /// <summary>
        /// 测试某位是否为真
        /// </summary>
        private bool BitTest32(int num, int bitpostion)
        {
            return (bitpostion > 31) || (bitpostion < 0)
                ? throw new Exception("Error Param: bitpostion[0-31]:" + bitpostion.ToString())
                : (num & 1 << bitpostion) != 0;
        }

        /// <summary>
        /// 获得当前时间的时辰
        /// </summary> 
        private string GetChineseHour(DateTime dt)
        {
            //计算时辰的地支
            var _hour = dt.Hour;
            if (dt.Minute != 0) _hour += 1;
            var offset = _hour / 2;
            if (offset >= 12) offset = 0;
            //计算天干
            var i = (Date - ChineseCalendarData.GanZhiStartDay).Days % 60;
            var indexGan = ((i % 10 + 1) * 2 - 1) % 10 - 1;
            var tmpGan = ChineseCalendarData.TianGan.Substring(indexGan) + ChineseCalendarData.TianGan.Substring(0, indexGan + 2);//凑齐12位
            return tmpGan[offset].ToString() + ChineseCalendarData.DiZhi[offset].ToString();
        }

        /// <summary>
        /// 将0-9转成汉字形式
        /// </summary>
        private string ConvertNumToChineseNum(char n)
        {
            return ChineseCalendarData.HZNum.ContainsKey(n) ? ChineseCalendarData.HZNum[n] : string.Empty;
        }

        /// <summary>
        /// 将星期几转成数字表示
        /// </summary>
        private int ConvertDayOfWeek(DayOfWeek dayOfWeek)
        {
            return dayOfWeek.GetHashCode() + 1;
        }

        /// <summary>
        /// 比较当天是不是指定的第周几
        /// </summary>
        private bool CompareWeekDayHoliday(DateTime date, int month, int week, int day)
        {
            var ret = false;
            if (date.Month == month && ConvertDayOfWeek(date.DayOfWeek) == day) //月份相同
            {
                var firstDay = new DateTime(date.Year, date.Month, 1);//生成当月第一天
                var i = ConvertDayOfWeek(firstDay.DayOfWeek);
                var firWeekDays = 7 - ConvertDayOfWeek(firstDay.DayOfWeek) + 1; //计算第一周剩余天数
                if (i > day)
                {
                    if ((week - 1) * 7 + day + firWeekDays == date.Day)
                    {
                        ret = true;
                    }
                }
                else
                {
                    if (day + firWeekDays + (week - 2) * 7 == date.Day)
                    {
                        ret = true;
                    }
                }
            }
            return ret;
        }

        #endregion

        #region 构造函数
        /// <summary>
        /// 用一个标准的公历日期来初使化
        /// </summary>
        public ChineseCalendar(DateTime dt)
        {
            if (dt < ChineseCalendarData.MinDay || dt > ChineseCalendarData.MaxDay)
            {
                throw new Exception("超出可转换的日期");
            }
            int i;
            Date = dt.Date;
            Datetime = dt;
            var temp = 0;
            var offset = (dt.Date - ChineseCalendarData.MinDay).Days;
            for (i = ChineseCalendarData.MinYear; i <= ChineseCalendarData.MaxYear; i++)
            {
                //求当年农历年天数
                temp = GetChineseYearDays(i);
                if (offset - temp < 1)
                    break;
                else
                {
                    offset -= temp;
                }
            }
            ChineseYear = i;
            //计算该年闰哪个月
            var leap = GetChineseLeapMonth(ChineseYear);
            //设定当年是否有闰月
            IsChineseLeapYear = leap > 0;
            IsChineseLeapMonth = false;
            for (i = 1; i <= 12; i++)
            {
                //闰月
                if ((leap > 0) && (i == leap + 1) && (IsChineseLeapMonth == false))
                {
                    IsChineseLeapMonth = true;
                    i--;
                    temp = GetChineseLeapMonthDays(ChineseYear); //计算闰月天数
                }
                else
                {
                    IsChineseLeapMonth = false;
                    temp = GetChineseMonthDays(ChineseYear, i);  //计算非闰月天数
                }
                offset -= temp;
                if (offset <= 0) break;
            }
            offset += temp;
            ChineseMonth = i;
            ChineseDay = offset;
        }

        /// <summary>
        /// 用农历的日期来初使化
        /// </summary>
        /// <param name="cy">农历年</param>
        /// <param name="cm">农历月</param>
        /// <param name="cd">农历日</param>
        /// <param name="LeapFlag">闰月标志</param>
        public ChineseCalendar(int cy, int cm, int cd, bool leapMonthFlag)
        {
            CheckChineseDateLimit(cy, cm, cd, leapMonthFlag);
            int i;
            ChineseYear = cy;
            ChineseMonth = cm;
            ChineseDay = cd;
            var offset = 0;
            for (i = ChineseCalendarData.MinYear; i < cy; i++)
            {
                offset += GetChineseYearDays(i);
            }
            //计算该年应该闰哪个月
            var leap = GetChineseLeapMonth(cy);
            IsChineseLeapYear = leap != 0;
            IsChineseLeapMonth = cm == leap && leapMonthFlag;
            //当年没有闰月||计算月份小于闰月
            if ((IsChineseLeapYear == false) || (cm < leap))
            {
                for (i = 1; i < cm; i++)
                {
                    offset += GetChineseMonthDays(cy, i);//计算非闰月天数
                }
                //检查日期是否大于最大天
                if (cd > GetChineseMonthDays(cy, cm))
                {
                    throw new Exception("不合法的农历日期");
                }
                //加上当月的天数
                offset += cd;
            }
            //是闰年，且计算月份大于或等于闰月
            else
            {
                for (i = 1; i < cm; i++)
                {
                    offset += GetChineseMonthDays(cy, i);
                }
                //计算月大于闰月
                if (cm > leap)
                {
                    offset += GetChineseLeapMonthDays(cy);
                    if (cd > GetChineseMonthDays(cy, cm))
                    {
                        throw new Exception("不合法的农历日期");
                    }
                    offset += cd;
                }
                //计算月等于闰月
                else
                {
                    //如果需要计算的是闰月，则应首先加上与闰月对应的普通月的天数
                    if (IsChineseLeapMonth)         //计算月为闰月
                    {
                        offset += GetChineseMonthDays(cy, cm);
                    }
                    if (cd > GetChineseLeapMonthDays(cy))
                    {
                        throw new Exception("不合法的农历日期");
                    }
                    offset += cd;
                }
            }
            Date = ChineseCalendarData.MinDay.AddDays(offset);
        }
        #endregion

        #region  公开属性
        /// <summary>
        /// 公历日期
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// 公历日期&时间
        /// </summary>
        public DateTime Datetime { get; set; }

        /// <summary>
        /// 农历年
        /// </summary>
        public int ChineseYear { get; set; }

        /// <summary>
        /// 农历月
        /// </summary>
        public int ChineseMonth { get; set; }

        /// <summary>
        /// 计算中国农历节日
        /// </summary>
        public string NewCalendarHoliday
        {
            get
            {
                var tempStr = string.Empty;
                if (IsChineseLeapMonth == false) //闰月不计算节日
                {
                    tempStr = (from lh in ChineseCalendarData.LHolidayInfo
                               where (lh.Month == ChineseMonth) && (lh.Day == ChineseDay)
                               select lh).FirstOrDefault().HolidayName ?? string.Empty;
                    //对除夕进行特别处理
                    if (ChineseMonth == 12)
                    {
                        var i = GetChineseMonthDays(ChineseYear, 12); //计算当年农历12月的总天数
                        if (ChineseDay == i) //如果为最后一天
                        {
                            tempStr = "除夕";
                        }
                    }
                }
                return tempStr;
            }
        }

        /// <summary>
        /// 按某月第几周第几日计算的节日
        /// </summary>
        public string WeekDayHoliday => (from wh in ChineseCalendarData.WHolidayInfo
                                         where CompareWeekDayHoliday(Date, wh.Month, wh.WeekAtMonth, wh.WeekDay)
                                         select wh).FirstOrDefault().HolidayName ?? string.Empty;

        /// <summary>
        /// 按公历日计算的节日
        /// </summary>
        public string DateHoliday => (from sh in ChineseCalendarData.SHolidayInfo
                                      where sh.Month == Date.Month && sh.Day == Date.Day
                                      select sh).FirstOrDefault().HolidayName ?? string.Empty;

        /// <summary>
        /// 取星期几
        /// </summary>
        public DayOfWeek WeekDay => Date.DayOfWeek;

        /// <summary>
        /// 周几的字符
        /// </summary>
        public string WeekDayStr => ChineseCalendarData.DayOfWeekDictionary[Date.DayOfWeek];

        /// <summary>
        /// 公历日期中文表示法 如一九九七年七月一日
        /// </summary>
        public string DateString => "公元" + Date.ToLongDateString();

        /// <summary>
        /// 当前是否公历闰年
        /// </summary>
        public bool IsLeapYear => DateTime.IsLeapYear(Date.Year);

        /// <summary>
        /// 28星宿计算
        /// </summary>
        public string ChineseConstellation
        {
            get
            {
                var offset = (Date - ChineseCalendarData.ChineseConstellationReferDay).Days;
                var modStarDay = offset % 28;
                return modStarDay >= 0 ? ChineseCalendarData.ChineseConstellationName[modStarDay] : ChineseCalendarData.ChineseConstellationName[27 + modStarDay];
            }
        }

        /// <summary>
        /// 时辰
        /// </summary>
        public string ChineseHour => GetChineseHour(Datetime);

        #region 农历日期
        /// <summary>
        /// 是否闰月
        /// </summary>
        public bool IsChineseLeapMonth { get; }

        /// <summary>
        /// 当年是否有闰月
        /// </summary>
        public bool IsChineseLeapYear { get; }

        /// <summary>
        /// 农历日
        /// </summary>
        public int ChineseDay { get; }

        /// <summary>
        /// 农历日中文表示
        /// </summary>
        public string ChineseDayString
        {
            get
            {
                switch (ChineseDay)
                {
                    case 0:
                        return string.Empty;
                    case 10:
                        return "初十";
                    case 20:
                        return "二十";
                    case 30:
                        return "三十";
                    default:
                        return ChineseCalendarData.Day_N_2[ChineseDay / 10].ToString() + ChineseCalendarData.Day_N_1[ChineseDay % 10].ToString();
                }
            }
        }

        /// <summary>
        /// 农历月份字符串
        /// </summary>
        public string ChineseMonthString => ChineseCalendarData.Months[ChineseMonth];

        /// <summary>
        /// 取农历年字符串如，一九九七年
        /// </summary>
        public string ChineseYearString
        {
            get
            {
                var tempStr = string.Empty;
                var num = ChineseYear.ToString();
                for (int i = 0; i < 4; i++)
                {
                    tempStr += ConvertNumToChineseNum(num[i]);
                }
                return tempStr + "年";
            }
        }

        /// <summary>
        /// 取农历日期表示法：农历一九九七年正月初五
        /// </summary>
        public string ChineseDateString => IsChineseLeapMonth == true
                    ? "农历" + ChineseYearString + "闰" + ChineseMonthString + ChineseDayString
                    : "农历" + ChineseYearString + ChineseMonthString + ChineseDayString;

        /// <summary>
        /// 定气法计算二十四节气,二十四节气是按地球公转来计算的，并非是阴历计算的
        /// </summary>
        /// <remarks>
        /// 节气的定法有两种。古代历法采用的称为"恒气"，即按时间把一年等分为24份，
        /// 每一节气平均得15天有余，所以又称"平气"。现代农历采用的称为"定气"，即
        /// 按地球在轨道上的位置为标准，一周360°，两节气之间相隔15°。由于冬至时地
        /// 球位于近日点附近，运动速度较快，因而太阳在黄道上移动15°的时间不到15天。
        /// 夏至前后的情况正好相反，太阳在黄道上移动较慢，一个节气达16天之多。采用
        /// 定气时可以保证春、秋两分必然在昼夜平分的那两天。
        /// </remarks>
        public string ChineseTwentyFourDay
        {
            get
            {
                DateTime baseDateAndTime = new DateTime(1900, 1, 6, 2, 5, 0); //#1/6/1900 2:05:00 AM#
                var tempStr = string.Empty;
                var y = Date.Year;
                for (var i = 1; i <= 24; i++)
                {
                    var num = 525948.76 * (y - 1900) + ChineseCalendarData.StermInfo[i - 1];
                    var newDate = baseDateAndTime.AddMinutes(num);//按分钟计算
                    if (newDate.DayOfYear == Date.DayOfYear)
                    {
                        tempStr = ChineseCalendarData.SolarTerm[i - 1];
                        break;
                    }
                }
                return tempStr;
            }
        }

        //当前日期前一个最近节气
        public string ChineseTwentyFourPrevDay
        {
            get
            {
                DateTime baseDateAndTime = new DateTime(1900, 1, 6, 2, 5, 0); //#1/6/1900 2:05:00 AM#
                string tempStr = string.Empty;
                var y = Date.Year;
                for (int i = 24; i >= 1; i--)
                {
                    var num = 525948.76 * (y - 1900) + ChineseCalendarData.StermInfo[i - 1];
                    var newDate = baseDateAndTime.AddMinutes(num);//按分钟计算
                    if (newDate.DayOfYear < Date.DayOfYear)
                    {
                        tempStr = string.Format("{0}[{1}]", ChineseCalendarData.SolarTerm[i - 1], newDate.ToString("yyyy-MM-dd"));
                        break;
                    }
                }
                return tempStr;
            }
        }

        //当前日期后一个最近节气
        public string ChineseTwentyFourNextDay
        {
            get
            {
                DateTime baseDateAndTime = new DateTime(1900, 1, 6, 2, 5, 0); //#1/6/1900 2:05:00 AM#
                string tempStr = string.Empty;
                var y = Date.Year;
                for (int i = 1; i <= 24; i++)
                {
                    var num = 525948.76 * (y - 1900) + ChineseCalendarData.StermInfo[i - 1];
                    var newDate = baseDateAndTime.AddMinutes(num);//按分钟计算
                    if (newDate.DayOfYear > Date.DayOfYear)
                    {
                        tempStr = string.Format("{0}[{1}]", ChineseCalendarData.SolarTerm[i - 1], newDate.ToString("yyyy-MM-dd"));
                        break;
                    }
                }
                return tempStr;
            }
        }
        #endregion

        /// <summary>
        /// 计算指定日期的星座序号 
        /// </summary>
        public string Constellation
        {
            get
            {
                var m = Date.Month;
                var d = Date.Day;
                var y = m * 100 + d;
                int index;
                if ((y >= 321) && (y <= 419)) { index = 0; }
                else if ((y >= 420) && (y <= 520)) { index = 1; }
                else if ((y >= 521) && (y <= 620)) { index = 2; }
                else if ((y >= 621) && (y <= 722)) { index = 3; }
                else if ((y >= 723) && (y <= 822)) { index = 4; }
                else if ((y >= 823) && (y <= 922)) { index = 5; }
                else if ((y >= 923) && (y <= 1022)) { index = 6; }
                else if ((y >= 1023) && (y <= 1121)) { index = 7; }
                else if ((y >= 1122) && (y <= 1221)) { index = 8; }
                else if ((y >= 1222) || (y <= 119)) { index = 9; }
                else if ((y >= 120) && (y <= 218)) { index = 10; }
                else if ((y >= 219) && (y <= 320)) { index = 11; }
                else { index = 0; }
                return ChineseCalendarData.ConstellationName[index];
            }
        }

        /// <summary>
        /// 计算属相的索引，注意虽然属相是以农历年来区别的，但是目前在实际使用中是按公历来计算的
        /// 鼠年为1,其它类推
        /// </summary>
        public int Animal => ((Date.Year - ChineseCalendarData.AnimalStartYear) % 12) + 1;

        /// <summary>
        /// 取属相字符串
        /// </summary>
        public string AnimalString => ChineseCalendarData.Animal[(Date.Year - ChineseCalendarData.AnimalStartYear) % 12].ToString();

        /// <summary>
        /// 取农历年的干支表示法如 乙丑年
        /// </summary>
        public string GanZhiYearString
        {
            get
            {
                int i = (ChineseYear - ChineseCalendarData.GanZhiStartYear) % 60; //计算干支
                return ChineseCalendarData.TianGan[i % 10].ToString() + ChineseCalendarData.DiZhi[i % 12].ToString() + "年";
            }
        }

        /// <summary>
        /// 取干支的月表示字符串，注意农历的闰月不记干支
        /// </summary>
        public string GanZhiMonthString
        {
            get
            {
                //每个月的地支总是固定的,而且总是从寅月开始
                var zhiIndex = ChineseMonth > 10 ? ChineseMonth - 10 : ChineseMonth + 2;
                var zhi = ChineseCalendarData.DiZhi[zhiIndex - 1].ToString();
                //根据当年的干支年的干来计算月干的第一个
                var ganIndex = 1;
                var i = (ChineseYear - ChineseCalendarData.GanZhiStartYear) % 60; //计算干支
                switch (i % 10)
                {
                    case 0: //甲
                        ganIndex = 3;
                        break;
                    case 1: //乙
                        ganIndex = 5;
                        break;
                    case 2: //丙
                        ganIndex = 7;
                        break;
                    case 3: //丁
                        ganIndex = 9;
                        break;
                    case 4: //戊
                        ganIndex = 1;
                        break;
                    case 5: //己
                        ganIndex = 3;
                        break;
                    case 6: //庚
                        ganIndex = 5;
                        break;
                    case 7: //辛
                        ganIndex = 7;
                        break;
                    case 8: //壬
                        ganIndex = 9;
                        break;
                    case 9: //癸
                        ganIndex = 1;
                        break;
                }
                string gan = ChineseCalendarData.TianGan[(ganIndex + ChineseMonth - 2) % 10].ToString();
                return gan + zhi + "月";
            }
        }

        /// <summary>
        /// 取干支日表示法
        /// </summary>
        public string GanZhiDayString
        {
            get
            {
                var offset = (Date - ChineseCalendarData.GanZhiStartDay).Days;
                var i = offset % 60;
                return ChineseCalendarData.TianGan[i % 10].ToString() + ChineseCalendarData.DiZhi[i % 12].ToString() + "日";
            }
        }

        /// <summary>
        /// 取当前日期的干支表示法如 甲子年乙丑月丙庚日
        /// </summary>
        public string GanZhiDateString => GanZhiYearString + GanZhiMonthString + GanZhiDayString;
        #endregion
    }
}

using System;
using X.Util.Entities;

namespace X.Util.Other
{
    /// <summary>
    /// 公历转农历
    /// </summary>
    public class CnDateHelper
    {
        #region 私有方法
        private static readonly long[] lunarInfo = new long[] { 0x04bd8, 0x04ae0, 0x0a570, 0x054d5, 0x0d260, 0x0d950, 0x16554,
                                                               0x056a0, 0x09ad0, 0x055d2, 0x04ae0, 0x0a5b6, 0x0a4d0, 0x0d250, 0x1d255, 0x0b540, 0x0d6a0, 0x0ada2, 0x095b0,
                                                               0x14977, 0x04970, 0x0a4b0, 0x0b4b5, 0x06a50, 0x06d40, 0x1ab54, 0x02b60, 0x09570, 0x052f2, 0x04970, 0x06566,
                                                               0x0d4a0, 0x0ea50, 0x06e95, 0x05ad0, 0x02b60, 0x186e3, 0x092e0, 0x1c8d7, 0x0c950, 0x0d4a0, 0x1d8a6, 0x0b550,
                                                               0x056a0, 0x1a5b4, 0x025d0, 0x092d0, 0x0d2b2, 0x0a950, 0x0b557, 0x06ca0, 0x0b550, 0x15355, 0x04da0, 0x0a5d0,
                                                               0x14573, 0x052d0, 0x0a9a8, 0x0e950, 0x06aa0, 0x0aea6, 0x0ab50, 0x04b60, 0x0aae4, 0x0a570, 0x05260, 0x0f263,
                                                               0x0d950, 0x05b57, 0x056a0, 0x096d0, 0x04dd5, 0x04ad0, 0x0a4d0, 0x0d4d4, 0x0d250, 0x0d558, 0x0b540, 0x0b5a0,
                                                               0x195a6, 0x095b0, 0x049b0, 0x0a974, 0x0a4b0, 0x0b27a, 0x06a50, 0x06d40, 0x0af46, 0x0ab60, 0x09570, 0x04af5,
                                                               0x04970, 0x064b0, 0x074a3, 0x0ea50, 0x06b58, 0x055c0, 0x0ab60, 0x096d5, 0x092e0, 0x0c960, 0x0d954, 0x0d4a0,
                                                               0x0da50, 0x07552, 0x056a0, 0x0abb7, 0x025d0, 0x092d0, 0x0cab5, 0x0a950, 0x0b4a0, 0x0baa4, 0x0ad50, 0x055d9,
                                                               0x04ba0, 0x0a5b0, 0x15176, 0x052b0, 0x0a930, 0x07954, 0x06aa0, 0x0ad50, 0x05b52, 0x04b60, 0x0a6e6, 0x0a4e0,
                                                               0x0d260, 0x0ea65, 0x0d530, 0x05aa0, 0x076a3, 0x096d0, 0x04bd7, 0x04ad0, 0x0a4d0, 0x1d0b6, 0x0d250, 0x0d520,
                                                               0x0dd45, 0x0b5a0, 0x056d0, 0x055b2, 0x049b0, 0x0a577, 0x0a4b0, 0x0aa50, 0x1b255, 0x06d20, 0x0ada0 };
        private static readonly string[] nStr1 = new string[] { "", "正", "二", "三", "四", "五", "六", "七", "八", "九", "十", "十一", "十二" };
        private static readonly string[] Gan = new string[] { "甲", "乙", "丙", "丁", "戊", "己", "庚", "辛", "壬", "癸" };
        private static readonly string[] Zhi = new string[] { "子", "丑", "寅", "卯", "辰", "巳", "午", "未", "申", "酉", "戌", "亥" };
        private static readonly string[] Animals = new string[] { "鼠", "牛", "虎", "兔", "龙", "蛇", "马", "羊", "猴", "鸡", "狗", "猪" };
        private static readonly string[] solarTerm = new string[] { "小寒", "大寒", "立春", "雨水", "惊蛰", "春分", "清明", "谷雨", "立夏", "小满", "芒种", "夏至", "小暑", "大暑", "立秋", "处暑", "白露", "秋分", "寒露", "霜降", "立冬", "小雪", "大雪", "冬至" };
        private static readonly int[] sTermInfo = { 0, 21208, 42467, 63836, 85337, 107014, 128867, 150921, 173149, 195551, 218072, 240693, 263343, 285989, 308563, 331033, 353350, 375494, 397447, 419210, 440795, 462224, 483532, 504758 };
        private static readonly string[] lFtv = new string[] { "0101农历春节", "0202 龙抬头节", "0115 元宵节", "0505 端午节", "0707 七夕情人节", "0815 中秋节", "0909 重阳节", "1208 腊八节", "1114 李君先生生日", "1224 小年", "0100除夕" };
        private static readonly string[] sFtv = new string[] { "0101 新年元旦",
                                                         "0202 世界湿地日",
                                                         "0207 国际声援南非日",
                                                         "0210 国际气象节",
                                                         "0214 情人节",
                                                         "0301 国际海豹日",
                                                         "0303 全国爱耳日",
                                                         "0308 国际妇女节",
                                                         "0312 植树节 孙中山逝世纪念日",
                                                         "0314 国际警察日",
                                                         "0315 国际消费者权益日",
                                                         "0317 中国国医节 国际航海日",
                                                         "0321 世界森林日 消除种族歧视国际日",
                                                         "0321 世界儿歌日",
                                                         "0322 世界水日",
                                                         "0323 世界气象日",
                                                         "0324 世界防治结核病日",
                                                         "0325 全国中小学生安全教育日",
                                                         "0330 巴勒斯坦国土日",
                                                         "0401 愚人节 全国爱国卫生运动月(四月) 税收宣传月(四月)",
                                                         "0407 世界卫生日",
                                                         "0422 世界地球日",
                                                         "0423 世界图书和版权日",
                                                         "0424 亚非新闻工作者日",
                                                         "0501 国际劳动节",
                                                         "0504 中国五四青年节",
                                                         "0505 碘缺乏病防治日",
                                                         "0508 世界红十字日",
                                                         "0512 国际护士节",
                                                         "0515 国际家庭日",
                                                         "0517 世界电信日",
                                                         "0518 国际博物馆日",
                                                         "0520 全国学生营养日",
                                                         "0523 国际牛奶日",
                                                         "0531 世界无烟日",
                                                         "0601 国际儿童节",
                                                         "0605 世界环境日",
                                                         "0606 全国爱眼日",
                                                         "0617 防治荒漠化和干旱日",
                                                         "0623 国际奥林匹克日",
                                                         "0625 全国土地日",
                                                         "0626 国际反毒品日",
                                                         "0701 中国共产党建党日 世界建筑日",
                                                         "0702 国际体育记者日",
                                                         "0707 中国人民抗日战争纪念日",
                                                         "0711 世界人口日",
                                                         "0730 非洲妇女日",
                                                         "0801 中国建军节",
                                                         "0808 中国男子节(爸爸节)",
                                                         "0815 日本正式宣布无条件投降日",
                                                         "0908 国际扫盲日 国际新闻工作者日",
                                                         "0910 教师节",
                                                         "0914 世界清洁地球日",
                                                         "0916 国际臭氧层保护日",
                                                         "0918 九·一八事变纪念日",
                                                         "0920 全国爱牙日",
                                                         "0927 世界旅游日",
                                                         "1001 国庆节 世界音乐日 国际老人节",
                                                         "1001 国际音乐日",
                                                         "1002 国际和平与民主自由斗争日",
                                                         "1004 世界动物日",
                                                         "1008 全国高血压日",
                                                         "1008 世界视觉日",
                                                         "1009 世界邮政日 万国邮联日",
                                                         "1010 辛亥革命纪念日 世界精神卫生日",
                                                         "1013 世界保健日 国际教师节",
                                                         "1014 世界标准日",
                                                         "1015 国际盲人节(白手杖节)",
                                                         "1016 世界粮食日",
                                                         "1017 世界消除贫困日",
                                                         "1022 世界传统医药日",
                                                         "1024 联合国日 世界发展信息日",
                                                         "1031 世界勤俭日",
                                                         "1107 十月社会主义革命纪念日",
                                                         "1108 中国记者日",
                                                         "1109 全国消防安全宣传教育日",
                                                         "1110 世界青年节",
                                                         "1111 国际科学与和平周(本日所属的一周)",
                                                         "1112 孙中山诞辰纪念日",
                                                         "1114 世界糖尿病日",
                                                         "1117 国际大学生节 世界学生节",
                                                         "1121 世界问候日 世界电视日",
                                                         "1129 国际声援巴勒斯坦人民国际日",
                                                         "1201 世界艾滋病日",
                                                         "1203 世界残疾人日",
                                                         "1205 国际经济和社会发展志愿人员日",
                                                         "1208 国际儿童电视日",
                                                         "1209 世界足球日",
                                                         "1210 世界人权日",
                                                         "1212 西安事变纪念日",
                                                         "1213 南京大屠杀(1937年)纪念日！紧记血泪史！",
                                                         "1221 国际篮球日",
                                                         "1224 平安夜",
                                                         "1225 圣诞节",
                                                         "1226 毛主席诞辰",
                                                         "1229 国际生物多样性日" };


        /// <summary>
        /// 传回农历y年的总天数
        /// </summary>
        private static int LYearDays(int y)
        {
            int i, sum = 348;
            for (i = 0x8000; i > 0x8; i >>= 1)
            {
                if ((lunarInfo[y - 1900] & i) != 0)
                    sum += 1;
            }
            return (sum + LeapDays(y));
        }

        /// <summary>
        /// 传回农历y年闰月的天数
        /// </summary>
        private static int LeapDays(int y)
        {
            return LeapMonth(y) != 0 ? (lunarInfo[y - 1900] & 0x10000) != 0 ? 30 : 29 : 0;
        }

        /// <summary>
        /// 传回农历y年闰哪个月 1-12 , 没闰传回 0
        /// </summary>
        private static int LeapMonth(int y)
        {
            return (int)(lunarInfo[y - 1900] & 0xf);
        }

        /// <summary>
        /// 传回农历y年m月的总天数
        /// </summary>
        private static int MonthDays(int y, int m)
        {
            return (lunarInfo[y - 1900] & (0x10000 >> m)) == 0 ? 29 : 30;
        }

        /// <summary>
        /// 传回农历y年的生肖
        /// </summary>
        private static string AnimalsYear(int y)
        {
            return Animals[(y - 4) % 12];
        }

        /// <summary>
        /// 传入月日的offset 传回干支,0=甲子
        /// </summary>
        private static string Cyclicalm(int num)
        {
            return (Gan[num % 10] + Zhi[num % 12]);
        }

        /// <summary>
        /// 传入offset 传回干支, 0=甲子
        /// </summary>
        private static string Cyclical(int y)
        {
            int num = y - 1900 + 36;
            return (Cyclicalm(num));
        }

        /// <summary>
        /// 传出y年m月d日对应的农历.year0 .month1 .day2 .yearCyl3 .monCyl4 .dayCyl5 .isLeap6
        /// </summary>
        private static long[] CalElement(int y, int m, int d)
        {
            long[] nongDate = new long[7];
            int temp = 0;
            var baseDate = new DateTime(1900, 1, 31);
            var objDate = new DateTime(y, m, d);
            var ts = objDate - baseDate;
            long offset = (long)ts.TotalDays;
            nongDate[5] = offset + 40;
            nongDate[4] = 14;
            int i;
            for (i = 1900; i < 2050 && offset > 0; i++)
            {
                temp = LYearDays(i);
                offset -= temp;
                nongDate[4] += 12;
            }
            if (offset < 0)
            {
                offset += temp;
                i--;
                nongDate[4] -= 12;
            }
            nongDate[0] = i;
            nongDate[3] = i - 1864;
            int leap = LeapMonth(i);
            nongDate[6] = 0;

            for (i = 1; i < 13 && offset > 0; i++)
            {
                // 闰月
                if (leap > 0 && i == (leap + 1) && nongDate[6] == 0)
                {
                    --i;
                    nongDate[6] = 1;
                    temp = LeapDays((int)nongDate[0]);
                }
                else
                {
                    temp = MonthDays((int)nongDate[0], i);
                }

                // 解除闰月
                if (nongDate[6] == 1 && i == (leap + 1))
                    nongDate[6] = 0;
                offset -= temp;
                if (nongDate[6] == 0)
                    nongDate[4]++;
            }

            if (offset == 0 && leap > 0 && i == leap + 1)
            {
                if (nongDate[6] == 1)
                {
                    nongDate[6] = 0;
                }
                else
                {
                    nongDate[6] = 1;
                    --i;
                    --nongDate[4];
                }
            }
            if (offset < 0)
            {
                offset += temp;
                --i;
                --nongDate[4];
            }
            nongDate[1] = i;
            nongDate[2] = offset + 1;
            return nongDate;
        }

        private static string GetChinaDate(int day)
        {
            string a = "";
            if (day == 10)
                return "初十";
            if (day == 20)
                return "二十";
            if (day == 30)
                return "三十";
            int two = day / 10;
            if (two == 0)
                a = "初";
            if (two == 1)
                a = "十";
            if (two == 2)
                a = "廿";
            if (two == 3)
                a = "三";
            int one = day % 10;
            switch (one)
            {
                case 1:
                    a += "一";
                    break;
                case 2:
                    a += "二";
                    break;
                case 3:
                    a += "三";
                    break;
                case 4:
                    a += "四";
                    break;
                case 5:
                    a += "五";
                    break;
                case 6:
                    a += "六";
                    break;
                case 7:
                    a += "七";
                    break;
                case 8:
                    a += "八";
                    break;
                case 9:
                    a += "九";
                    break;
            }
            return a;
        }

        private static DateTime STerm(int y, int n)
        {
            double ms = 31556925974.7 * (y - 1900);
            double ms1 = sTermInfo[n];
            return new DateTime(1900, 1, 6, 2, 5, 0).AddMilliseconds(ms).AddMinutes(ms1);
        }

        static string FormatDate(int m, int d)
        {
            return string.Format("{0:00}{1:00}", m, d);
        }
        #endregion

        #region 公有方法
        /// <summary>
        /// 传回公历y年m月的总天数
        /// </summary>
        public static int GetDaysByMonth(int y, int m)
        {
            int[] days = new int[] { 31, DateTime.IsLeapYear(y) ? 29 : 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };
            return days[m - 1];
        }

        /// <summary>
        /// 根据日期值获得周一的日期
        /// </summary>
        /// <param name="dt">输入日期</param>
        /// <returns>周一的日期</returns>
        public static DateTime GetMondayDateByDate(DateTime dt)
        {
            double d = 0;
            switch ((int)dt.DayOfWeek)
            {
                //case 1: d = 0; break;
                case 2: d = -1; break;
                case 3: d = -2; break;
                case 4: d = -3; break;
                case 5: d = -4; break;
                case 6: d = -5; break;
                case 0: d = -6; break;
            }
            return dt.AddDays(d);
        }

        /// <summary>
        /// 获取农历
        /// </summary>
        public static CnDateModel GetChinaDate(DateTime dt)
        {
            CnDateModel cd = new CnDateModel();
            int year = dt.Year;
            int month = dt.Month;
            int date = dt.Day;
            long[] l = CalElement(year, month, date);
            cd.cnIntYear = (int)l[0];
            cd.cnIntMonth = (int)l[1];
            cd.cnIntDay = (int)l[2];
            cd.cnStrYear = Cyclical(year);
            cd.cnAnm = AnimalsYear(year);
            cd.cnStrMonth = nStr1[(int)l[1]];
            cd.cnStrDay = GetChinaDate((int)(l[2]));
            string smd = dt.ToString("MMdd");

            string lmd = FormatDate(cd.cnIntMonth, cd.cnIntDay);
            for (int i = 0; i < solarTerm.Length; i++)
            {
                string s1 = STerm(dt.Year, i).ToString("MMdd");
                if (s1.Equals(dt.ToString("MMdd")))
                {
                    cd.cnSolarTerm = solarTerm[i];
                    break;
                }
            }
            foreach (string s in sFtv)
            {
                string s1 = s.Substring(0, 4);
                if (s1.Equals(smd))
                {
                    cd.cnFtvs = s.Substring(4, s.Length - 4);
                    break;
                }
            }
            foreach (string s in lFtv)
            {
                string s1 = s.Substring(0, 4);
                if (s1.Equals(lmd))
                {
                    cd.cnFtvl = s.Substring(4, s.Length - 4);
                    break;
                }
            }
            dt = dt.AddDays(1);
            year = dt.Year;
            month = dt.Month;
            date = dt.Day;
            l = CalElement(year, month, date);
            lmd = FormatDate((int)l[1], (int)l[2]);
            if (lmd.Equals("0101")) cd.cnFtvl = "除夕";
            return cd;
        }
        #endregion
    }
}

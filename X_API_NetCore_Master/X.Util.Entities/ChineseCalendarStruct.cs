namespace X.Util.Entities
{
    /// <summary>
    /// 阳历
    /// </summary>
    public struct SolarHolidayStruct
    {
        public SolarHolidayStruct(int month, int day, int recess, string name)
        {
            Month = month;
            Day = day;
            Recess = recess;
            HolidayName = name;
        }
        public int Month { get; set; }
        public int Day { get; set; }
        //假期长度
        public int Recess { get; set; }
        public string HolidayName { get; set; }
    }

    /// <summary>
    /// 农历
    /// </summary>
    public struct LunarHolidayStruct
    {
        public LunarHolidayStruct(int month, int day, int recess, string name)
        {
            Month = month;
            Day = day;
            Recess = recess;
            HolidayName = name;
        }
        public int Month { get; set; }
        public int Day { get; set; }
        public int Recess { get; set; }
        public string HolidayName { get; set; }
    }

    public struct WeekHolidayStruct
    {
        public WeekHolidayStruct(int month, int weekAtMonth, int weekDay, string name)
        {
            Month = month;
            WeekAtMonth = weekAtMonth;
            WeekDay = weekDay;
            HolidayName = name;
        }
        public int Month { get; set; }
        public int WeekAtMonth { get; set; }
        public int WeekDay { get; set; }
        public string HolidayName { get; set; }
    }
}

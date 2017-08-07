using System;

namespace X.Interface.Dto.HttpResponse
{
    /// <summary>
    /// 时间
    /// </summary>
    public class DateTimeDto
    {
        /// <summary>
        /// DateTime
        /// </summary>
        public DateTime DateTime { get; set; }

        /// <summary>
        /// WeekOfYear
        /// </summary>
        public int WeekOfYear { get; set; }

        /// <summary>
        /// DayOfYear
        /// </summary>
        public int DayOfYear { get; set; }
    }
}

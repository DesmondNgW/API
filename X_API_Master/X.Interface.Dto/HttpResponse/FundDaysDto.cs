using System;

namespace X.Interface.Dto.HttpResponse
{
    /// <summary>
    /// 基金确认到账日Dto
    /// </summary>
    public class FundDaysDto
    {
        /// <summary>
        /// 到账日
        /// </summary>
        public int CashDays { get; set; }

        /// <summary>
        /// 基金代码
        /// </summary>
        public string FundCode { get; set; }

        /// <summary>
        /// 是否删除
        /// </summary>
        public bool IsDel { get; set; }

        /// <summary>
        /// 认购确认日
        /// </summary>
        public int RgCfmDays { get; set; }

        /// <summary>
        /// 交易确认日
        /// </summary>
        public int TradeCfmDays { get; set; }

        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 是否可用
        /// </summary>
        public bool IsEnabled { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime UpdateTime { get; set; }
    }
}

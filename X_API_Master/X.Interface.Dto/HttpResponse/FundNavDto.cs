using System;

namespace X.Interface.Dto.HttpResponse
{
    /// <summary>
    /// 基金净值Dto
    /// </summary>
    public class FundNavDto
    {
        /// <summary>
        /// 7日年化
        /// </summary>
        public decimal Annual7D { get; set; }

        /// <summary>
        /// 基金代码
        /// </summary>
        public string FundCode { get; set; }

        /// <summary>
        /// 基金名称
        /// </summary>
        public string FundName { get; set; }

        /// <summary>
        /// 主键id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 净值
        /// </summary>
        public decimal Nav { get; set; }

        /// <summary>
        /// 净值日期
        /// </summary>
        public DateTime Navdate { get; set; }

        /// <summary>
        /// 万份收益
        /// </summary>
        public decimal UnitAccrual { get; set; }
    }
}

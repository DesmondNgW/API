using System;

namespace X.Interface.Dto.HttpResponse
{
    /// <summary>
    /// 基金对象Dto
    /// </summary>
    public class FundDto
    {
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

        /// <summary>
        /// 允许业务
        /// </summary>
        public string AllowBusin { get; set; }

        /// <summary>
        /// CanCashBuy
        /// </summary>
        public string CanCashBuy { get; set; }

        /// <summary>
        /// CanCashRedeem
        /// </summary>
        public string CanCashRedeem { get; set; }

        /// <summary>
        /// CashType
        /// </summary>
        public string CashType { get; set; }

        /// <summary>
        /// 收费类型
        /// </summary>
        public string ChargeType { get; set; }

        /// <summary>
        /// 收费类型名称
        /// </summary>
        public string ChargeTypeName { get; set; }

        /// <summary>
        /// 分红方式
        /// </summary>
        public string DefDividendMethod { get; set; }

        /// <summary>
        /// 折扣
        /// </summary>  
        public decimal Discount { get; set; }

        /// <summary>
        /// 短期理财基金周期
        /// </summary>  
        public int FinancialRound { get; set; }

        /// <summary>
        /// FinancialTip
        /// </summary>       
        public string FinancialTip { get; set; }

        /// <summary>
        /// 理财基金类型
        /// </summary>        
        public string FinancialType { get; set; }

        /// <summary>
        /// 基金代码
        /// </summary>
        public string FundCode { get; set; }

        /// <summary>
        /// 基金公司
        /// </summary>    
        public string FundCompany { get; set; }

        /// <summary>
        /// 基金公司id
        /// </summary>       
        public string FundCompanyNo { get; set; }

        /// <summary>
        /// 基金代码名称
        /// </summary>        
        public string FundName { get; set; }

        /// <summary>
        /// 基金风险等级
        /// </summary>       
        public string FundRisk { get; set; }

        /// <summary>
        /// 基金风险等级名称
        /// </summary>      
        public string FundRiskName { get; set; }

        /// <summary>
        /// 基金状态
        /// </summary>        
        public string FundState { get; set; }

        /// <summary>
        /// 基金状态名称
        /// </summary>        
        public string FundStateName { get; set; }

        /// <summary>
        /// 基金类型
        /// </summary>       
        public string FundType { get; set; }

        /// <summary>
        /// 基金类型名称
        /// </summary>       
        public string FundTypeName { get; set; }

        /// <summary>
        /// 认购基金IPO开始时间
        /// </summary>      
        public DateTime? IpoBeginDate { get; set; }

        /// <summary>
        /// 认购基金IPO结束时间
        /// </summary>        
        public DateTime? IpoEndDate { get; set; }

        /// <summary>
        /// MajorFundCode
        /// </summary>       
        public string MajorFundCode { get; set; }

        /// <summary>
        /// 最大赎回份额
        /// </summary>        
        public decimal? MaxRedeem { get; set; }

        /// <summary>
        /// 最大申购金额
        /// </summary>      
        public decimal? MaxSg { get; set; }

        /// <summary>
        /// 最小持仓份额
        /// </summary>       
        public decimal MinHoldShare { get; set; }

        /// <summary>
        /// 最小赎回份额
        /// </summary>       
        public decimal MinRedeem { get; set; }

        /// <summary>
        /// 最小认购追加金额
        /// </summary>       
        public decimal MinRgAppend { get; set; }

        /// <summary>
        /// 最小认购购买金额
        /// </summary>       
        public decimal MinRgInvested { get; set; }

        /// <summary>
        /// 最小申购追加金额
        /// </summary>        
        public decimal MinSgAppend { get; set; }

        /// <summary>
        /// 最小申购购买金额
        /// </summary>
        public decimal MinSgInvested { get; set; }

        /// <summary>
        /// 最小定投金额
        /// </summary>
        public decimal MinTimerSg { get; set; }

        /// <summary>
        /// 最小转换
        /// </summary>
        public decimal MinTrans { get; set; }

        /// <summary>
        /// MoneyTypeB
        /// </summary>
        public string MoneyTypeB { get; set; }

        /// <summary>
        /// 基金净值
        /// </summary>
        public decimal Nav { get; set; }

        /// <summary>
        /// 净值日期
        /// </summary>
        public DateTime NavDate { get; set; }

        /// <summary>
        /// 费率
        /// </summary>
        public string Rate { get; set; }

        /// <summary>
        /// ReachDay
        /// </summary>
        public int ReachDay { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public decimal RedeemLimits { get; set; }

        /// <summary>
        /// StablishDate
        /// </summary>
        public DateTime? StablishDate { get; set; }

        /// <summary>
        /// 可转换基金代码集合
        /// </summary>
        public string TransCodes { get; set; }
    }
}

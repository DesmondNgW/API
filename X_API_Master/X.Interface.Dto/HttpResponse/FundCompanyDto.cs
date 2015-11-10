namespace X.Interface.Dto.HttpResponse
{
    /// <summary>
    /// 基金公司对象Dto
    /// </summary>
    public class FundCompanyDto
    {
        /// <summary>
        /// 公司名称
        /// </summary>
        public string CompanyName { get; set; }

        /// <summary>
        /// 公司id
        /// </summary>
        public string CompanyNo { get; set; }

        /// <summary>
        /// ContactPerson
        /// </summary>
        public string ContactPerson { get; set; }

        /// <summary>
        /// ContractInfo
        /// </summary>
        public string ContractInfo { get; set; }

        /// <summary>
        /// MainOfficeAddress
        /// </summary>
        public string MainOfficeAddress { get; set; }

        /// <summary>
        /// PersonInCharge
        /// </summary>
        public string PersonInCharge { get; set; }

        /// <summary>
        /// RegisteredAddress
        /// </summary>
        public string RegisteredAddress { get; set; }
    }
}

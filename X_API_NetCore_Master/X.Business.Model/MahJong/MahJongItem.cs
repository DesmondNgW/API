namespace X.Business.Model.MahJong
{
    /// <summary>
    /// 麻将牌
    /// </summary>
    public class MahJongItem
    {
        /// <summary>
        /// 编码
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 类型
        /// </summary>
        public EnumMahJongType Type { get; set; }
    }
}

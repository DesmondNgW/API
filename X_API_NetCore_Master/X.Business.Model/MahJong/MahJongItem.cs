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

        /// <summary>
        /// 赤宝牌
        /// </summary>
        public bool IsRed { get; set; }

        /// <summary>
        /// 宝牌或杠宝牌
        /// </summary>
        public bool IsRewardOuter { get; set; }

        /// <summary>
        /// 里宝牌
        /// </summary>
        public bool IsRewardInner { get; set; }

        /// <summary>
        /// 宝牌个数
        /// </summary>
        public int RewardOuterCount { get; set; }

        /// <summary>
        /// 里宝牌个数
        /// </summary>
        public int RewardInnerCount { get; set; }

        /// <summary>
        /// 总宝牌个数
        /// </summary>
        public int RewardTotalCount
        {
            get
            {
                return RewardInnerCount + RewardOuterCount + (IsRed ? 1 : 0);
            }
        }

        /// <summary>
        /// 是否生效
        /// </summary>
        public bool Active { get; set; }
    }
}

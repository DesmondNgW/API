using System.Collections.Generic;

namespace X.Business.Model.MahJong
{
    public class MahJongModel
    {
        /// <summary>
        /// 牌山
        /// </summary>
        public List<MahJongItem> MahJongList { get; set; }

        /// <summary>
        /// 动态牌山
        /// </summary>
        public List<MahJongItem> DymicMahJongList { get; set; }

        /// <summary>
        /// 杠牌区
        /// </summary>
        public List<MahJongItem> MahJongGangList { get; set; }

        /// <summary>
        /// 宝牌区
        /// </summary>
        public List<MahJongItem> MahJongRewardList { get; set; }

    }
}

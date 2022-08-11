using System.Collections.Generic;

namespace X.Business.Model.MahJong
{
    /// <summary>
    /// 牌山
    /// </summary>
    public class MahJongModel : MahJongOption
    {
        /// <summary>
        /// 牌山
        /// </summary>
        public List<MahJongItem> MahJongList { get; set; }

        /// <summary>
        /// 动态牌山
        /// </summary>
        public List<MahJongItem> DymicMahJongList { get; set; }
    }
}

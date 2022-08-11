using System.Collections.Generic;

namespace X.Business.Model.MahJong
{
    /// <summary>
    /// 玩家
    /// </summary>
    public class MahJongPlayer
    {
        /// <summary>
        /// 手牌
        /// </summary>
        public List<MahJongItem> List { get; set; }

        /// <summary>
        /// 当前选手的类型
        /// </summary>
        public EnumOrderType CurrentPlayerType { get; set; }
    }
}

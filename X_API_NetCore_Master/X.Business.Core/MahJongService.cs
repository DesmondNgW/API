using System.Collections.Generic;
using X.Business.Model.MahJong;
using X.Util.Core.Common;

namespace X.Business.Core
{
    public class MahJongService
    {
        /// <summary>
        /// 初始化牌山
        /// </summary>
        /// <returns></returns>
        public static List<MahJongItem> Init()
        {
            var data = MahJongData.InitMahJongItems();
            var ret = new List<MahJongItem>();
            while (data.Count > 0)
            {
                var item = StringConvert.SysRandom.Next(0, data.Count - 1);
                ret.Add(data[item]);
                data.RemoveAt(item);
            }
            return ret;
        }

        /// <summary>
        /// 初始化模型
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static MahJongModel InitModel(List<MahJongItem> list)
        {
            var model = new MahJongModel
            {
                MahJongList = list,
                MahJongGangList = new List<MahJongItem>(),
                MahJongRewardList = new List<MahJongItem>(),
                DymicMahJongList = new List<MahJongItem>()
            };
            for (var i = 0; i < list.Count - 14; i++)
            {
                model.DymicMahJongList.Add(list[i]);
            }
            var count = 14;
            while (count > 0)
            {
                if (model.MahJongRewardList.Count < 10)
                {
                    if (count == 6 || count == 5)
                    {
                        list[list.Count - count].Active = true;
                    }
                    model.MahJongRewardList.Add(list[list.Count - count]);
                }
                else
                {
                    model.MahJongGangList.Add(list[list.Count - count]);
                }
                count--;
            }
            return model;
        }
    }
}

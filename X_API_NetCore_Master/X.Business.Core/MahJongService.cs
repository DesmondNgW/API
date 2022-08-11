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
                DymicMahJongList = new List<MahJongItem>()
            };
            for (var i = 0; i < list.Count - MahJongOption.UnGetItemCount; i++)
            {
                model.DymicMahJongList.Add(list[i]);
            }
            return model;
        }
    }
}

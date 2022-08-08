using System;
using System.Collections.Generic;
using X.Business.Model.MahJong;

namespace X.Business.Core
{
    public class MahJongData
    {
        public static int ItemCount = 4;
        public static int RedCount = 1;
        public static List<string> Numbers = new List<string>() { "1", "2", "3", "4", "5", "6", "7", "8", "9" };
        public static List<string> Others = new List<string>() { "东", "南", "西", "北", "中", "发", "白" };

        /// <summary>
        /// 初始化数据
        /// </summary>
        /// <returns></returns>
        public static List<MahJongItem> InitMahJongItems()
        {
            var ret = new List<MahJongItem>();
            foreach (var item in (EnumMahJongType[])Enum.GetValues(typeof(EnumMahJongType)))
            {
                List<string> target = item == EnumMahJongType.Z ? Others : Numbers;
                foreach (var number in target)
                {
                    for (var i = 0; i < ItemCount; i++)
                    {
                        var mahJongItem = new MahJongItem()
                        {
                            Name = number,
                            Code = Guid.NewGuid().ToString("N2"),
                            IsRed = i <= RedCount - 1 && number == "5",
                            Type = item,
                        };
                        ret.Add(mahJongItem);
                    }
                }
            }
            return ret;
        }
    }
}

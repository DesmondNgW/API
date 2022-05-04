using System;
using System.Collections.Generic;
using X.Business.Model.Enums;
using X.Util.Core;
using X.Util.Core.Kernel;

namespace X.Business.Util
{
    public class EnumZoneHelper
    {
        /// <summary>
        /// Zones
        /// </summary>
        public static List<string> Zones
        {
            get
            {
                var zones = new List<string>();
                for (var i = 1; i < Enum.GetValues(typeof(EnumZone)).Length; i++)
                {
                    zones.Add(i.ToString());
                }
                return zones;
            }
        }

        /// <summary>
        ///  token Store Zone
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public static int GetTokenZone(string token)
        {
            return CoreUtil.GetConsistentHash(Zones, ConstHelper.LoginKeyPrefix + token).Convert2Int32(1);
        }

        /// <summary>
        /// LoginStatus Store Zone
        /// </summary>
        /// <param name="token"></param>
        /// <param name="uid"></param>
        /// <returns></returns>
        public static int GetStatusZone(string token, string uid)
        {
            return CoreUtil.GetConsistentHash(Zones, ConstHelper.LoginKeyPrefix + token + uid).Convert2Int32(1);
        }
    }
}

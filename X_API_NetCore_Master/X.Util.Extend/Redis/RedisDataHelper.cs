using StackExchange.Redis;
using System.Collections.Generic;
using System.Linq;
using X.Util.Core;

namespace X.Util.Extend.Redis
{
    /// <summary>
    /// RedisValue 作为存储结果，以json格式存在 
    /// </summary>
    public class RedisDataHelper
    {

        public static RedisKey String2Key(string key)
        {
            return key;
        }

        public static RedisKey[] String2Key(List<string> keys)
        {
            if (keys != null && keys.Any())
            {
                return keys.Select(p => (RedisKey)p).ToArray();
            }
            return default;
        }

        public static RedisValue Type2Value<T>(T obj)
        {
            return obj.ToJson();
        }

        public static RedisValue String2Value(string value)
        {
            return value;
        }

        public static RedisValue[] String2Value(List<string> values)
        {
            if (values != null && values.Any())
            {
                return values.Select(p => (RedisValue)p).ToArray();
            }
            return default;
        }

        public static T Value2Type<T>(RedisValue value)
        {
            return ((string)value).FromJson<T>();
        }

        public static List<string> Value2String(RedisValue[] values)
        {
            if (values != null && values.Any())
            {
                return values.Select(p => (string)p).ToList();
            }
            return default;
        }
    }
}

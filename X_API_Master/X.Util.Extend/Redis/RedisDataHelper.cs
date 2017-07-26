using System.Collections.Generic;
using System.Linq;
using StackExchange.Redis;
using X.Util.Core;

namespace X.Util.Extend.Redis
{
    public class RedisDataHelper
    {
        public static RedisKey ConvertKey(string key)
        {
            return key;
        }

        public static RedisKey[] ConvertKey(List<string> keys)
        {
            if (keys != null && keys.Any())
            {
                return keys.Select(p => (RedisKey)p).ToArray();
            }
            return default(RedisKey[]);
        }

        public static RedisValue ConvertValue<T>(T obj)
        {
            return obj.ToJson();
        }

        public static RedisValue ConvertValue(string value)
        {
            return value;
        }

        public static RedisValue[] ConvertValue(List<string> values)
        {
            if (values != null && values.Any())
            {
                return values.Select(p => (RedisValue)p).ToArray();
            }
            return default(RedisValue[]);
        }

        public static T ConvertValue<T>(RedisValue value)
        {
            return ((string)value).FromJson<T>();
        }

        public static List<string> ConvertValue(RedisValue[] values)
        {
            if (values != null && values.Any())
            {
                return values.Select(p => (string)p).ToList();
            }
            return default(List<string>);
        }
    }
}

using StackExchange.Redis;
using System;
using System.Collections.Generic;

namespace X.Util.Entities.Interface
{
    /// <summary>
    /// Redis Base Interface
    /// </summary>
    public interface IRedisBase
    {
        bool KeyExists(string key, CommandFlags flags = CommandFlags.None);

        bool KeyExpire(string key, DateTime? expire, CommandFlags flags = CommandFlags.None);

        bool KeyExpire(string key, TimeSpan? expire, CommandFlags flags = CommandFlags.None);

        bool KeyDelete(string key, CommandFlags flags = CommandFlags.None);

        long KeyDelete(List<string> keys, CommandFlags flags = CommandFlags.None);

        bool KeyMove(string key, int database, CommandFlags flags = CommandFlags.None);

        bool KeyPersist(string key, CommandFlags flags = CommandFlags.None);

        string KeyRandom(CommandFlags flags = CommandFlags.None);

        bool KeyRename(string key, string newKey, When when = When.Always, CommandFlags flags = CommandFlags.None);

        RedisType KeyType(string key, CommandFlags flags = CommandFlags.None);

        TimeSpan? KeyTimeToLive(string key, CommandFlags flags = CommandFlags.None);

        T StringGet<T>(string key, CommandFlags flags = CommandFlags.None);

        RedisValue[] StringGet(List<string> keys, CommandFlags flags = CommandFlags.None);

        bool StringSet<T>(string key, T value, TimeSpan? expire, When when = When.Always,
            CommandFlags flags = CommandFlags.None);

        bool StringSet(Dictionary<RedisKey, RedisValue> values, When when = When.Always,
            CommandFlags flags = CommandFlags.None);

        double StringIncrement(string key, double value, CommandFlags flags = CommandFlags.None);

        double StringDecrement(string key, double value, CommandFlags flags = CommandFlags.None);

        bool KeyExists(string key, string hashField, CommandFlags flags = CommandFlags.None);

        long HashDecrement(string key, string hashField, long value = 1, CommandFlags flags = CommandFlags.None);

        double HashDecrement(string key, string hashField, double value, CommandFlags flags = CommandFlags.None);

        bool HashDelete(string key, string hashField, CommandFlags flags = CommandFlags.None);

        long HashDelete(string key, List<string> hashFields, CommandFlags flags = CommandFlags.None);

        T HashGet<T>(string key, string hashField, CommandFlags flags = CommandFlags.None);

        RedisValue[] HashGet(string key, List<string> hashFields, CommandFlags flags = CommandFlags.None);

        HashEntry[] HashGetAll(RedisKey key, CommandFlags flags = CommandFlags.None);

        long HashIncrement(string key, string hashField, long value = 1, CommandFlags flags = CommandFlags.None);

        double HashIncrement(string key, string hashField, double value, CommandFlags flags = CommandFlags.None);

        List<string> HashKeys(string key, CommandFlags flags = CommandFlags.None);

        long HashLength(RedisKey key, CommandFlags flags = CommandFlags.None);

        bool HashSet<T>(string key, string hashField, T value, When when = When.Always,
            CommandFlags flags = CommandFlags.None);

        void HashSet(string key, Dictionary<RedisValue, RedisValue> hashFields, CommandFlags flags = CommandFlags.None);

        RedisValue[] HashValues(RedisKey key, CommandFlags flags = CommandFlags.None);

        T ListGetByIndex<T>(string key, long index, CommandFlags flags = CommandFlags.None);

        long ListInsertAfter<T>(string key, RedisValue pivot, T value, CommandFlags flags = CommandFlags.None);

        long ListInsertBefore<T>(string key, RedisValue pivot, T value, CommandFlags flags = CommandFlags.None);

        T ListLeftPop<T>(string key, CommandFlags flags = CommandFlags.None);

        long ListLeftPush(string key, RedisValue[] values, CommandFlags flags = CommandFlags.None);

        long ListLeftPush<T>(string key, T value, When when = When.Always, CommandFlags flags = CommandFlags.None);

        long ListLength(string key, CommandFlags flags = CommandFlags.None);

        RedisValue[] ListRange(string key, long start = 0, long stop = -1, CommandFlags flags = CommandFlags.None);

        long ListRemove<T>(string key, T value, long count = 0, CommandFlags flags = CommandFlags.None);

        T ListRightPop<T>(string key, CommandFlags flags = CommandFlags.None);

        long ListRightPush(string key, RedisValue[] values, CommandFlags flags = CommandFlags.None);

        long ListRightPush<T>(string key, T value, When when = When.Always, CommandFlags flags = CommandFlags.None);

        void ListSetByIndex<T>(string key, long index, T value, CommandFlags flags = CommandFlags.None);

        bool SetAdd<T>(string key, T value, CommandFlags flags = CommandFlags.None);

        long SetAdd(string key, RedisValue[] values, CommandFlags flags = CommandFlags.None);

        long SetLength(string key, CommandFlags flags = CommandFlags.None);

        RedisValue[] SetMembers(RedisKey key, CommandFlags flags = CommandFlags.None);

        bool SetRemove<T>(string key, T value, CommandFlags flags = CommandFlags.None);

        long SetRemove(string key, RedisValue[] values, CommandFlags flags = CommandFlags.None);

        long SortedSetAdd(string key, SortedSetEntry[] values, CommandFlags flags);

        bool SortedSetAdd<T>(RedisKey key, T member, double score, CommandFlags flags);

        long SortedSetAdd(string key, SortedSetEntry[] values, When when = When.Always,
            CommandFlags flags = CommandFlags.None);

        bool SortedSetAdd<T>(string key, T member, double score, When when = When.Always,
            CommandFlags flags = CommandFlags.None);

        double SortedSetDecrement<T>(string key, T member, double value, CommandFlags flags = CommandFlags.None);

        double SortedSetIncrement<T>(string key, T member, double value, CommandFlags flags = CommandFlags.None);

        long SortedSetLength(string key, double min = -1.0 / 0.0, double max = 1.0 / 0.0, Exclude exclude = Exclude.None,
            CommandFlags flags = CommandFlags.None);

        long SortedSetLengthByValue(string key, RedisValue min, RedisValue max, Exclude exclude = Exclude.None,
            CommandFlags flags = CommandFlags.None);

        RedisValue[] SortedSetRangeByRank(string key, long start = 0, long stop = -1, Order order = Order.Ascending,
            CommandFlags flags = CommandFlags.None);

        SortedSetEntry[] SortedSetRangeByRankWithScores(RedisKey key, long start = 0, long stop = -1,
            Order order = Order.Ascending, CommandFlags flags = CommandFlags.None);

        bool SortedSetRemove<T>(string key, T member, CommandFlags flags = CommandFlags.None);

        double? SortedSetScore<T>(string key, T member, CommandFlags flags = CommandFlags.None);

        void Subscribe(string subChannel, Action<RedisChannel, RedisValue> handler);

        long Publish<T>(string channel, T msg);

        void Unsubscribe(string channel);

        void UnsubscribeAll();

        ITransaction CreateTransaction();

        IServer GetServer(string hostAndPort);
    }
}

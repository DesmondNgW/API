﻿using System;
using System.Collections.Generic;
using System.Linq;
using StackExchange.Redis;
using X.Util.Core;
using X.Util.Core.Configuration;
using X.Util.Core.Kernel;
using X.Util.Entities;
using X.Util.Entities.Interface;
using X.Util.Extend.Core;
using X.Util.Provider;

namespace X.Util.Extend.Redis
{
    public class RedisBase : IRedisBase
    {
        #region 构造函数
        public readonly string ServerName = ConfigurationHelper.RedisDefaultServername;
        public readonly int DbNum = -1;
        public static readonly IRedisBase Default = new RedisBase();
        private RedisBase() { }
        public RedisBase(string serverName)
        {
            ServerName = serverName;
        }
        public RedisBase(int dbNum)
        {
            DbNum = dbNum;
        }
        public RedisBase(string serverName, int dbNum)
        {
            ServerName = serverName;
            DbNum = dbNum;
        } 
        #endregion

        #region Key
        public bool KeyExists(string key, CommandFlags flags = CommandFlags.None)
        {
            if (!AppConfig.RedisCacheEnable) return false;
            var redisProvider = new RedisProvider(DbNum, ServerName);
            return CoreAccess<IDatabase>.TryCall(redisProvider.Client.KeyExists, RedisDataHelper.ConvertKey(key),
                flags, redisProvider, new LogOptions<bool>(CoreBase.CallSuccess));
        }

        public bool KeyExpire<T>(string key, T value, DateTime? expire, CommandFlags flags = CommandFlags.None)
        {
            if (!AppConfig.RedisCacheEnable) return false;
            var redisProvider = new RedisProvider(DbNum, ServerName);
            return CoreAccess<IDatabase>.TryCall(
                redisProvider.Client.KeyExpire, RedisDataHelper.ConvertKey(key), expire, flags,
                redisProvider, new LogOptions<bool>(CoreBase.CallSuccess, true));
        }

        public bool KeyExpire<T>(string key, T value, TimeSpan? expire, CommandFlags flags = CommandFlags.None)
        {
            if (!AppConfig.RedisCacheEnable) return false;
            var redisProvider = new RedisProvider(DbNum, ServerName);
            return CoreAccess<IDatabase>.TryCall(
                redisProvider.Client.KeyExpire, RedisDataHelper.ConvertKey(key), expire, flags,
                redisProvider, new LogOptions<bool>(CoreBase.CallSuccess, true));
        }

        public bool KeyDelete(string key, CommandFlags flags = CommandFlags.None)
        {
            if (!AppConfig.RedisCacheEnable) return false;
            var redisProvider = new RedisProvider(DbNum, ServerName);
            return CoreAccess<IDatabase>.TryCall(redisProvider.Client.KeyDelete, RedisDataHelper.ConvertKey(key),
                flags, redisProvider, new LogOptions<bool>(CoreBase.CallSuccess, true));
        }

        public long KeyDelete(List<string> keys, CommandFlags flags = CommandFlags.None)
        {
            if (!AppConfig.RedisCacheEnable) return default(long);
            var redisProvider = new RedisProvider(DbNum, ServerName);
            return CoreAccess<IDatabase>.TryCall(redisProvider.Client.KeyDelete, RedisDataHelper.ConvertKey(keys),
                flags, redisProvider, new LogOptions<long>(CoreBase.CallSuccess, true));
        }

        public bool KeyMove(string key, int database, CommandFlags flags = CommandFlags.None)
        {
            if (!AppConfig.RedisCacheEnable) return default(bool);
            var redisProvider = new RedisProvider(DbNum, ServerName);
            return CoreAccess<IDatabase>.TryCall(redisProvider.Client.KeyMove, RedisDataHelper.ConvertKey(key), database,
                flags, redisProvider, new LogOptions<bool>(CoreBase.CallSuccess, true));
        }

        public bool KeyPersist(string key, CommandFlags flags = CommandFlags.None)
        {
            if (!AppConfig.RedisCacheEnable) return default(bool);
            var redisProvider = new RedisProvider(DbNum, ServerName);
            return CoreAccess<IDatabase>.TryCall(redisProvider.Client.KeyPersist, RedisDataHelper.ConvertKey(key),
                flags, redisProvider, new LogOptions<bool>(CoreBase.CallSuccess, true));
        }

        public string KeyRandom(CommandFlags flags = CommandFlags.None)
        {
            if (!AppConfig.RedisCacheEnable) return string.Empty;
            var redisProvider = new RedisProvider(DbNum, ServerName);
            return CoreAccess<IDatabase>.TryCall(redisProvider.Client.KeyRandom,
                flags, redisProvider, new LogOptions<RedisKey>(CoreBase.CallSuccess, true));
        }

        public bool KeyRename(string key, string newKey, When when = When.Always, CommandFlags flags = CommandFlags.None)
        {
            if (!AppConfig.RedisCacheEnable) return default(bool);
            var redisProvider = new RedisProvider(DbNum, ServerName);
            return CoreAccess<IDatabase>.TryCall(redisProvider.Client.KeyRename, RedisDataHelper.ConvertKey(key),
                RedisDataHelper.ConvertKey(newKey),
                when, flags, redisProvider, new LogOptions<bool>(CoreBase.CallSuccess, true));
        }

        public RedisType KeyType(string key, CommandFlags flags = CommandFlags.None)
        {
            if (!AppConfig.RedisCacheEnable) return default(RedisType);
            var redisProvider = new RedisProvider(DbNum, ServerName);
            return CoreAccess<IDatabase>.TryCall(redisProvider.Client.KeyType, RedisDataHelper.ConvertKey(key), flags, redisProvider, new LogOptions<RedisType>(CoreBase.CallSuccess, true));
        }

        public TimeSpan? KeyTimeToLive(string key, CommandFlags flags = CommandFlags.None)
        {
            if (!AppConfig.RedisCacheEnable) return default(TimeSpan?);
            var redisProvider = new RedisProvider(DbNum, ServerName);
            return CoreAccess<IDatabase>.TryCall(redisProvider.Client.KeyTimeToLive, RedisDataHelper.ConvertKey(key),
                flags, redisProvider, new LogOptions<TimeSpan?>(CoreBase.CallSuccess, true));
        }

        #endregion

        #region String
        public T StringGet<T>(string key, CommandFlags flags = CommandFlags.None)
        {
            if (!AppConfig.RedisCacheEnable) return default(T);
            var redisProvider = new RedisProvider(DbNum, ServerName);
            string ret = CoreAccess<IDatabase>.TryCall(redisProvider.Client.StringGet, RedisDataHelper.ConvertKey(key),
                flags, redisProvider, new LogOptions<RedisValue>(CallSuccess));
            return ret.FromJson<T>();
        }

        public RedisValue[] StringGet(List<string> keys, CommandFlags flags = CommandFlags.None)
        {
            if (!AppConfig.RedisCacheEnable) return null;
            var redisProvider = new RedisProvider(DbNum, ServerName);
            return CoreAccess<IDatabase>.TryCall(redisProvider.Client.StringGet, RedisDataHelper.ConvertKey(keys), flags, redisProvider,
                new LogOptions<RedisValue[]>(CallSuccess));
        }

        public bool StringSet<T>(string key, T value, TimeSpan? expire, When when = When.Always, CommandFlags flags = CommandFlags.None)
        {
            if (!AppConfig.RedisCacheEnable) return false;
            var redisProvider = new RedisProvider(DbNum, ServerName);
            return CoreAccess<IDatabase>.TryCall(
                redisProvider.Client.StringSet, RedisDataHelper.ConvertKey(key), RedisDataHelper.ConvertValue(value), expire, when, flags,
                redisProvider, new LogOptions<bool>(CoreBase.CallSuccess, true));
        }

        public bool StringSet(Dictionary<RedisKey, RedisValue> values, When when = When.Always, CommandFlags flags = CommandFlags.None)
        {
            if (!AppConfig.RedisCacheEnable) return false;
            var redisProvider = new RedisProvider(DbNum, ServerName);
            return CoreAccess<IDatabase>.TryCall(
                redisProvider.Client.StringSet, values.ToArray(), when, flags,
                redisProvider, new LogOptions<bool>(CoreBase.CallSuccess, true));
        }

        public double StringIncrement(string key, double value, CommandFlags flags = CommandFlags.None)
        {
            if (!AppConfig.RedisCacheEnable) return default(double);
            var redisProvider = new RedisProvider(DbNum, ServerName);
            return CoreAccess<IDatabase>.TryCall(
                redisProvider.Client.StringIncrement, RedisDataHelper.ConvertKey(key), value, flags,
                redisProvider, new LogOptions<double>(CoreBase.CallSuccess, true));
        }

        public double StringDecrement(string key, double value, CommandFlags flags = CommandFlags.None)
        {
            if (!AppConfig.RedisCacheEnable) return default(double);
            var redisProvider = new RedisProvider(DbNum, ServerName);
            return CoreAccess<IDatabase>.TryCall(
                redisProvider.Client.StringDecrement, RedisDataHelper.ConvertKey(key), value, flags,
                redisProvider, new LogOptions<double>(CoreBase.CallSuccess, true));
        }

        #endregion

        #region Hash

        public bool KeyExists(string key, string hashField, CommandFlags flags = CommandFlags.None)
        {
            if (!AppConfig.RedisCacheEnable) return false;
            var redisProvider = new RedisProvider(DbNum, ServerName);
            return CoreAccess<IDatabase>.TryCall(redisProvider.Client.HashExists, RedisDataHelper.ConvertKey(key),
                RedisDataHelper.ConvertValue(hashField), flags, redisProvider,
                new LogOptions<bool>(CoreBase.CallSuccess));
        }

        public long HashDecrement(string key, string hashField, long value = 1, CommandFlags flags = CommandFlags.None)
        {
            if (!AppConfig.RedisCacheEnable) return default(long);
            var redisProvider = new RedisProvider(DbNum, ServerName);
            return CoreAccess<IDatabase>.TryCall(redisProvider.Client.HashDecrement, RedisDataHelper.ConvertKey(key),
                RedisDataHelper.ConvertValue(hashField), value, flags, redisProvider,
                new LogOptions<long>(CoreBase.CallSuccess));
        }

        public double HashDecrement(string key, string hashField, double value, CommandFlags flags = CommandFlags.None)
        {
            if (!AppConfig.RedisCacheEnable) return default(double);
            var redisProvider = new RedisProvider(DbNum, ServerName);
            return CoreAccess<IDatabase>.TryCall(redisProvider.Client.HashDecrement, RedisDataHelper.ConvertKey(key),
                RedisDataHelper.ConvertValue(hashField), value, flags, redisProvider,
                new LogOptions<double>(CoreBase.CallSuccess));
        }

        public bool HashDelete(string key, string hashField, CommandFlags flags = CommandFlags.None)
        {
            if (!AppConfig.RedisCacheEnable) return false;
            var redisProvider = new RedisProvider(DbNum, ServerName);
            return CoreAccess<IDatabase>.TryCall(redisProvider.Client.HashDelete, RedisDataHelper.ConvertKey(key),
                RedisDataHelper.ConvertValue(hashField), flags, redisProvider,
                new LogOptions<bool>(CoreBase.CallSuccess));
        }

        public long HashDelete(string key, List<string> hashFields, CommandFlags flags = CommandFlags.None)
        {
            if (!AppConfig.RedisCacheEnable) return default(long);
            var redisProvider = new RedisProvider(DbNum, ServerName);
            return CoreAccess<IDatabase>.TryCall(redisProvider.Client.HashDelete, RedisDataHelper.ConvertKey(key),
                RedisDataHelper.ConvertValue(hashFields), flags, redisProvider,
                new LogOptions<long>(CoreBase.CallSuccess));
        }

        public T HashGet<T>(string key, string hashField, CommandFlags flags = CommandFlags.None)
        {
            if (!AppConfig.RedisCacheEnable) return default(T);
            var redisProvider = new RedisProvider(DbNum, ServerName);
            string ret = CoreAccess<IDatabase>.TryCall(redisProvider.Client.HashGet, RedisDataHelper.ConvertKey(key),
                RedisDataHelper.ConvertValue(hashField), flags, redisProvider,
                new LogOptions<RedisValue>(CallSuccess));
            return ret.FromJson<T>();
        }

        public RedisValue[] HashGet(string key, List<string> hashFields, CommandFlags flags = CommandFlags.None)
        {
            if (!AppConfig.RedisCacheEnable) return default(RedisValue[]);
            var redisProvider = new RedisProvider(DbNum, ServerName);
            return CoreAccess<IDatabase>.TryCall(redisProvider.Client.HashGet, RedisDataHelper.ConvertKey(key),
                RedisDataHelper.ConvertValue(hashFields), flags, redisProvider,
                new LogOptions<RedisValue[]>(CallSuccess));
        }

        public HashEntry[] HashGetAll(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            if (!AppConfig.RedisCacheEnable) return default(HashEntry[]);
            var redisProvider = new RedisProvider(DbNum, ServerName);
            return CoreAccess<IDatabase>.TryCall(redisProvider.Client.HashGetAll, RedisDataHelper.ConvertKey(key), flags,
                redisProvider, new LogOptions<HashEntry[]>(CallSuccess));
        }

        public long HashIncrement(string key, string hashField, long value = 1, CommandFlags flags = CommandFlags.None)
        {
            if (!AppConfig.RedisCacheEnable) return default(long);
            var redisProvider = new RedisProvider(DbNum, ServerName);
            return CoreAccess<IDatabase>.TryCall(redisProvider.Client.HashIncrement, RedisDataHelper.ConvertKey(key),
                RedisDataHelper.ConvertValue(hashField), value, flags, redisProvider,
                new LogOptions<long>(CoreBase.CallSuccess));
        }

        public double HashIncrement(string key, string hashField, double value, CommandFlags flags = CommandFlags.None)
        {
            if (!AppConfig.RedisCacheEnable) return default(double);
            var redisProvider = new RedisProvider(DbNum, ServerName);
            return CoreAccess<IDatabase>.TryCall(redisProvider.Client.HashIncrement, RedisDataHelper.ConvertKey(key),
                RedisDataHelper.ConvertValue(hashField), value, flags, redisProvider,
                new LogOptions<double>(CoreBase.CallSuccess));
        }

        public List<string> HashKeys(string key, CommandFlags flags = CommandFlags.None)
        {
            if (!AppConfig.RedisCacheEnable) return default(List<string>);
            var redisProvider = new RedisProvider(DbNum, ServerName);
            return
                RedisDataHelper.ConvertValue(CoreAccess<IDatabase>.TryCall(redisProvider.Client.HashKeys,
                    RedisDataHelper.ConvertKey(key), flags, redisProvider, new LogOptions<RedisValue[]>(CallSuccess)));
        }

        public long HashLength(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            if (!AppConfig.RedisCacheEnable) return default(long);
            var redisProvider = new RedisProvider(DbNum, ServerName);
            return CoreAccess<IDatabase>.TryCall(redisProvider.Client.HashLength, RedisDataHelper.ConvertKey(key),
                flags, redisProvider, new LogOptions<long>(CoreBase.CallSuccess));
        }

        public bool HashSet<T>(string key, string hashField, T value, When when = When.Always, CommandFlags flags = CommandFlags.None)
        {
            if (!AppConfig.RedisCacheEnable) return default(bool);
            var redisProvider = new RedisProvider(DbNum, ServerName);
            return CoreAccess<IDatabase>.TryCall(redisProvider.Client.HashSet, RedisDataHelper.ConvertKey(key),
                RedisDataHelper.ConvertValue(hashField), RedisDataHelper.ConvertValue(value), when, flags, redisProvider, new LogOptions<bool>(CoreBase.CallSuccess));
        }

        public void HashSet(string key, Dictionary<RedisValue, RedisValue> hashFields, CommandFlags flags = CommandFlags.None)
        {
            if (!AppConfig.RedisCacheEnable) return;
            var redisProvider = new RedisProvider(DbNum, ServerName);
            CoreAccess<IDatabase>.TryCallAsync(redisProvider.Client.HashSet, RedisDataHelper.ConvertKey(key),
                hashFields.Select(p => new HashEntry(p.Key, p.Value)).ToArray(), flags, redisProvider, null, new LogOptions());
        }

        public RedisValue[] HashValues(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            if (!AppConfig.RedisCacheEnable) return default(RedisValue[]);
            var redisProvider = new RedisProvider(DbNum, ServerName);
            return CoreAccess<IDatabase>.TryCall(redisProvider.Client.HashValues, RedisDataHelper.ConvertKey(key), flags,
                redisProvider, new LogOptions<RedisValue[]>(CallSuccess));
        }

        #endregion

        #region List
        public T ListGetByIndex<T>(string key, long index, CommandFlags flags = CommandFlags.None)
        {
            if (!AppConfig.RedisCacheEnable) return default(T);
            var redisProvider = new RedisProvider(DbNum, ServerName);
            string ret = CoreAccess<IDatabase>.TryCall(redisProvider.Client.ListGetByIndex, RedisDataHelper.ConvertKey(key),
                index, flags, redisProvider,
                new LogOptions<RedisValue>(CallSuccess));
            return ret.FromJson<T>();
        }

        public long ListInsertAfter<T>(string key, RedisValue pivot, T value, CommandFlags flags = CommandFlags.None)
        {
            if (!AppConfig.RedisCacheEnable) return default(long);
            var redisProvider = new RedisProvider(DbNum, ServerName);
            return CoreAccess<IDatabase>.TryCall(redisProvider.Client.ListInsertAfter, RedisDataHelper.ConvertKey(key),
                pivot, RedisDataHelper.ConvertValue(value), flags, redisProvider,
                new LogOptions<long>(CoreBase.CallSuccess));
        }

        public long ListInsertBefore<T>(string key, RedisValue pivot, T value, CommandFlags flags = CommandFlags.None)
        {
            if (!AppConfig.RedisCacheEnable) return default(long);
            var redisProvider = new RedisProvider(DbNum, ServerName);
            return CoreAccess<IDatabase>.TryCall(redisProvider.Client.ListInsertBefore, RedisDataHelper.ConvertKey(key),
                pivot, RedisDataHelper.ConvertValue(value), flags, redisProvider,
                new LogOptions<long>(CoreBase.CallSuccess));
        }

        public T ListLeftPop<T>(string key, CommandFlags flags = CommandFlags.None)
        {
            if (!AppConfig.RedisCacheEnable) return default(T);
            var redisProvider = new RedisProvider(DbNum, ServerName);
            string ret = CoreAccess<IDatabase>.TryCall(redisProvider.Client.ListLeftPop, RedisDataHelper.ConvertKey(key),
                flags, redisProvider, new LogOptions<RedisValue>(CallSuccess));
            return ret.FromJson<T>();
        }

        public long ListLeftPush(string key, RedisValue[] values, CommandFlags flags = CommandFlags.None)
        {
            if (!AppConfig.RedisCacheEnable) return default(long);
            var redisProvider = new RedisProvider(DbNum, ServerName);
            return CoreAccess<IDatabase>.TryCall(redisProvider.Client.ListLeftPush, RedisDataHelper.ConvertKey(key),
                values, flags, redisProvider, new LogOptions<long>(CoreBase.CallSuccess));
        }

        public long ListLeftPush<T>(string key, T value, When when = When.Always, CommandFlags flags = CommandFlags.None)
        {
            if (!AppConfig.RedisCacheEnable) return default(long);
            var redisProvider = new RedisProvider(DbNum, ServerName);
            return CoreAccess<IDatabase>.TryCall(redisProvider.Client.ListLeftPush, RedisDataHelper.ConvertKey(key),
                RedisDataHelper.ConvertValue(value), when, flags, redisProvider,
                new LogOptions<long>(CoreBase.CallSuccess));
        }

        public long ListLength(string key, CommandFlags flags = CommandFlags.None)
        {
            if (!AppConfig.RedisCacheEnable) return default(long);
            var redisProvider = new RedisProvider(DbNum, ServerName);
            return CoreAccess<IDatabase>.TryCall(redisProvider.Client.ListLength, RedisDataHelper.ConvertKey(key),
                flags, redisProvider, new LogOptions<long>(CoreBase.CallSuccess));
        }

        public RedisValue[] ListRange(string key, long start = 0, long stop = -1, CommandFlags flags = CommandFlags.None)
        {
            if (!AppConfig.RedisCacheEnable) return default(RedisValue[]);
            var redisProvider = new RedisProvider(DbNum, ServerName);
            return CoreAccess<IDatabase>.TryCall(redisProvider.Client.ListRange, RedisDataHelper.ConvertKey(key),
                start, stop, flags, redisProvider, new LogOptions<RedisValue[]>(CoreBase.CallSuccess));
        }

        public long ListRemove<T>(string key, T value, long count = 0, CommandFlags flags = CommandFlags.None)
        {
            if (!AppConfig.RedisCacheEnable) return default(long);
            var redisProvider = new RedisProvider(DbNum, ServerName);
            return CoreAccess<IDatabase>.TryCall(redisProvider.Client.ListRemove, RedisDataHelper.ConvertKey(key),
                RedisDataHelper.ConvertValue(value), count, flags, redisProvider,
                new LogOptions<long>(CoreBase.CallSuccess));
        }

        public T ListRightPop<T>(string key, CommandFlags flags = CommandFlags.None)
        {
            if (!AppConfig.RedisCacheEnable) return default(T);
            var redisProvider = new RedisProvider(DbNum, ServerName);
            string ret = CoreAccess<IDatabase>.TryCall(redisProvider.Client.ListRightPop, RedisDataHelper.ConvertKey(key),
                flags, redisProvider, new LogOptions<RedisValue>(CallSuccess));
            return ret.FromJson<T>();
        }

        public long ListRightPush(string key, RedisValue[] values, CommandFlags flags = CommandFlags.None)
        {
            if (!AppConfig.RedisCacheEnable) return default(long);
            var redisProvider = new RedisProvider(DbNum, ServerName);
            return CoreAccess<IDatabase>.TryCall(redisProvider.Client.ListRightPush, RedisDataHelper.ConvertKey(key),
                values, flags, redisProvider, new LogOptions<long>(CoreBase.CallSuccess));
        }

        public long ListRightPush<T>(string key, T value, When when = When.Always, CommandFlags flags = CommandFlags.None)
        {
            if (!AppConfig.RedisCacheEnable) return default(long);
            var redisProvider = new RedisProvider(DbNum, ServerName);
            return CoreAccess<IDatabase>.TryCall(redisProvider.Client.ListRightPush, RedisDataHelper.ConvertKey(key),
                RedisDataHelper.ConvertValue(value), when, flags, redisProvider,
                new LogOptions<long>(CoreBase.CallSuccess));
        }

        public void ListSetByIndex<T>(string key, long index, T value, CommandFlags flags = CommandFlags.None)
        {
            if (!AppConfig.RedisCacheEnable) return;
            var redisProvider = new RedisProvider(DbNum, ServerName);
            CoreAccess<IDatabase>.TryCallAsync(redisProvider.Client.ListSetByIndex, RedisDataHelper.ConvertKey(key),
                index, RedisDataHelper.ConvertValue(value), flags, redisProvider, null, new LogOptions());
        }

        #endregion

        #region Set 集合

        public bool SetAdd<T>(string key, T value, CommandFlags flags = CommandFlags.None)
        {
            if (!AppConfig.RedisCacheEnable) return false;
            var redisProvider = new RedisProvider(DbNum, ServerName);
            return CoreAccess<IDatabase>.TryCall(redisProvider.Client.SetAdd, RedisDataHelper.ConvertKey(key),
                RedisDataHelper.ConvertValue(value), flags, redisProvider, new LogOptions<bool>(CoreBase.CallSuccess));
        }

        public long SetAdd(string key, RedisValue[] values, CommandFlags flags = CommandFlags.None)
        {
            if (!AppConfig.RedisCacheEnable) return default(long);
            var redisProvider = new RedisProvider(DbNum, ServerName);
            return CoreAccess<IDatabase>.TryCall(redisProvider.Client.SetAdd, RedisDataHelper.ConvertKey(key),
                values, flags, redisProvider, new LogOptions<long>(CoreBase.CallSuccess));
        }

        public long SetLength(string key, CommandFlags flags = CommandFlags.None)
        {
            if (!AppConfig.RedisCacheEnable) return default(long);
            var redisProvider = new RedisProvider(DbNum, ServerName);
            return CoreAccess<IDatabase>.TryCall(redisProvider.Client.SetLength, RedisDataHelper.ConvertKey(key),
                flags, redisProvider, new LogOptions<long>(CoreBase.CallSuccess));
        }

        public RedisValue[] SetMembers(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            if (!AppConfig.RedisCacheEnable) return default(RedisValue[]);
            var redisProvider = new RedisProvider(DbNum, ServerName);
            return CoreAccess<IDatabase>.TryCall(redisProvider.Client.SetMembers, RedisDataHelper.ConvertKey(key),
                flags, redisProvider, new LogOptions<RedisValue[]>(CoreBase.CallSuccess));
        }

        public bool SetRemove<T>(string key, T value, CommandFlags flags = CommandFlags.None)
        {
            if (!AppConfig.RedisCacheEnable) return false;
            var redisProvider = new RedisProvider(DbNum, ServerName);
            return CoreAccess<IDatabase>.TryCall(redisProvider.Client.SetRemove, RedisDataHelper.ConvertKey(key),
                RedisDataHelper.ConvertValue(value), flags, redisProvider, new LogOptions<bool>(CoreBase.CallSuccess));
        }

        public long SetRemove(string key, RedisValue[] values, CommandFlags flags = CommandFlags.None)
        {
            if (!AppConfig.RedisCacheEnable) return default(long);
            var redisProvider = new RedisProvider(DbNum, ServerName);
            return CoreAccess<IDatabase>.TryCall(redisProvider.Client.SetRemove, RedisDataHelper.ConvertKey(key),
                values, flags, redisProvider, new LogOptions<long>(CoreBase.CallSuccess));
        }

        #endregion

        #region SortedSet 有序集合

        public long SortedSetAdd(string key, SortedSetEntry[] values, CommandFlags flags)
        {
            if (!AppConfig.RedisCacheEnable) return default(long);
            var redisProvider = new RedisProvider(DbNum, ServerName);
            return CoreAccess<IDatabase>.TryCall(redisProvider.Client.SortedSetAdd, RedisDataHelper.ConvertKey(key),
                values, flags, redisProvider, new LogOptions<long>(CoreBase.CallSuccess));
        }

        public bool SortedSetAdd<T>(RedisKey key, T member, double score, CommandFlags flags)
        {
            if (!AppConfig.RedisCacheEnable) return default(bool);
            var redisProvider = new RedisProvider(DbNum, ServerName);
            return CoreAccess<IDatabase>.TryCall(redisProvider.Client.SortedSetAdd, RedisDataHelper.ConvertKey(key),
                RedisDataHelper.ConvertValue(member), score, flags, redisProvider,
                new LogOptions<bool>(CoreBase.CallSuccess));
        }

        public long SortedSetAdd(string key, SortedSetEntry[] values, When when = When.Always, CommandFlags flags = CommandFlags.None)
        {
            if (!AppConfig.RedisCacheEnable) return default(long);
            var redisProvider = new RedisProvider(DbNum, ServerName);
            return CoreAccess<IDatabase>.TryCall(redisProvider.Client.SortedSetAdd, RedisDataHelper.ConvertKey(key),
                values, when, flags, redisProvider, new LogOptions<long>(CoreBase.CallSuccess));
        }

        public bool SortedSetAdd<T>(string key, T member, double score, When when = When.Always, CommandFlags flags = CommandFlags.None)
        {
            if (!AppConfig.RedisCacheEnable) return default(bool);
            var redisProvider = new RedisProvider(DbNum, ServerName);
            return CoreAccess<IDatabase>.TryCall(redisProvider.Client.SortedSetAdd, RedisDataHelper.ConvertKey(key),
                RedisDataHelper.ConvertValue(member), score, when, flags, redisProvider,
                new LogOptions<bool>(CoreBase.CallSuccess));
        }

        public double SortedSetDecrement<T>(string key, T member, double value, CommandFlags flags = CommandFlags.None)
        {
            if (!AppConfig.RedisCacheEnable) return default(double);
            var redisProvider = new RedisProvider(DbNum, ServerName);
            return CoreAccess<IDatabase>.TryCall(redisProvider.Client.SortedSetDecrement,
                RedisDataHelper.ConvertKey(key), RedisDataHelper.ConvertValue(member), value, flags, redisProvider,
                new LogOptions<double>(CoreBase.CallSuccess, true));
        }

        public double SortedSetIncrement<T>(string key, T member, double value, CommandFlags flags = CommandFlags.None)
        {
            if (!AppConfig.RedisCacheEnable) return default(double);
            var redisProvider = new RedisProvider(DbNum, ServerName);
            return CoreAccess<IDatabase>.TryCall(redisProvider.Client.SortedSetIncrement,
                RedisDataHelper.ConvertKey(key), RedisDataHelper.ConvertValue(member), value, flags, redisProvider,
                new LogOptions<double>(CoreBase.CallSuccess, true));
        }

        public long SortedSetLength(string key, double min = -1.0/0.0, double max = 1.0/0.0, Exclude exclude = Exclude.None, CommandFlags flags = CommandFlags.None)
        {
            if (!AppConfig.RedisCacheEnable) return default(long);
            var redisProvider = new RedisProvider(DbNum, ServerName);
            return CoreAccess<IDatabase>.TryCall(redisProvider.Client.SortedSetLength,
                RedisDataHelper.ConvertKey(key), min, max, exclude, flags, redisProvider,
                new LogOptions<long>(CoreBase.CallSuccess));
        }

        public long SortedSetLengthByValue(string key, RedisValue min, RedisValue max, Exclude exclude = Exclude.None, CommandFlags flags = CommandFlags.None)
        {
            if (!AppConfig.RedisCacheEnable) return default(long);
            var redisProvider = new RedisProvider(DbNum, ServerName);
            return CoreAccess<IDatabase>.TryCall(redisProvider.Client.SortedSetLengthByValue,
                RedisDataHelper.ConvertKey(key), min, max, exclude, flags, redisProvider,
                new LogOptions<long>(CoreBase.CallSuccess));
        }

        public RedisValue[] SortedSetRangeByRank(string key, long start = 0, long stop = -1, Order order = Order.Ascending, CommandFlags flags = CommandFlags.None)
        {
            if (!AppConfig.RedisCacheEnable) return default(RedisValue[]);
            var redisProvider = new RedisProvider(DbNum, ServerName);
            return CoreAccess<IDatabase>.TryCall(redisProvider.Client.SortedSetRangeByRank,
                RedisDataHelper.ConvertKey(key), start, stop, order, flags, redisProvider,
                new LogOptions<RedisValue[]>(CoreBase.CallSuccess, true));
        }

        public SortedSetEntry[] SortedSetRangeByRankWithScores(RedisKey key, long start = 0, long stop = -1, Order order = Order.Ascending, CommandFlags flags = CommandFlags.None)
        {
            if (!AppConfig.RedisCacheEnable) return default(SortedSetEntry[]);
            var redisProvider = new RedisProvider(DbNum, ServerName);
            return CoreAccess<IDatabase>.TryCall(redisProvider.Client.SortedSetRangeByRankWithScores,
                RedisDataHelper.ConvertKey(key), start, stop, order, flags, redisProvider,
                new LogOptions<SortedSetEntry[]>(CoreBase.CallSuccess, true));
        }

        public bool SortedSetRemove<T>(string key, T member, CommandFlags flags = CommandFlags.None)
        {
            if (!AppConfig.RedisCacheEnable) return default(bool);
            var redisProvider = new RedisProvider(DbNum, ServerName);
            return CoreAccess<IDatabase>.TryCall(redisProvider.Client.SortedSetRemove, RedisDataHelper.ConvertKey(key),
                RedisDataHelper.ConvertValue(member), flags, redisProvider,
                new LogOptions<bool>(CoreBase.CallSuccess));
        }

        public double? SortedSetScore<T>(string key, T member, CommandFlags flags = CommandFlags.None)
        {
            if (!AppConfig.RedisCacheEnable) return default(double?);
            var redisProvider = new RedisProvider(DbNum, ServerName);
            return CoreAccess<IDatabase>.TryCall(redisProvider.Client.SortedSetScore, RedisDataHelper.ConvertKey(key),
                RedisDataHelper.ConvertValue(member), flags, redisProvider,
                new LogOptions<double?>(CoreBase.CallSuccess));
        }

        #endregion

        #region 其他
        public void Subscribe(string subChannel, Action<RedisChannel, RedisValue> handler)
        {
            if (!AppConfig.RedisCacheEnable) return;
            var redisProvider = new RedisProvider(DbNum, ServerName);
            var sub = redisProvider.ConnectionMultiplexer.GetSubscriber();
            sub.Subscribe(subChannel, (channel, message) =>
            {
                if (handler != null)
                {
                    handler(channel, message);
                }
            });
        }

        public long Publish<T>(string channel, T msg)
        {
            if (!AppConfig.RedisCacheEnable) return default(long);
            var redisProvider = new RedisProvider(DbNum, ServerName);
            var sub = redisProvider.ConnectionMultiplexer.GetSubscriber();
            return sub.Publish(channel, RedisDataHelper.ConvertValue(msg));
        }

        public void Unsubscribe(string channel)
        {
            if (!AppConfig.RedisCacheEnable) return;
            var redisProvider = new RedisProvider(DbNum, ServerName);
            var sub = redisProvider.ConnectionMultiplexer.GetSubscriber();
            sub.Unsubscribe(channel);
        }

        public void UnsubscribeAll()
        {
            if (!AppConfig.RedisCacheEnable) return;
            var redisProvider = new RedisProvider(DbNum, ServerName);
            var sub = redisProvider.ConnectionMultiplexer.GetSubscriber();
            sub.UnsubscribeAll();
        }

        public ITransaction CreateTransaction()
        {
            if (!AppConfig.RedisCacheEnable) return default(ITransaction);
            var redisProvider = new RedisProvider(DbNum, ServerName);
            return redisProvider.Client.CreateTransaction();
        }

        public IServer GetServer(string hostAndPort)
        {
            if (!AppConfig.RedisCacheEnable) return default(IServer);
            var redisProvider = new RedisProvider(DbNum, ServerName);
            return redisProvider.ConnectionMultiplexer.GetServer(hostAndPort);
        }
        #endregion

        #region CallSuccess
        public static bool CallSuccess(RedisValue iresult)
        {
            return !string.IsNullOrEmpty(iresult);
        }

        public static bool CallSuccess(RedisValue[] iresult)
        {
            return iresult != null && iresult.Length > 0;
        }

        public static bool CallSuccess(HashEntry[] iresult)
        {
            return iresult != null && iresult.Length > 0;
        }
        #endregion

    }
}

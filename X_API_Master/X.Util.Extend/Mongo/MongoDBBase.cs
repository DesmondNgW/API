﻿using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using X.Util.Core;
using X.Util.Core.Configuration;
using X.Util.Core.Kernel;
using X.Util.Entities;
using X.Util.Entities.Interface;
using X.Util.Extend.Core;
using X.Util.Provider;

namespace X.Util.Extend.Mongo
{
    /// <summary>
    /// 读写mongoDB
    /// </summary>
    public sealed class MongoDbBase<T> : IMongoDbBase<T> where T : MongoBaseModel
    {
        #region 构造函数
        public string ServerName = ConfigurationHelper.MongoDefaultServername;
        public static IMongoDbBase<T> Default = new MongoDbBase<T>();
        private MongoDbBase() { }
        public MongoDbBase(string serverName)
        {
            ServerName = serverName;
        }
        #endregion

        #region 索引操作

        /// <summary>
        /// 创建索引
        /// </summary>
        /// <param name="database"></param>
        /// <param name="collection"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public void CreateIndex(string database, string collection, IMongoIndexKeys index)
        {
            if (!AppConfig.MongoDbEnable) return;
            var mc = new MongoDbProvider<T>(database, collection);
            CoreAccess<MongoCollection<T>>.TryCallAsync(mc.Client.CreateIndex, index, mc, null, new LogOptions<WriteConcernResult>(CallSuccess, true));
        }

        /// <summary>
        /// 删除索引
        /// </summary>
        /// <param name="database"></param>
        /// <param name="collection"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public void DropIndex(string database, string collection, IMongoIndexKeys index)
        {
            if (!AppConfig.MongoDbEnable) return;
            var mc = new MongoDbProvider<T>(database, collection);
            CoreAccess<MongoCollection<T>>.TryCallAsync(mc.Client.DropIndex, index, mc, null, new LogOptions<CommandResult>(CallSuccess, true));
        }

        /// <summary>
        /// 索引是否存在
        /// </summary>
        /// <param name="database"></param>
        /// <param name="collection"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public bool IndexExists(string database, string collection, IMongoIndexKeys index)
        {
            if (!AppConfig.MongoDbEnable) return false;
            var mc = new MongoDbProvider<T>(database, collection);
            return CoreAccess<MongoCollection<T>>.TryCall(mc.Client.IndexExists, index, mc, new LogOptions<bool>(CoreBase.CallSuccess, true));
        }

        /// <summary>
        /// 获取所有索引
        /// </summary>
        /// <param name="database"></param>
        /// <param name="collection"></param>
        /// <returns></returns>
        public IndexKeysDocument GetAllIndexes(string database, string collection)
        {
            if (!AppConfig.MongoDbEnable) return null;
            var mc = new MongoDbProvider<T>(database, collection);
            return CoreAccess<MongoCollection<T>>.TryCall(mc.Client.GetIndexes, mc, new LogOptions<GetIndexesResult>(iresult => iresult != null && iresult.Count > 0 && iresult.RawDocuments != null)).ToBsonDocument() as IndexKeysDocument;
        }
        #endregion

        #region 增删改操作
        public void SaveMongo(T t, string database, string collection)
        {
            if (!AppConfig.MongoDbEnable) return;
            var property = typeof(T).GetProperty("Id");
            if (property != null && Equals(property.GetValue(t, null), null))
            {
                property.SetValue(t, Guid.NewGuid().ToString("N"), null);
            }
            var mc = new MongoDbProvider<T>(database, collection);
            CoreAccess<MongoCollection<T>>.TryCallAsync(mc.Client.Save, t, WriteConcern.Acknowledged, mc, null, new LogOptions<WriteConcernResult>(CallSuccess, true));
        }

        /// <summary>
        /// 插入会失败（若有主键）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="database"></param>
        /// <param name="collection"></param>
        public void InsertMongo(T t, string database, string collection)
        {
            if (!AppConfig.MongoDbEnable) return;
            var property = typeof(T).GetProperty("Id");
            if (property != null && Equals(property.GetValue(t, null), null))
            {
                property.SetValue(t, Guid.NewGuid().ToString("N"), null);
            }
            var mc = new MongoDbProvider<T>(database, collection);
            CoreAccess<MongoCollection<T>>.TryCallAsync(mc.Client.Insert, t, WriteConcern.Acknowledged, mc, null, new LogOptions<WriteConcernResult>(CallSuccess, true));
        }

        /// <summary>
        /// 批量插入会失败（若有主键）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="database"></param>
        /// <param name="collection"></param>
        public void InsertBatchMongo(IEnumerable<T> list, string database, string collection)
        {
            if (!AppConfig.MongoDbEnable) return;
            var property = typeof(T).GetProperty("Id");
            var enumerable = list as T[] ?? list.ToArray();
            foreach (var t in enumerable.Where(t => property != null && Equals(property.GetValue(t, null), null)))
            {
                property.SetValue(t, Guid.NewGuid().ToString("N"), null);
            }
            var mc = new MongoDbProvider<T>(database, collection);
            CoreAccess<MongoCollection<T>>.TryCallAsync(mc.Client.InsertBatch, enumerable, WriteConcern.Acknowledged, mc, null, new LogOptions<IEnumerable<WriteConcernResult>>(CallSuccess, true));
        }

        public void SaveMongo(Func<T> loader, string database, string collection)
        {
            if (!AppConfig.MongoDbEnable) return;
            SaveMongo(loader(), database, collection);
        }

        /// <summary>
        /// 更新MongoDB数据
        /// </summary>
        /// <param name="database"></param>
        /// <param name="collection"></param>
        /// <param name="query"></param>
        /// <param name="update"></param>
        /// <param name="flag"></param>
        /// <returns></returns>
        public void UpdateMongo(string database, string collection, IMongoQuery query, IMongoUpdate update, UpdateFlags flag = UpdateFlags.Multi)
        {
            if (!AppConfig.MongoDbEnable) return;
            var mc = new MongoDbProvider<T>(database, collection);
            CoreAccess<MongoCollection<T>>.TryCallAsync(mc.Client.Update, query, update, flag, WriteConcern.Acknowledged, mc, null, new LogOptions<WriteConcernResult>(CallSuccess, true));
        }

        /// <summary>
        /// 删除MongoDB
        /// </summary>
        /// <param name="database"></param>
        /// <param name="collection"></param>
        /// <param name="query"></param>
        /// <param name="flag"></param>
        /// <returns></returns>
        public void RemoveMongo(string database, string collection, IMongoQuery query, RemoveFlags flag = RemoveFlags.None)
        {
            if (!AppConfig.MongoDbEnable) return;
            var mc = new MongoDbProvider<T>(database, collection);
            CoreAccess<MongoCollection<T>>.TryCallAsync(mc.Client.Remove, query, flag, WriteConcern.Acknowledged, mc, null, new LogOptions<WriteConcernResult>(CallSuccess, true));
        }

        /// <summary>
        /// 清空指定Collection所有数据
        /// </summary>
        /// <param name="database"></param>
        /// <param name="collection"></param>
        /// <returns></returns>
        public void RemoveAllMongo(string database, string collection)
        {
            if (!AppConfig.MongoDbEnable) return;
            var mc = new MongoDbProvider<T>(database, collection);
            CoreAccess<MongoCollection<T>>.TryCallAsync(mc.Client.RemoveAll, WriteConcern.Acknowledged, mc, null, new LogOptions<WriteConcernResult>(CallSuccess, true));
        }

        /// <summary>
        /// 删除指定Collection
        /// </summary>
        /// <param name="database"></param>
        /// <param name="collection"></param>
        /// <returns></returns>
        public void DropMongo(string database, string collection)
        {
            if (!AppConfig.MongoDbEnable) return;
            var mc = new MongoDbProvider<T>(database, collection);
            CoreAccess<MongoCollection<T>>.TryCallAsync(mc.Client.Drop, mc, null, new LogOptions<CommandResult>(CallSuccess, true));
        }
        #endregion

        #region 查询操作
        /// <summary>
        /// 查询MongoDB
        /// </summary>
        /// <param name="database"></param>
        /// <param name="collection"></param>
        /// <param name="query">查询语句</param>
        /// <param name="field">字段</param>
        /// <param name="sortBy">排序</param>
        /// <param name="limit">选取条数</param>
        /// <param name="skip">从索引值开始选取</param>
        public MongoCursor<T> Find(string database, string collection, IMongoQuery query, IMongoFields field = null, IMongoSortBy sortBy = null, int limit = 0, int skip = 0)
        {
            if (!AppConfig.MongoDbEnable) return default(MongoCursor<T>);
            var mc = new MongoDbProvider<T>(database, collection);
            var result = CoreAccess<MongoCollection<T>>.TryCall((iquery, ifield, isortBy, ilimit, iskip) =>
            {
                var docs = mc.Client.Find(iquery);
                docs = (ifield != null) ? docs.SetFields(ifield) : docs;
                docs = (isortBy != null) ? docs.SetSortOrder(isortBy) : docs;
                docs = (ilimit != 0) ? docs.SetLimit(ilimit) : docs;
                docs = (iskip != 0) ? docs.SetSkip(iskip) : docs;
                return docs;
            }, query, field, sortBy, limit, skip, mc, new LogOptions<MongoCursor<T>>(iresult => iresult != null && iresult.Any()));
            return result;
        }

        /// <summary>
        /// 查询MongoDB 返回BsonDocument
        /// </summary>
        /// <param name="database"></param>
        /// <param name="collection"></param>
        /// <param name="query"></param>
        /// <param name="field"></param>
        /// <param name="sortBy"></param>
        /// <param name="limit"></param>
        /// <param name="skip"></param>
        /// <returns></returns>
        public MongoCursor<BsonDocument> FindBsonDocument(string database, string collection, IMongoQuery query, IMongoFields field = null, IMongoSortBy sortBy = null, int limit = 0, int skip = 0)
        {
            if (!AppConfig.MongoDbEnable) return null;
            var mc = new MongoDbProvider<BsonDocument>(database, collection);
            var result = CoreAccess<MongoCollection<BsonDocument>>.TryCall((iquery, ifield, isortBy, ilimit, iskip) =>
            {
                var docs = mc.Client.Find(iquery);
                docs = (ifield != null) ? docs.SetFields(ifield) : docs;
                docs = (isortBy != null) ? docs.SetSortOrder(isortBy) : docs;
                docs = (ilimit != 0) ? docs.SetLimit(ilimit) : docs;
                docs = (iskip != 0) ? docs.SetSkip(iskip) : docs;
                return docs;
            }, query, field, sortBy, limit, skip, mc, new LogOptions<MongoCursor<BsonDocument>>(iresult => iresult != null && iresult.Any()));
            return result;
        }

        /// <summary>
        /// 取MongoDB条数
        /// </summary>
        /// <param name="database"></param>
        /// <param name="collection"></param>
        /// <param name="query">查询语句</param>
        /// <param name="limit">选取条数</param>
        /// <param name="skip">从索引值开始选取</param>
        public long Count(string database, string collection, IMongoQuery query, int limit = 0, int skip = 0)
        {
            if (!AppConfig.MongoDbEnable) return 0;
            var mc = new MongoDbProvider<T>(database, collection);
            return CoreAccess<MongoCollection<T>>.TryCall((iquery, ilimit, iskip) =>
            {
                var docs = mc.Client.Find(iquery);
                docs = (ilimit != 0) ? docs.SetLimit(ilimit) : docs;
                docs = (iskip != 0) ? docs.SetSkip(iskip) : docs;
                return docs.Count();
            }, query, limit, skip, mc, new LogOptions<long>(CoreBase.CallSuccess));
        }

        /// <summary>
        /// 查询MongoDB,取第一条
        /// </summary>
        /// <param name="database"></param>
        /// <param name="collection"></param>
        /// <param name="query">查询语句</param>
        /// <returns></returns>
        public T FindOne(string database, string collection, IMongoQuery query)
        {
            if (!AppConfig.MongoDbEnable) return null;
            var mc = new MongoDbProvider<T>(database, collection);
            return CoreAccess<MongoCollection<T>>.TryCall(mc.Client.FindOne, query, mc, new LogOptions<T>(CoreBase.CallSuccess));
        }

        /// <summary>
        /// 查询Distinct
        /// </summary>
        /// <param name="database"></param>
        /// <param name="collection"></param>
        /// <param name="query"></param>
        /// <param name="field"></param>
        /// <returns></returns>
        public IEnumerable<BsonValue> Distinct(string database, string collection, IMongoQuery query, string field)
        {
            if (!AppConfig.MongoDbEnable) return null;
            var mc = new MongoDbProvider<T>(database, collection);
            return CoreAccess<MongoCollection<T>>.TryCall(mc.Client.Distinct, field, query, mc, new LogOptions<IEnumerable<BsonValue>>(CoreBase.CallSuccess));
        }
        #endregion

        #region ToEntity & CallSuccess
        private static bool CallSuccess(WriteConcernResult result)
        {
            return result != null && result.Ok;
        }

        private static bool CallSuccess(IEnumerable<WriteConcernResult> result)
        {
            var writeConcernResults = result as WriteConcernResult[] ?? result.ToArray();
            return result != null && writeConcernResults.All(p => p.Ok) && writeConcernResults.Any();
        }

        private static bool CallSuccess(CommandResult result)
        {
            return result != null && result.Ok;
        }

        /// <summary>
        /// MongoDB数据转为为实体List（需要实体和MongoDB字段一一对应）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="docs"></param>
        /// <returns></returns>
        public static List<T> ToEntity(MongoCursor<BsonDocument> docs)
        {
            var list = new List<T>();
            foreach (var doc in docs)
            {
                var document = doc.ToDictionary();
                var result = document.AutoMapper<T>();
                typeof(T).GetProperty("Id").SetValue(result, document["_id"], null);
                list.Add(result);
            }
            return list;
        }

        /// <summary>
        /// MongoDB数据转为为实体List（需要实体和MongoDB字段一一对应）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="doc"></param>
        /// <returns></returns>
        public static T ToEntity(BsonDocument doc)
        {
            var document = doc.ToDictionary();
            var result = document.AutoMapper<T>();
            typeof(T).GetProperty("Id").SetValue(result, document["_id"], null);
            return result;
        }
        #endregion
    }
}

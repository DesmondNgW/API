using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using X.Util.Core;
using X.Util.Entities;
using X.Util.Extend.Core;
using X.Util.Provider;

namespace X.Util.Extend.Mongo
{
    /// <summary>
    /// 读写mongoDB
    /// </summary>
    public sealed class MongoDbBase
    {
        #region 构造函数
        public string ServerName = ConfigurationHelper.MongoDefaultServername;
        public static MongoDbBase Default = new MongoDbBase();
        private const LogDomain EDomain = LogDomain.ThirdParty;
        public MongoDbBase() { }
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
        public void CreateIndex(string database, string collection, IndexKeysDocument index)
        {
            var mc = new MongoDbProvider(database, collection, ServerName);
            CoreAccess.TryCallAsync(EDomain, mc.Collection.CreateIndex, index, CallSuccess, mc.Dispose, null, false, ServerName);
        }

        /// <summary>
        /// 删除索引
        /// </summary>
        /// <param name="database"></param>
        /// <param name="collection"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public void DropIndex(string database, string collection, IndexKeysDocument index)
        {
            var mc = new MongoDbProvider(database, collection, ServerName);
            CoreAccess.TryCallAsync(EDomain, mc.Collection.DropIndex, index, CallSuccess, mc.Dispose, null, false, ServerName);
        }

        /// <summary>
        /// 索引是否存在
        /// </summary>
        /// <param name="database"></param>
        /// <param name="collection"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public bool IndexExists(string database, string collection, IndexKeysDocument index)
        {
            var mc = new MongoDbProvider(database, collection, ServerName);
            return CoreAccess.TryCall(EDomain, mc.Collection.IndexExists, index, CoreBase.CallSuccess, mc.Dispose, 1, true, ServerName);
        }

        /// <summary>
        /// 获取所有索引
        /// </summary>
        /// <param name="database"></param>
        /// <param name="collection"></param>
        /// <returns></returns>
        public IndexKeysDocument GetAllIndexes(string database, string collection)
        {
            var mc = new MongoDbProvider(database, collection, ServerName);
            return CoreAccess.TryCall(EDomain, mc.Collection.GetIndexes, iresult => iresult != null && iresult.Count > 0 && iresult.RawDocuments != null, mc.Dispose, 1, true, ServerName).ToBsonDocument() as IndexKeysDocument;
        }
        #endregion

        #region 增删改操作
        public void SaveMongo<T>(T t, string database, string collection) where T : MongoBaseModel
        {
            var property = typeof(T).GetProperty("Id");
            if (property != null && Equals(property.GetValue(t, null), null))
            {
                property.SetValue(t, Guid.NewGuid().ToString("N"), null);
            }
            var mc = new MongoDbProvider(database, collection, ServerName);
            CoreAccess.TryCallAsync(EDomain, mc.Collection.Save, t, WriteConcern.Acknowledged, CallSuccess, mc.Dispose, null, false, ServerName);
        }
        /// <summary>
        /// 插入会失败（若有主键）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="database"></param>
        /// <param name="collection"></param>
        public void InsertMongo<T>(T t, string database, string collection) where T : MongoBaseModel
        {
            var property = typeof(T).GetProperty("Id");
            if (property != null && Equals(property.GetValue(t, null), null))
            {
                property.SetValue(t, Guid.NewGuid().ToString("N"), null);
            }
            var mc = new MongoDbProvider(database, collection, ServerName);
            CoreAccess.TryCallAsync(EDomain, mc.Collection.Insert, t, WriteConcern.Acknowledged, CallSuccess, mc.Dispose, null, false, ServerName);
        }

        /// <summary>
        /// 批量插入会失败（若有主键）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="database"></param>
        /// <param name="collection"></param>
        public void InsertBatchMongo<T>(IEnumerable<T> list, string database, string collection) where T : MongoBaseModel
        {
            var property = typeof(T).GetProperty("Id");
            var enumerable = list as T[] ?? list.ToArray();
            foreach (var t in enumerable.Where(t => property != null && Equals(property.GetValue(t, null), null)))
            {
                property.SetValue(t, Guid.NewGuid().ToString("N"), null);
            }
            var mc = new MongoDbProvider(database, collection, ServerName);
            CoreAccess.TryCallAsync(EDomain, mc.Collection.InsertBatch, enumerable, WriteConcern.Acknowledged, CallSuccess, mc.Dispose, null, false, ServerName);
        }

        public void SaveMongo<T>(Func<T> loader, string database, string collection) where T : MongoBaseModel
        {
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
        public void UpdateMongo(string database, string collection, QueryDocument query, UpdateDocument update, UpdateFlags flag = UpdateFlags.Multi)
        {
            var mc = new MongoDbProvider(database, collection, ServerName);
            CoreAccess.TryCallAsync(EDomain, mc.Collection.Update, query, update, flag, WriteConcern.Acknowledged, CallSuccess, mc.Dispose, null, false, ServerName);
        }

        /// <summary>
        /// 删除MongoDB
        /// </summary>
        /// <param name="database"></param>
        /// <param name="collection"></param>
        /// <param name="query"></param>
        /// <param name="flag"></param>
        /// <returns></returns>
        public void RemoveMongo(string database, string collection, QueryDocument query, RemoveFlags flag = RemoveFlags.None)
        {
            var mc = new MongoDbProvider(database, collection, ServerName);
            CoreAccess.TryCallAsync(EDomain, mc.Collection.Remove, query, flag, WriteConcern.Acknowledged, CallSuccess, mc.Dispose, null, false, ServerName);
        }

        /// <summary>
        /// 清空指定Collection所有数据
        /// </summary>
        /// <param name="database"></param>
        /// <param name="collection"></param>
        /// <returns></returns>
        public void RemoveAllMongo(string database, string collection)
        {
            var mc = new MongoDbProvider(database, collection, ServerName);
            CoreAccess.TryCallAsync(EDomain, mc.Collection.RemoveAll, WriteConcern.Acknowledged, CallSuccess, mc.Dispose, null, false, ServerName);
        }

        /// <summary>
        /// 删除指定Collection
        /// </summary>
        /// <param name="database"></param>
        /// <param name="collection"></param>
        /// <returns></returns>
        public void DropMongo(string database, string collection)
        {
            var mc = new MongoDbProvider(database, collection, ServerName);
            CoreAccess.TryCallAsync(EDomain, mc.Collection.Drop, CallSuccess, mc.Dispose, null, false, ServerName);
        }
        #endregion

        #region 查询操作
        private static MongoCursor<BsonDocument> Find(MongoDbProvider mc, QueryDocument query, FieldsDocument field = null, SortByDocument sortBy = null, int limit = 0, int skip = 0)
        {
            var docs = mc.Collection.Find(query);
            docs = (field != null) ? docs.SetFields(field) : docs;
            docs = (sortBy != null) ? docs.SetSortOrder(sortBy) : docs;
            docs = (limit != 0) ? docs.SetLimit(limit) : docs;
            docs = (skip != 0) ? docs.SetSkip(skip) : docs;
            return docs;
        }

        private static long Count(MongoDbProvider mc, QueryDocument query, int limit = 0, int skip = 0)
        {
            var docs = mc.Collection.Find(query);
            docs = (limit != 0) ? docs.SetLimit(limit) : docs;
            docs = (skip != 0) ? docs.SetSkip(skip) : docs;
            return docs.Count();
        }

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
        /// <returns></returns>
        public MongoCursor<BsonDocument> ReadMongo(string database, string collection, QueryDocument query, FieldsDocument field = null, SortByDocument sortBy = null, int limit = 0, int skip = 0)
        {
            var mc = new MongoDbProvider(database, collection, ServerName);
            return CoreAccess.TryCall(EDomain, Find, mc, query, field, sortBy, limit, skip, iresult => iresult != null && iresult.Any(), mc.Dispose, 1, true, ServerName);
        }

        /// <summary>
        /// 查询MongoDB,取第一条
        /// </summary>
        /// <param name="database"></param>
        /// <param name="collection"></param>
        /// <param name="query">查询语句</param>
        /// <returns></returns>
        public BsonDocument FindOne(string database, string collection, IMongoQuery query)
        {
            var mc = new MongoDbProvider(database, collection, ServerName);
            return CoreAccess.TryCall(EDomain, mc.Collection.FindOne, query, CoreBase.CallSuccess, mc.Dispose, 1, true, ServerName);
        }

        /// <summary>
        /// 取MongoDB条数
        /// </summary>
        /// <param name="database"></param>
        /// <param name="collection"></param>
        /// <param name="query">查询语句</param>
        /// <param name="limit">选取条数</param>
        /// <param name="skip">从索引值开始选取</param>
        /// <returns></returns>
        public long ReadMongoCount(string database, string collection, QueryDocument query, int limit = 0, int skip = 0)
        {
            var mc = new MongoDbProvider(database, collection, ServerName);
            return CoreAccess.TryCall(EDomain, Count, mc, query, limit, skip, CoreBase.CallSuccess, mc.Dispose, 1, true, ServerName);
        }

        /// <summary>
        /// 查询Distinct
        /// </summary>
        /// <param name="database"></param>
        /// <param name="collection"></param>
        /// <param name="query"></param>
        /// <param name="field"></param>
        /// <returns></returns>
        public IEnumerable<BsonValue> Distinct(string database, string collection, QueryDocument query, string field)
        {
            var mc = new MongoDbProvider(database, collection, ServerName);
            return CoreAccess.TryCall(EDomain, mc.Collection.Distinct, field, query, CoreBase.CallSuccess, mc.Dispose, 1, true, ServerName);
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
        public static List<T> ToEntity<T>(MongoCursor<BsonDocument> docs)
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
        public static T ToEntity<T>(BsonDocument doc)
        {
            var document = doc.ToDictionary();
            var result = document.AutoMapper<T>();
            typeof(T).GetProperty("Id").SetValue(result, document["_id"], null);
            return result;
        }
        #endregion
    }
}

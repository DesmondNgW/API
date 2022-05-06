using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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
        public Task<string> CreateIndex(string database, string collection, CreateIndexModel<T> index)
        {
            if (!AppConfig.MongoDbEnable) return default(Task<string>);
            var mc = new MongoDbProvider<T>(database, collection);
            return CoreAccess<IMongoCollection<T>>.TryCallAsync<string, CreateIndexModel<T>, CreateOneIndexOptions, CancellationToken>(mc.Client.Indexes.CreateOne,
                index, null, default(CancellationToken), mc, null, new LogOptions<string>(CallSuccess, true));
        }

        /// <summary>
        /// 删除索引
        /// </summary>
        /// <param name="database"></param>
        /// <param name="collection"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public Task DropIndex(string database, string collection, string name)
        {
            if (!AppConfig.MongoDbEnable) return default(Task);
            var mc = new MongoDbProvider<T>(database, collection);
            return CoreAccess<IMongoCollection<T>>.TryCallAsync(mc.Client.Indexes.DropOne, name, default(CancellationToken), mc, null, new LogOptions(true));
        }

        /// <summary>
        /// 获取所有索引
        /// </summary>
        /// <param name="database"></param>
        /// <param name="collection"></param>
        /// <returns></returns>
        public IAsyncCursor<BsonDocument> IndexList(string database, string collection)
        {
            if (!AppConfig.MongoDbEnable) return default(IAsyncCursor<BsonDocument>);
            var mc = new MongoDbProvider<T>(database, collection);
            return CoreAccess<IMongoCollection<T>>.TryCall(mc.Client.Indexes.List, default(CancellationToken), mc, new LogOptions<IAsyncCursor<BsonDocument>>(CallSuccess, true));
        }
        #endregion

        #region 增删改操作
        /// <summary>
        /// 插入会失败（若有主键）
        /// </summary>
        /// <param name="t"></param>
        /// <param name="database"></param>
        /// <param name="collection"></param>
        /// <returns></returns>
        public Task InsertMongo(T t, string database, string collection)
        {
            if (!AppConfig.MongoDbEnable) return default(Task);
            var property = typeof(T).GetProperty("Id");
            if (property != null && Equals(property.GetValue(t, null), null))
            {
                property.SetValue(t, Guid.NewGuid().ToString("N"), null);
            }
            var mc = new MongoDbProvider<T>(database, collection);
            return CoreAccess<IMongoCollection<T>>.TryCallAsync<T, InsertOneOptions, CancellationToken>(mc.Client.InsertOne, t, null, default(CancellationToken), mc, null, new LogOptions(true));
        }

        /// <summary>
        /// 批量插入会失败（若有主键）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="database"></param>
        /// <param name="collection"></param>
        public Task InsertBatchMongo(IEnumerable<T> list, string database, string collection)
        {
            if (!AppConfig.MongoDbEnable) return default(Task);
            var property = typeof(T).GetProperty("Id");
            var enumerable = list as T[] ?? list.ToArray();
            foreach (var t in enumerable.Where(t => property != null && Equals(property.GetValue(t, null), null)))
            {
                property.SetValue(t, Guid.NewGuid().ToString("N"), null);
            }
            var mc = new MongoDbProvider<T>(database, collection);
            return CoreAccess<IMongoCollection<T>>.TryCallAsync<IEnumerable<T>, InsertManyOptions, CancellationToken>(mc.Client.InsertMany, enumerable, null, default(CancellationToken), mc, null, new LogOptions(true));
        }

        /// <summary>
        /// SaveMongo
        /// </summary>
        /// <param name="loader"></param>
        /// <param name="database"></param>
        /// <param name="collection"></param>
        public void SaveMongo(Func<T> loader, string database, string collection)
        {
            if (!AppConfig.MongoDbEnable) return;
            InsertMongo(loader(), database, collection);
        }

        /// <summary>
        /// 更新MongoDB数据
        /// </summary>
        /// <param name="database"></param>
        /// <param name="collection"></param>
        /// <param name="query"></param>
        /// <param name="update"></param>
        /// <returns></returns>
        public Task<UpdateResult> UpdateMongo(string database, string collection, FilterDefinition<T> filter, UpdateDefinition<T> update)
        {
            if (!AppConfig.MongoDbEnable) return default(Task<UpdateResult>);
            var mc = new MongoDbProvider<T>(database, collection);
            return CoreAccess<IMongoCollection<T>>.TryCallAsync<UpdateResult, FilterDefinition<T>, UpdateDefinition<T>, UpdateOptions, CancellationToken>(mc.Client.UpdateOne,
                filter, update, null, default(CancellationToken), mc, null, new LogOptions<UpdateResult>(CallSuccess, true));
        }

        /// <summary>
        /// 批量更新MongoDB数据
        /// </summary>
        /// <param name="database"></param>
        /// <param name="collection"></param>
        /// <param name="filter"></param>
        /// <param name="update"></param>
        /// <returns></returns>
        public Task<UpdateResult> UpdateBatchMongo(string database, string collection, FilterDefinition<T> filter, UpdateDefinition<T> update)
        {
            if (!AppConfig.MongoDbEnable) return default(Task<UpdateResult>);
            var mc = new MongoDbProvider<T>(database, collection);
            return CoreAccess<IMongoCollection<T>>.TryCallAsync<UpdateResult, FilterDefinition<T>, UpdateDefinition<T>, UpdateOptions, CancellationToken>(mc.Client.UpdateMany,
                filter, update, null, default(CancellationToken), mc, null, new LogOptions<UpdateResult>(CallSuccess, true));
        }

        /// <summary>
        /// 替换MongoDB数据
        /// </summary>
        /// <param name="database"></param>
        /// <param name="collection"></param>
        /// <param name="filter"></param>
        /// <param name="replace"></param>
        /// <returns></returns>
        public Task<ReplaceOneResult> ReplaceMongo(string database, string collection, FilterDefinition<T> filter, T replace)
        {
            if (!AppConfig.MongoDbEnable) return default(Task<ReplaceOneResult>);
            var mc = new MongoDbProvider<T>(database, collection);
            return CoreAccess<IMongoCollection<T>>.TryCallAsync<ReplaceOneResult, FilterDefinition<T>, T, ReplaceOptions, CancellationToken>(mc.Client.ReplaceOne,
                filter, replace, null, default(CancellationToken), mc, null, new LogOptions<ReplaceOneResult>(CallSuccess, true));
        }

        /// <summary>
        /// 删除MongoDB数据
        /// </summary>
        /// <param name="database"></param>
        /// <param name="collection"></param>
        /// <param name="query"></param>
        /// <param name="flag"></param>
        /// <returns></returns>
        public Task<DeleteResult> RemoveMongo(string database, string collection, FilterDefinition<T> filter)
        {
            if (!AppConfig.MongoDbEnable) return default(Task<DeleteResult>);
            var mc = new MongoDbProvider<T>(database, collection);
            return CoreAccess<IMongoCollection<T>>.TryCallAsync(mc.Client.DeleteOne, filter, default(CancellationToken), mc, null, new LogOptions<DeleteResult>(CallSuccess, true));
        }

        /// <summary>
        /// 删除MongoDB数据
        /// </summary>
        /// <param name="database"></param>
        /// <param name="collection"></param>
        /// <returns></returns>
        public Task<DeleteResult> RemoveBatchMongo(string database, string collection, FilterDefinition<T> filter)
        {
            if (!AppConfig.MongoDbEnable) return default(Task<DeleteResult>);
            var mc = new MongoDbProvider<T>(database, collection);
            return CoreAccess<IMongoCollection<T>>.TryCallAsync(mc.Client.DeleteMany, filter, default(CancellationToken), mc, null, new LogOptions<DeleteResult>(CallSuccess, true));
        }
        #endregion

        #region 查询操作
        /// <summary>
        /// 查询MongoDB
        /// </summary>
        /// <param name="database"></param>
        /// <param name="collection"></param>
        /// <param name="filter"></param>
        /// <param name="sortBy"></param>
        /// <param name="limit"></param>
        /// <param name="skip"></param>
        /// <returns></returns>
        public IFindFluent<T, T> Find(string database, string collection, FilterDefinition<T> filter, SortDefinition<T> sortBy = null, int limit = 0, int skip = 0)
        {
            if (!AppConfig.MongoDbEnable) return default(IFindFluent<T, T>);
            var mc = new MongoDbProvider<T>(database, collection);
            var result = CoreAccess<IMongoCollection<T>>.TryCall((ifilter, isortBy, ilimit, iskip) =>
            {
                IFindFluent<T, T> docs = mc.Client.Find(ifilter);
                docs = (isortBy != null) ? docs.Sort(isortBy) : docs;
                docs = (ilimit != 0) ? docs.Limit(ilimit) : docs;
                docs = (iskip != 0) ? docs.Skip(iskip) : docs;
                return docs;
            }, filter, sortBy, limit, skip, mc, new LogOptions<IFindFluent<T, T>>(CallSuccess, true));
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
        public long Count(string database, string collection, FilterDefinition<T> filter)
        {
            if (!AppConfig.MongoDbEnable) return 0;
            var mc = new MongoDbProvider<T>(database, collection);
            return CoreAccess<IMongoCollection<T>>.TryCall<long, FilterDefinition<T>, CountOptions, CancellationToken>(mc.Client.CountDocuments, filter, null, default(CancellationToken),
                mc, new LogOptions<long>(CoreBase.CallSuccess, true));
        }

        /// <summary>
        /// 查询Distinct
        /// </summary>
        /// <param name="database"></param>
        /// <param name="collection"></param>
        /// <param name="query"></param>
        /// <param name="field"></param>
        /// <returns></returns>
        public IAsyncCursor<TField> Distinct<TField>(string database, string collection, FieldDefinition<T, TField> field, FilterDefinition<T> filter)
        {
            if (!AppConfig.MongoDbEnable) return default(IAsyncCursor<TField>);
            var mc = new MongoDbProvider<T>(database, collection);
            return CoreAccess<IMongoCollection<T>>.TryCall<IAsyncCursor<TField>, FieldDefinition<T, TField>, FilterDefinition<T>, DistinctOptions, CancellationToken>
                (mc.Client.Distinct, field, filter, null, default(CancellationToken), mc, new LogOptions<IAsyncCursor<TField>>(CallSuccess, true));
        }
        #endregion

        #region 高级操作
        /// <summary>
        /// Aggregate
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="database"></param>
        /// <param name="collection"></param>
        /// <param name="pipeline"></param>
        /// <returns></returns>
        public IAsyncCursor<TResult> Aggregate<TResult>(string database, string collection, PipelineDefinition<T, TResult> pipeline)
        {
            if (!AppConfig.MongoDbEnable) return default(IAsyncCursor<TResult>);
            var mc = new MongoDbProvider<T>(database, collection);
            return CoreAccess<IMongoCollection<T>>.TryCall<IAsyncCursor<TResult>, PipelineDefinition<T, TResult>, AggregateOptions, CancellationToken>
                (mc.Client.Aggregate, pipeline, null, default(CancellationToken), mc, new LogOptions<IAsyncCursor<TResult>>(CallSuccess, true));
        }

        /// <summary>
        /// AggregateToCollection
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="database"></param>
        /// <param name="collection"></param>
        /// <param name="pipeline"></param>
        /// <returns></returns>
        public Task AggregateToCollection<TResult>(string database, string collection, PipelineDefinition<T, TResult> pipeline)
        {
            if (!AppConfig.MongoDbEnable) return default(Task);
            var mc = new MongoDbProvider<T>(database, collection);
            return CoreAccess<IMongoCollection<T>>.TryCallAsync<PipelineDefinition<T, TResult>, AggregateOptions, CancellationToken>
                (mc.Client.AggregateToCollection, pipeline, null, default(CancellationToken), mc, null, new LogOptions(true));
        }

        /// <summary>
        /// BulkWrite
        /// </summary>
        /// <param name="database"></param>
        /// <param name="collection"></param>
        /// <param name="requests"></param>
        /// <returns></returns>
        public Task<BulkWriteResult<T>> BulkWrite(string database, string collection, IEnumerable<WriteModel<T>> requests)
        {
            if (!AppConfig.MongoDbEnable) return default(Task<BulkWriteResult<T>>);
            var mc = new MongoDbProvider<T>(database, collection);
            return CoreAccess<IMongoCollection<T>>.TryCallAsync<BulkWriteResult<T>, IEnumerable<WriteModel<T>>, BulkWriteOptions, CancellationToken>
                (mc.Client.BulkWrite, requests, null, default(CancellationToken), mc, null, new LogOptions<BulkWriteResult<T>>(CallSuccess, true));
        }

        /// <summary>
        /// Watch
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="database"></param>
        /// <param name="collection"></param>
        /// <param name="pipeline"></param>
        /// <returns></returns>
        public Task<IChangeStreamCursor<TResult>> Watch<TResult>(string database, string collection, PipelineDefinition<ChangeStreamDocument<T>, TResult> pipeline)
        {
            if (!AppConfig.MongoDbEnable) return default(Task<IChangeStreamCursor<TResult>>);
            var mc = new MongoDbProvider<T>(database, collection);
            return CoreAccess<IMongoCollection<T>>.TryCallAsync<IChangeStreamCursor<TResult>, PipelineDefinition<ChangeStreamDocument<T>, TResult>, ChangeStreamOptions, CancellationToken>
                (mc.Client.Watch, pipeline, null, default(CancellationToken), mc, null, new LogOptions<IChangeStreamCursor<TResult>>(CallSuccess, true));
        }
        #endregion

        #region ToEntity & CallSuccess
        private static bool CallSuccess(string result)
        {
            return !string.IsNullOrEmpty(result) && !string.IsNullOrWhiteSpace(result);
        }

        private static bool CallSuccess(IAsyncCursor<BsonDocument> result)
        {
            return result != null && result.Current != null && result.Current.Any();
        }

        private static bool CallSuccess<TField>(IAsyncCursor<TField> result)
        {
            return result != null && result.Current != null && result.Current.Any();
        }

        private static bool CallSuccess(UpdateResult result)
        {
            return result != null && result.IsAcknowledged&& result.IsModifiedCountAvailable;
        }

        private static bool CallSuccess(ReplaceOneResult result)
        {
            return result != null && result.IsAcknowledged && result.IsModifiedCountAvailable;
        }

        private static bool CallSuccess(DeleteResult result)
        {
            return result != null && result.IsAcknowledged;
        }

        private static bool CallSuccess(IFindFluent<T, T> result)
        {
            return result != null && result.Any();
        }

        private static bool CallSuccess(BulkWriteResult<T> result)
        {
            return result != null && result.IsAcknowledged && result.IsModifiedCountAvailable;
        }

        /// <summary>
        /// MongoDB数据转为为实体List
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="docs"></param>
        /// <returns></returns>
        public static List<T> ToEntity(IFindFluent<T, T> docs)
        {
            return ToEntity(docs.ToCursor());
        }

        /// <summary>
        /// MongoDB数据转为为实体List（需要实体和MongoDB字段一一对应）
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public static List<T> ToEntity(IAsyncCursor<BsonDocument> result)
        {
            var list = default(List<T>);
            if (result != null && result.Current != null)
            {
                list = (from current in result.Current select ToEntity(current)).ToList();
            }
            return list;
        }

        /// <summary>
        /// MongoDB数据转为为实体List
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public static List<T> ToEntity(IAsyncCursor<T> result)
        {
            var list = default(List<T>);
            if (result != null && result.Current != null)
            {
                list = (from current in result.Current select current).ToList();
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

using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace X.Util.Entities.Interface
{
    public interface IMongoDbBase<T> where T : MongoBaseModel
    {
        Task<string> CreateIndex(string database, string collection, CreateIndexModel<T> index);

        Task DropIndex(string database, string collection, string index);

        IAsyncCursor<BsonDocument> IndexList(string database, string collection);

        Task InsertMongo(T t, string database, string collection);

        Task InsertBatchMongo(IEnumerable<T> list, string database, string collection);

        void SaveMongo(Func<T> loader, string database, string collection);

        Task<UpdateResult> UpdateMongo(string database, string collection, FilterDefinition<T> filter, UpdateDefinition<T> update);

        Task<UpdateResult> UpdateBatchMongo(string database, string collection, FilterDefinition<T> filter, UpdateDefinition<T> update);

        Task<ReplaceOneResult> ReplaceMongo(string database, string collection, FilterDefinition<T> filter, T replace);

        Task<DeleteResult> RemoveMongo(string database, string collection, FilterDefinition<T> filter);

        Task<DeleteResult> RemoveBatchMongo(string database, string collection, FilterDefinition<T> filter);

        IFindFluent<T, T> Find(string database, string collection, FilterDefinition<T> filter, SortDefinition<T> sortBy = null, int limit = 0, int skip = 0);

        long Count(string database, string collection, FilterDefinition<T> filter);

        IAsyncCursor<TField> Distinct<TField>(string database, string collection, FieldDefinition<T, TField> field, FilterDefinition<T> filter);

        IAsyncCursor<TResult> Aggregate<TResult>(string database, string collection, PipelineDefinition<T, TResult> pipeline);

        Task AggregateToCollection<TResult>(string database, string collection, PipelineDefinition<T, TResult> pipeline);

        Task<BulkWriteResult<T>> BulkWrite(string database, string collection, IEnumerable<WriteModel<T>> requests);

        Task<IChangeStreamCursor<TResult>> Watch<TResult>(string database, string collection, PipelineDefinition<ChangeStreamDocument<T>, TResult> pipeline);
    }
}

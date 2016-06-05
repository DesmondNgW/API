using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Driver;

namespace X.Util.Entities.Interface
{
    public interface IMongoDbBase<T> where T : MongoBaseModel
    {
        void CreateIndex(string database, string collection, IndexKeysDocument index);

        void DropIndex(string database, string collection, IndexKeysDocument index);

        bool IndexExists(string database, string collection, IndexKeysDocument index);

        IndexKeysDocument GetAllIndexes(string database, string collection);

        void SaveMongo(T t, string database, string collection);

        void InsertMongo(T t, string database, string collection);

        void InsertBatchMongo(IEnumerable<T> list, string database, string collection);

        void SaveMongo(Func<T> loader, string database, string collection);

        void UpdateMongo(string database, string collection, QueryDocument query, UpdateDocument update, UpdateFlags flag = UpdateFlags.Multi);

        void RemoveMongo(string database, string collection, QueryDocument query, RemoveFlags flag = RemoveFlags.None);

        MongoCursor<T> ReadMongo(string database, string collection, QueryDocument query, FieldsDocument field = null, SortByDocument sortBy = null, int limit = 0, int skip = 0);

        T FindOne(string database, string collection, IMongoQuery query);

        long ReadMongoCount(string database, string collection, QueryDocument query, int limit = 0, int skip = 0);

        IEnumerable<BsonValue> Distinct(string database, string collection, QueryDocument query, string field);
    }
}

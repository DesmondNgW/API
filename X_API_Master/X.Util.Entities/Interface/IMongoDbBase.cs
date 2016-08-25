using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Driver;

namespace X.Util.Entities.Interface
{
    public interface IMongoDbBase<T> where T : MongoBaseModel
    {
        void CreateIndex(string database, string collection, string credentialDataBase, IndexKeysDocument index);

        void DropIndex(string database, string collection, string credentialDataBase, IndexKeysDocument index);

        bool IndexExists(string database, string collection, string credentialDataBase, IndexKeysDocument index);

        IndexKeysDocument GetAllIndexes(string database, string collection, string credentialDataBase);

        void SaveMongo(T t, string database, string collection, string credentialDataBase);

        void InsertMongo(T t, string database, string collection, string credentialDataBase);

        void InsertBatchMongo(IEnumerable<T> list, string database, string credentialDataBase, string collection);

        void SaveMongo(Func<T> loader, string database, string collection, string credentialDataBase);

        void UpdateMongo(string database, string collection, string credentialDataBase, QueryDocument query, UpdateDocument update, UpdateFlags flag = UpdateFlags.Multi);

        void RemoveMongo(string database, string collection, string credentialDataBase, QueryDocument query, RemoveFlags flag = RemoveFlags.None);

        MongoCursor<T> ReadMongo(string database, string collection, string credentialDataBase, QueryDocument query, FieldsDocument field = null, SortByDocument sortBy = null, int limit = 0, int skip = 0);

        T FindOne(string database, string collection, string credentialDataBase, IMongoQuery query);

        long ReadMongoCount(string database, string collection, string credentialDataBase, QueryDocument query, int limit = 0, int skip = 0);

        IEnumerable<BsonValue> Distinct(string database, string collection, string credentialDataBase, QueryDocument query, string field);
    }
}

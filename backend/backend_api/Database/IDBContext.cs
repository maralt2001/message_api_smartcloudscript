using MongoDB.Driver;
using MongoDB.Bson;
using System.Threading.Tasks;
using System;
using backend_api.Database.Helpers;
using System.Collections.Generic;

namespace backend_api.Database
{
    public interface IDBContext
    {
        IMongoDatabase Database { get; set; }
        MongoClient MongoClient { get; set; }

        Task<bool> InsertRecordAsync<T>(string collectionName, T record);
        Task<List<T>> LoadRecordsAsync<T>(string collectionName);
        Task<T> LoadRecordAsync<T>(string collectionName, string id);
        Task<bool> UpdateRecordAsync<T>(string collectionName, string id, T record);
        Task<bool> DeleteRecordAsync<T>(string collectionName, string id);
        Task<bool> IsConnectionUp (int secondToWait = 1);
        Task<int> BulkInsert<T> (List<T> data, string collectionName);
    }

        
}
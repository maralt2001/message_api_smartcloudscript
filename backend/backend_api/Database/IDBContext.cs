using MongoDB.Driver;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace backend_api.Database
{
    public interface IDBContext
    {
        IMongoDatabase Database { get; set; }
        MongoClient MongoClient { get; set; }

        Task<bool> InsertRecordAsync<T>(string collectionName, T record);
        Task<List<T>> LoadRecordsAsync<T>(string collectionName);
        Task<T> LoadRecordAsync<T>(string collectionName, string fieldKey, string fieldValue);
        Task<T> LoadRecordAsync<T>(string collectionName, string id);
        Task<bool> UpdateRecordAsync<T>(string collectionName, string id, T record);
        Task<bool> DeleteRecordAsync<T>(string collectionName, string id);
        Task<bool> IsConnectionUp (int secondToWait = 1);
        Task<int> BulkInsert<T> (List<T> data, string collectionName);
        Task<string> CreateIndex<T>(string collectionName, string indexKey);
        Task<bool> DropIndex<T>(string collectionName, string indexKey);
    }

        
}
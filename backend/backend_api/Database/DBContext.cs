using MongoDB.Driver;
using MongoDB.Bson;
using System.Threading.Tasks;
using System;
using backend_api.Database.Helpers;
using System.Collections.Generic;
using System.Linq;
using backend_api.Model;
using System.Runtime.InteropServices;
using MongoDB.Bson.IO;
using backend_api.Vault;
using backend_api.Vault.Models;

namespace backend_api.Database
{
    public abstract class DBContext : IDBContext
    {
        public MongoClient MongoClient { get; set; }
        public IMongoDatabase Database { get; set; }


        public async Task<bool> InsertRecordAsync<T>(string collectionName, T record)
        {
            try
            {
                IMongoCollection<T> collection = Database.GetCollection<T>(collectionName);
                await collection.InsertOneAsync(record).ConfigureAwait(false);
                return true;
            }
            catch (Exception)
            {

                return false;
            }
        }

        public async Task<List<T>> LoadRecordsAsync<T>(string collectionName)
        {
            IMongoCollection<T> collection = Database.GetCollection<T>(collectionName);
            return await collection.FindAsync(new BsonDocument()).Result.ToListAsync().ConfigureAwait(false);

        }

        public async Task<T> LoadRecordAsync<T>(string collectionName, string fieldKey, string fieldValue)
        {
            IMongoCollection<T> collection = Database.GetCollection<T>(collectionName);
            FilterDefinition<T> filter = Builders<T>.Filter.Eq(fieldKey, fieldValue);
            return await collection.FindAsync(filter).Result.FirstOrDefaultAsync().ConfigureAwait(false);
        }

        public async Task<bool> UpdateRecordAsync<T>(string collectionName, string id, T record)
        {
            BsonDocument bson = await BsonFactory.GetSetDocumentAsync(record);

            var filter = Builders<T>.Filter.Eq("_id", id);
            try
            {
                IMongoCollection<T> collection = Database.GetCollection<T>(collectionName);
                var result = await collection.UpdateOneAsync(filter, bson).ConfigureAwait(false);
                return true;

            }
            catch (Exception)
            {

                return false;
            }
        }

        public async Task<T> LoadRecordAsync<T>(string collectionName, string id)
        {
            try
            {

                IMongoCollection<T> collection = Database.GetCollection<T>(collectionName);
                FilterDefinition<T> filter = Builders<T>.Filter.Eq("_id", ObjectId.Parse(id));
                return await collection.FindAsync(filter).Result.FirstOrDefaultAsync().ConfigureAwait(false);

            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<int> CountDocumentsAsync<T> (string collectionName, string fieldKey, string fieldValue)
        {
            IMongoCollection<T> collection = Database.GetCollection<T>(collectionName);
            FilterDefinition<T> filter = Builders<T>.Filter.Eq(fieldKey, fieldValue);
            var result = Convert.ToInt32(await collection.CountDocumentsAsync(filter).ConfigureAwait(false));
            return result;
        }

        public async Task<bool> DeleteRecordAsync<T>(string collectionName, string id)
        {
            try
            {
                IMongoCollection<T> collection = Database.GetCollection<T>(collectionName);
                var filter = Builders<T>.Filter.Eq("_id", id);
                var result = await collection.DeleteOneAsync(filter).ConfigureAwait(false);
                if (result.DeletedCount > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<bool> IsConnectionUp(int secondToWait = 1)
        {

            if (secondToWait <= 0)
            {
                throw new ArgumentOutOfRangeException("secondToWait", secondToWait, "Must be at least 1 second");
            }
            else
            {
                Task<bool> result = Task.Run(() =>
                    {
                        return Database.RunCommandAsync((Command<BsonDocument>)"{ping:1}").Wait(secondToWait * 1000);
                    });

                return await result.ConfigureAwait(false);

            }
        }

        public async Task<int> BulkInsert<T>(List<T> data, string collectionName)
        {

            var bulkOps = new List<WriteModel<T>>();

            if (data.Count > 0)
            {

                foreach (var chunk in data)
                {
                    var upsertOne = new InsertOneModel<T>(chunk);
                    bulkOps.Add(upsertOne);

                }
                IMongoCollection<T> collection = Database.GetCollection<T>(collectionName);

                var writeresult = await collection.BulkWriteAsync(bulkOps);
                return Convert.ToInt32(writeresult.InsertedCount);
            }
            else
            {
                return 0;
            }


        }

        public async Task<string> CreateIndex<T>(string collectionName, string indexKey)
        {
            var collection = Database.GetCollection<T>(collectionName);
            IndexKeysDefinition<T> keys = new BsonDocument { { indexKey, 1 } };
            CreateIndexOptions options = new CreateIndexOptions { Name = indexKey, Unique = true };

            var indexModel = new CreateIndexModel<T>(keys, options);
            var result = await collection.Indexes.CreateOneAsync(indexModel);
            return result;
        }

        public async Task<bool> DropIndex<T>(string collectionName, string indexName)
        {
            var collection = Database.GetCollection<T>(collectionName);

            try
            {
                await collection.Indexes.DropOneAsync(indexName);
                return true;
            }
            catch (Exception)
            {
                return false;
            }

        }


    }

    public class MongoWithCredential : DBContext
    {
        public MongoWithCredential(string databaseName, string databaseUrl, string user, string password)
        {

            MongoCredential credential = MongoCredential.CreateCredential(databaseName, user, password);
            MongoClientSettings settings = new MongoClientSettings
            {
                Credential = credential,
                Server = new MongoServerAddress(databaseUrl)
            };
            MongoClient = new MongoClient(settings);
            Database = MongoClient.GetDatabase(databaseName);


        }

    }

   public class MongoWithCredentialVault : DBContext
    {
        
        public MongoWithCredentialVault(string databaseName, string databaseUrl, string user, string password)
        {


            MongoCredential credential = MongoCredential.CreateCredential(databaseName, user, password);
            MongoClientSettings settings = new MongoClientSettings
            {
                Credential = credential,
                Server = new MongoServerAddress(databaseUrl)
            };
            MongoClient = new MongoClient(settings);
            Database = MongoClient.GetDatabase(databaseName);
        }
        
    }
    

    public class MongoLocalDB : DBContext
    {
        public MongoLocalDB(string databaseName)
        {
           
            MongoUrl url = new MongoUrl("mongodb://localhost:27017");
            MongoClient = new MongoClient(url);
            Database = MongoClient.GetDatabase(databaseName);
        }
    }
}
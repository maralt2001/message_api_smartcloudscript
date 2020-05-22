using MongoDB.Bson;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace backend_api.Database.Helpers
{
   
        public static class BsonFactory
        {
            public async static Task<BsonDocument> GetSetDocumentAsync<T>(T record)
            {
                var result = Task.Run(() =>

                {
                    var serializerSettings = new JsonSerializerSettings()
                    {
                        NullValueHandling = NullValueHandling.Ignore,
                        DefaultValueHandling = DefaultValueHandling.Ignore
                    };
                    BsonDocument bson = new BsonDocument() { { "$set", BsonDocument.Parse(JsonConvert.SerializeObject(record, serializerSettings)) } };
                    return bson;

                });

                return await result;


            }


        }
}
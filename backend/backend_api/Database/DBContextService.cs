using backend_api.Vault.Models;
using KeyVault;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;

namespace backend_api.Database
{
    public class DBContextService : IDBContextService
    {

        private MongoDBClient MongoDBClient;
        private readonly IConfiguration _configuration;
        public DBContextService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public DBContext GetDBContext(string environment, IVaultProvider vaultProvider = default)
        {
            var dbName = _configuration.GetValue<string>("DBConnection:DB");
            var dbPath = _configuration.GetValue<string>("DBConnection:Path");
            var vaultPolicy = _configuration.GetValue<string>("VaultSettings:Backend_DB_Policy");

            if (this.MongoDBClient == null)
            {
                

                if (environment == "Production" && vaultProvider == default)
                {
                    MongoDBClient = new MongoDBClient(dbName, dbPath, "web", "db");
                    return MongoDBClient;
                }
                if (environment == "Production" && vaultProvider != default)
                {
                    var client = vaultProvider.GetAppVaultClientAsync(vaultPolicy).Result;
                    var result = vaultProvider.GetSecretAsync<DBLoginFromVault>(client, "db/login/").Result;
                    MongoDBClient = new MongoDBClient(dbName, dbPath, result.Data.user, result.Data.password);
                    return MongoDBClient;
                }

                else
                {
                    MongoDBClient = new MongoDBClient("WebDB");
                    return MongoDBClient;
                }
            }
            else
            {
                return MongoDBClient;
            }
        }
    }
}

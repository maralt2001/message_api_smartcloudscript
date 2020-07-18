using backend_api.Database;
using backend_api.Vault;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VaultSharp;
using VaultSharp.V1.Commons;


namespace backend_api.Extensions
{
    public static class MongoServiceExtension
    {
        
        public static void AddMongoClient(this IServiceCollection services, IConfiguration config, IWebHostEnvironment env)
        {
            
            if(env.EnvironmentName == "Development")
            {
                services.AddSingleton<IDBContext>(sp => new MongoLocalDB(
                    config.GetSection("DBConnection").GetSection("DB").Value
                ));

                
            }
            else
            {
                services.AddSingleton<IDBContext>(sp => new MongoWithCredential(
                     config.GetSection("DBConnection").GetSection("DB").Value,
                     config.GetSection("DBConnection").GetSection("Path").Value,
                     "web",
                     "db"
                    ));

                
            }
            
        }

        
    }
}
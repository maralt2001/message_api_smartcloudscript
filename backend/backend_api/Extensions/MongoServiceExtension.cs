using backend_api.Database;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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
                    "db"));
            }
            
        }
    }
}
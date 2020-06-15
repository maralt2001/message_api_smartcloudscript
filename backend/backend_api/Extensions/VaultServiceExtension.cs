using backend_api.Database;
using backend_api.Vault;
using backend_api.Vault.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace backend_api.Extensions
{
    public static class VaultServiceExtension
    {

        public static void AddVaultApp(this IServiceCollection services, IConfiguration config, IWebHostEnvironment env)
        {

            if (env.EnvironmentName == "Development")
            {
                var host = config.GetSection("VaultTokens").GetSection("DevHost").Value;
                services.AddSingleton<IVaultContext>(sp => new VaultApp(config.GetSection("VaultTokens").GetSection("DevToken").Value, host));
                
            }
            else
            {
                var host = config.GetSection("VaultTokens").GetSection("ProdHost").Value;
                services.AddSingleton<IVaultContext>(sp => new VaultApp(config.GetSection("VaultTokens").GetSection("ProdToken").Value, host));
                

            }

        }

        public static async void UseVaultTemp(this IApplicationBuilder builder)
        {
            var vaultContext = (IVaultContext)builder.ApplicationServices.GetService(typeof(IVaultContext));

            var tempClient = new TempVaultContext(vaultContext, "backenddb", vaultContext.GetVaultHost());
            var result = await tempClient.GetSecret("db/login", "secret");
            if (result.Data.ContainsKey("password") && result.Data.ContainsKey("user"))
            {

                var dblogin = new DbLogin { User = result.Data["user"].ToString(), Password = result.Data["password"].ToString() };
                MongoWithCredentialVault.password = dblogin.Password;
                MongoWithCredentialVault.user = dblogin.User;
            }
        }
        
    }
}

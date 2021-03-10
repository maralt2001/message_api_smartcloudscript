using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using KeyVault;

namespace backend_api.Extensions
{
    public static class VaultServiceExtension
    {
        public static void AddVaultAdminClient(this IServiceCollection services, IConfiguration config, IWebHostEnvironment env)
        {
            if (env.EnvironmentName == "Development")
            {
                services.AddSingleton<IVaultProvider>(sp => new VaultProvider(
                    config.GetValue<string>("VaultSettings:DevBaseUrl"),
                    config.GetValue<string>("VaultTokens:DevToken")
                    ));
                   
            }
            else
            {
                services.AddSingleton<IVaultProvider>(sp => new VaultProvider(
                    config.GetValue<string>("VaultSettings:ProdBaseUrl"),
                    config.GetValue<string>("VaultTokens:ProdToken")
                    ));
            }
        }
    }
}

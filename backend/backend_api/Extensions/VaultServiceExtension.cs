using backend_api.Vault;
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
                    services.AddSingleton<IVaultContext>(sp => new VaultApp("s.cd2HXfpEBeqBIitqMpiNySFv","http://localhost:8200"));
                }
                else
                {
                    services.AddSingleton<IVaultContext>(sp => new VaultApp("s.cd2HXfpEBeqBIitqMpiNySFv", "http://vault:8200"));
            }

            }
        
    }
}

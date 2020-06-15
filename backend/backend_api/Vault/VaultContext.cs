using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using VaultSharp;
using VaultSharp.V1.AuthMethods;
using VaultSharp.V1.AuthMethods.Token;
using VaultSharp.V1.Commons;
using VaultSharp.V1.SystemBackend;

namespace backend_api.Vault
{
    public abstract class VaultContext : IVaultContext
    {
        public IVaultClient VaultClient { get; set; }
        public string Host { get; set; }
        
        public string GetVaultHost()
        {
            return this.Host;
        }
        public async Task<ListInfo> GetPathInfo(string mountpoint, string path)
        {
            try
            {
                Secret<ListInfo> secret = await VaultClient.V1.Secrets.KeyValue.V1.ReadSecretPathsAsync(path, mountpoint);
                //var x = await VaultClient.V1.Secrets.KeyValue.V1.ReadSecretAsync("db/login/", "secret");
                return secret.Data;
            }
            catch (Exception)
            {

                return new ListInfo();
            }
        }

        public async Task<Secret<object>> GetAppToken(string policy)
        {
            CreateTokenRequest request = new CreateTokenRequest

            {
                Renewable = true,
                Policies = new List<string> { policy },
                TimeToLive = "1h"
            };
            Secret<object> tokenData = await VaultClient.V1.Auth.Token.CreateTokenAsync(request);
            return tokenData;
        }
      

    }



    public class VaultApp : VaultContext
    {
        public VaultApp(string vaultToken, string vaultServer)
        {
            this.Host = vaultServer;
            IAuthMethodInfo authMethod = new TokenAuthMethodInfo(vaultToken);
            VaultClientSettings vaultClientSettings = new VaultClientSettings(vaultServer, authMethod);
            VaultClient = new VaultClient(vaultClientSettings);
        }

        
    }

    
}

using backend_api.Vault.Models;
using Microsoft.AspNetCore.DataProtection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VaultSharp;
using VaultSharp.V1.AuthMethods;
using VaultSharp.V1.AuthMethods.Token;
using VaultSharp.V1.Commons;

namespace backend_api.Vault
{
    public class TempVaultContext
    {
        public readonly IVaultContext _vaultContext;
        public string _Token { get; private set; }
        public string _VaultServer { get; private set; }
        public IVaultClient VaultClient { get; private set; }

        public TempVaultContext(IVaultContext vaultContext, string Policy, string VaultServer)
        {
            _vaultContext = vaultContext;
            _VaultServer = VaultServer;
            GetTempToken(Policy);
            CreateTempVaultClient();
            
        }

        public void GetTempToken(string policy)
        {
            var result= _vaultContext.GetAppToken(policy).Result;
            _Token =  result.AuthInfo.ClientToken;
        }

        public void CreateTempVaultClient()
        {
            IAuthMethodInfo authMethod = new TokenAuthMethodInfo(this._Token);
            VaultClientSettings vaultClientSettings = new VaultClientSettings(this._VaultServer, authMethod);
            VaultClient = new VaultClient(vaultClientSettings);
        }

        public async Task<Secret<Dictionary<string,object>>> GetSecret(string path, string mountpoint)
        {
             return await this.VaultClient.V1.Secrets.KeyValue.V1.ReadSecretAsync(path, mountpoint);
            

        }
    }
}

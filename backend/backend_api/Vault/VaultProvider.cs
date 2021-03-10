
using backend_api.Vault.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using VaultSharp;
using VaultSharp.Core;
using VaultSharp.V1.AuthMethods;
using VaultSharp.V1.AuthMethods.Token;
using VaultSharp.V1.Commons;

namespace KeyVault
{
    public class VaultProvider : IVaultProvider
    {

        public string BaseUrl { get; private set; }

        public bool Sealed { get; private set; }

        private string AccessToken { get; set; }

        public VaultClient AdminVaultClient { get; private set; }

        public VaultClient AppVaultClient { get; private set; }





        public VaultProvider(string VaultBaseUrl, string VaultAccessToken)
        {
            BaseUrl = VaultBaseUrl;
            AccessToken = VaultAccessToken;
            SetupVaultClient();

        }

        private void SetupVaultClient()
        {
            
            IAuthMethodInfo authMethod = new TokenAuthMethodInfo(AccessToken);
            var vaultClientSettings = new VaultClientSettings(BaseUrl, authMethod);
            AdminVaultClient = new VaultClient(vaultClientSettings);
        }

        public async Task<bool> CheckAdminAuthentication()
        {
            try
            {
                var lookup = await AdminVaultClient.V1.Auth.Token.LookupSelfAsync();
                return true;

            }
            catch (VaultApiException e)
            {
                return !e.ApiErrors.Contains<string>("permission denied");

            }
        }

        public async Task<bool> CheckConnection()
        {
            try
            {
                using var client = new HttpClient();
                var response = await client.GetAsync(BaseUrl + "/v1/sys/seal-status");
                var content = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<VaultSealed>(content);

                if (result.initialized && !result.@sealed)
                {
                    Sealed = false;
                    return true;
                }
                else
                {
                    Sealed = true;
                    return false;
                }

            }
            catch (HttpRequestException)
            {
                Sealed = true;
                return false;
            }

        }

        public async Task<VaultClient> GetAppVaultClientAsync(string Policy)
        {
            IAuthMethodInfo authMethod = new TokenAuthMethodInfo(await GetAppTokenAsync(Policy));
            var vaultClientSettings = new VaultClientSettings(BaseUrl, authMethod);
            AppVaultClient = new VaultClient(vaultClientSettings);
            return AppVaultClient;
        }

        private async Task<string> GetAppTokenAsync(string Policy)
        {
            CreateTokenRequest request = new CreateTokenRequest

            {
                Renewable = true,
                Policies = new List<string> { Policy },
                TimeToLive = "1h"
            };
            Secret<object> tokenData = await AdminVaultClient.V1.Auth.Token.CreateTokenAsync(request);
            return tokenData.AuthInfo.ClientToken;

        }

        public async Task<Secret<T>> GetSecretAsync<T>(VaultClient Client, string Path, string Mountpoint = "secret")
        {
            try
            {
                // path:db/login/ mountpoint:secret
                var secret = await Client.V1.Secrets.KeyValue.V1.ReadSecretAsync<T>(Path, Mountpoint);
                return secret;
            }
            catch (VaultApiException e)
            {
                Console.WriteLine(e.Message);
                return default;
            }
        }


    }
}

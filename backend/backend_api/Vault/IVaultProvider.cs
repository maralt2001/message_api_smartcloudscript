using System.Threading.Tasks;
using VaultSharp;
using VaultSharp.V1.Commons;

namespace KeyVault
{
    public interface IVaultProvider
    {
        VaultClient AppVaultClient { get; }
        string BaseUrl { get; }
        VaultClient AdminVaultClient { get; }

        Task<bool> CheckAdminAuthentication();
        Task<bool> CheckConnection();
        Task<VaultClient> GetAppVaultClientAsync(string Policy);
        Task<Secret<T>> GetSecretAsync<T>(VaultClient Client, string Path, string Mountpoint = "secret");
    }
}
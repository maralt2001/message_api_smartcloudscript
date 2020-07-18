using backend_api.Vault.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using VaultSharp.V1.Commons;

namespace backend_api.Vault
{
    public interface IVaultAccess
    {
        Task<ListInfo> GetPathInfo(string mountpoint, string path);
        Task<Secret<DBLoginFromVault>> GetSecret(string mountpoint, string path);
    }
}
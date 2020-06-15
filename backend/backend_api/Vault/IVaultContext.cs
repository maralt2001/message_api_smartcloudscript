using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VaultSharp.V1.Commons;

namespace backend_api.Vault
{
    public interface IVaultContext
    {
        string GetVaultHost();
        Task<ListInfo> GetPathInfo(string mountpoint, string path);
        Task<Secret<object>> GetAppToken(string policy);

        

    }
}

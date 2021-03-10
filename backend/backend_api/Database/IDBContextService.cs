using KeyVault;
using MongoDB.Driver;

namespace backend_api.Database
{
    public interface IDBContextService
    {
        
        DBContext GetDBContext(string environment, IVaultProvider vaultProvider = default);
    }
}
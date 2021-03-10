using backend_api.Database;
using backend_api.Vault.Models;
using KeyVault;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace backend_api.Controllers
{
    [ApiController]
    public class DevController : Controller
    {
        private readonly IVaultProvider _vaultProvider;
        private readonly IDBContextService _dbContextService;
        public static string hostEnvironment;
        
        public DevController(IVaultProvider vaultProvider, IDBContextService dbContextService)
        {
            _vaultProvider = vaultProvider;
            _dbContextService = dbContextService;
            
        }

        [HttpGet]
        [Route("/api/dev/checkconnections")]
        public async Task<IActionResult> CheckVaultConnection()
        {
            
            var db = _dbContextService.GetDBContext(hostEnvironment,_vaultProvider) ;
            var check = await db.IsConnectionUp(5);
            
            var result = await _vaultProvider.CheckConnection();
            return Ok(new {Connected_To_Vault = result, Connected_To_DB = check});
        }

        [HttpGet]
        [Route("/api/dev/dbcredentials")]
        public async Task<IActionResult> GetDBCredentials()
        {
            var vaultclient = await _vaultProvider.GetAppVaultClientAsync("backenddb");
            var result = await _vaultProvider.GetSecretAsync<DBLoginFromVault>(vaultclient, "db/login/");
            return Ok(result.Data);
            
        }


    }
}

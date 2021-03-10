using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using App.Metrics.Formatters.Prometheus;
using backend_api.Database;
using backend_api.MetricsDefinition;
using backend_api.Model;
using KeyVault;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;



namespace backend_api.Controllers
{

    [ApiController]
    public class AuthController : ControllerBase
    {
        
        public Stopwatch _stopwatch = new Stopwatch();
        private readonly IConfiguration _configuration;
        private readonly IVaultProvider _vaultprovider;
        private readonly IDBContextService _dbContextService;
        public static string hostEnvironment;

        public AuthController(IConfiguration configuration, IVaultProvider vaultProvider, IDBContextService dbContextService)
        {
            
            _configuration = configuration;
            _vaultprovider = vaultProvider;
            _dbContextService = dbContextService;


        }

        [HttpPost]
        [Route("/api/admin/backend/login")]

        public async Task<IActionResult> LoginBackendAdmin([FromBody] BackendAdmin backendAdmin)
        {
            _stopwatch.Start();
            var _db = _dbContextService.GetDBContext(hostEnvironment, _vaultprovider);

            BackendAdmin admin = await _db.LoadRecordAsync<BackendAdmin>("BackendAdmins", "email", backendAdmin.Email);

            void OnHit()
            {
                _stopwatch.Stop();
                MetricsRegistry.LoginRequestHistogram.Observe(_stopwatch.Elapsed.TotalMilliseconds);
                MetricsRegistry.ProcessedJobCount.Inc();

            }


            if (admin != null)
            {
                PasswordVerificationResult result = await admin.PasswordVerification(admin.Password, backendAdmin.Password);

                if(result.ToString() == "Success")
                {
                    var token = await admin.CreateJWTAsync(
                        _configuration.GetValue<string>("TokenValidation:Issuer"), 
                        _configuration.GetValue<string>("TokenValidation:Audience"),
                        _configuration.GetValue<string>("TokenValidation:Symsec"),
                        1);
                    MetricsRegistry.BackendLoginRequestSuccess.Inc();                
                    OnHit();
                    return Ok(new { your_token = token });
                }
                else
                {
                    MetricsRegistry.BackendLoginRequestFailed.Inc();
                    OnHit();
                    return BadRequest(new { state = "Login failed" });
                }
            }
            else
            {
                MetricsRegistry.BackendLoginRequestFailed.Inc();
                OnHit();
                return BadRequest(new { state = "Login failed" });
            }

        }

        [HttpPost]
        [Route("/api/admin/backend/register")]
        [Produces("application/json")]

        public async Task<IActionResult> RegisterBackendAdmin([FromBody] BackendAdmin backendAdmin)
        {
            var _db = _dbContextService.GetDBContext(hostEnvironment, _vaultprovider);
            var check = await _db.LoadRecordAsync<BackendAdmin>("BackendAdmins", "email", backendAdmin.Email);
            if(check == null)
            {
                check = new BackendAdmin();
            }
            if(check.Email != backendAdmin.Email)

            {
                var admin = new BackendAdmin(backendAdmin.Email, backendAdmin.Password, backendAdmin.Active);
                var register = Task.Run(() => {

                    var result = _db.InsertRecordAsync("BackendAdmins", admin);
                    return result;
                });

                if (await register)
                {
                    MetricsRegistry.BackendRegisterRequestSuccess.Inc();
                    return Ok(new { state = "Success Register" });
                }
                else
                {
                    MetricsRegistry.BackendRegisterRequestFailed.Inc();
                    return BadRequest(new { state = "Register failed" });
                }
            }

            else
            {
                MetricsRegistry.BackendRegisterRequestFailed.Inc();
                return BadRequest(new { state = "Register failed" });
            }
          
            
            

        }
    }
}

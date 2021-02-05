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
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Driver;
using Newtonsoft.Json;
using Prometheus;


namespace backend_api.Controllers
{

    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IDBContext _db;
        public Stopwatch _stopwatch = new Stopwatch();

        public AuthController(IDBContext db)
        {
            _db = db;
            
            
            
        }

        [HttpPost]
        [Route("/api/admin/backend/login")]

        public async Task<IActionResult> LoginBackendAdmin([FromBody] BackendAdmin backendAdmin)
        {
            _stopwatch.Start();

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
                    var token = await admin.CreateJWTAsync("login", "smartcloudscript.de", "halloWelthalloWelthalloWelt", 1);
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

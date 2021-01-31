using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using App.Metrics;
using backend_api.Database;
using backend_api.Metrics;
using backend_api.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Driver;
using Newtonsoft.Json;

namespace backend_api.Controllers
{

    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IDBContext _db;
        private readonly IMetrics _metrics;

        public AuthController(IDBContext db, IMetrics metrics)
        {
            _db = db;
            _metrics = metrics;
        }

        [HttpPost]
        [Route("/api/admin/backend/login")]

        public async Task<IActionResult> LoginBackendAdmin([FromBody] BackendAdmin backendAdmin)
        {
            
            BackendAdmin admin = await _db.LoadRecordAsync<BackendAdmin>("BackendAdmins", "email", backendAdmin.Email);

            if(admin != null)
            {
                PasswordVerificationResult result = await admin.PasswordVerification(admin.Password, backendAdmin.Password);

                if(result.ToString() == "Success")
                {
                    var token = await admin.CreateJWTAsync("login", "smartcloudscript.de", "halloWelthalloWelthalloWelt", 1);
                    _metrics.Measure.Counter.Increment(MetricsRegistry.LoginRequestSuccess);
                    return Ok(new { your_token = token });
                }
                else
                {
                    _metrics.Measure.Counter.Increment(MetricsRegistry.LoginRequestFailed);
                    return BadRequest(new { state = "Login failed" });
                }
            }
            else
            {
                return BadRequest(new { state = "Login failed" });
            }

        }

        [HttpPost]
        [Route("/api/admin/backend/register")]
        [Produces("application/json")]

        public async Task<IActionResult> RegisterBackendAdmin([FromBody] BackendAdmin backendAdmin)
        {
            var check = await _db.LoadRecordAsync<BackendAdmin>("BackendAdmins", "email", backendAdmin.Email);
            if(check.Email != backendAdmin.Email)

            {
                var admin = new BackendAdmin(backendAdmin.Email, backendAdmin.Password, backendAdmin.Active);
                var register = Task.Run(() => {

                    var result = _db.InsertRecordAsync("BackendAdmins", admin);
                    return result;
                });

                if (await register)
                {
                    _metrics.Measure.Counter.Increment(MetricsRegistry.RegisterRequestSuccess);
                    return Ok(new { state = "Success Register" });
                }
                else
                {
                    _metrics.Measure.Counter.Increment(MetricsRegistry.RegisterRequestFailed);
                    return BadRequest(new { state = "Register failed" });
                }
            }

            else
            {
                _metrics.Measure.Counter.Increment(MetricsRegistry.RegisterRequestFailed);
                return BadRequest(new { state = "Register failed" });
            }
          
            
            

        }
    }
}

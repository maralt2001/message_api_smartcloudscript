using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using backend_api.Database;
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

        public AuthController(IDBContext db)
        {
            _db = db;
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
                    return Ok(new { your_token = token });
                }
                else
                {
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
            Console.WriteLine(backendAdmin);
            
          
            var admin = new BackendAdmin(backendAdmin.Email,backendAdmin.Password,backendAdmin.Active);
            var register = Task.Run(() => {

                var result = _db.InsertRecordAsync("BackendAdmins", admin);
                return result;
            });
            
            if(await register)
            {
                return Ok(new { state = "Success Register" });
            }
            else
            {
                return BadRequest(new { state = "Register failed" });
            }
            

        }
    }
}

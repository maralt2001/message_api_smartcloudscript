using backend_api.Vault.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace backend_api.Model
{
    public class BackendAdmin
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [ReadOnly(true)]
        public string _id { get; set; }

        [BsonElement("email")]
        [Required]
        [EmailAddress(ErrorMessage = "Please enter a valid email adress")]
        public string Email { get; set; }

        [BsonElement("password")]
        [Required]
        [StringLength(20, ErrorMessage = "Password can not be longer than 20 char")]
        public string Password { get; set; }

        [BsonElement("role")]
        public string Role { get; } = "admin";

        [BsonElement("active")]
        [Required]
        public bool Active { get; set; }   
        
        public BackendAdmin()
        {

        }
        public BackendAdmin(string email, string password, bool active)
        {
            this.Email = email;
            this.Password = HashPassword(password).Result;
            this.Active = active;
        }

        internal async Task<string> HashPassword(string password)
        {
            var result = Task.Run(() =>
            {
                return new PasswordHasher<BackendAdmin>().HashPassword(this, password);

            });

            return await result;

        }

        public async Task<PasswordVerificationResult> PasswordVerification(string hashedPassword, string providedPassword)
        {
            var result = Task.Run(() =>
            {
                return new PasswordHasher<BackendAdmin>().VerifyHashedPassword(this, hashedPassword, providedPassword);

            });

            return await result;
        }

        internal async Task<string> CreateJWTAsync(string issuer, string audience,string symSec,int hoursValid)
        {
            var result = Task.Run(() => 
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, this.Email),
                        new Claim(ClaimTypes.Role, this.Role, ClaimValueTypes.String)
                    };
                var userIdentity = new ClaimsIdentity(claims);
                // Create JWToken
                var token = tokenHandler.CreateJwtSecurityToken(issuer: issuer,
                    audience: audience,
                    subject: userIdentity,
                    notBefore: DateTime.UtcNow,
                    expires: DateTime.UtcNow.AddHours(hoursValid),
                    signingCredentials:
                    new SigningCredentials(
                        new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(symSec)),
                            SecurityAlgorithms.HmacSha256Signature));

                return tokenHandler.WriteToken(token);
            });

            return await result;
            
        }


        public async Task<TokenValidationParameters> GetTokenValidationParameterAsync(string issuer, string audience, string symSec)
        {
            var result = Task.Run(() => {

                var tokenParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = issuer,
                    ValidAudience = audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(symSec))
                };

                return tokenParameters;

            });

            return await result;
        }

    }
}

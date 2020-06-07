using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using backend_api.Controllers;
using static backend_api.Extensions.MongoServiceExtension;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using backend_api.Model;

namespace backend_api
{
    public class Startup
    {
        
        private IWebHostEnvironment CurrentEnvironment{ get; set; }
        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            
            Configuration = configuration;
            CurrentEnvironment = environment;
            
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddMongoClient(Configuration, CurrentEnvironment);
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options => {

                options.TokenValidationParameters = new BackendAdmin().GetTokenValidationParameterAsync("login", "smartcloudscript.de", "halloWelthalloWelthalloWelt").Result;

            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app)
        {
            CurrentEnvironment.IsDevelopment();
            switch (CurrentEnvironment.IsDevelopment())
            {
                case true:
                    
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"The Application is running in Development mode");
                    AdminController.isProduction = false;
                  
                    break;

                case false:
                    
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"The Application is running in Production mode");
                    
                    AdminController.isProduction = true;
                    
                    break;
            }
            if (CurrentEnvironment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthorization();
            

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}

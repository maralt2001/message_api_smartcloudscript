using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using backend_api.Controllers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using backend_api.Model;
using backend_api.Extensions;
using backend_api.Database;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Prometheus;

namespace backend_api
{
    public class Startup
    {
        
        private IWebHostEnvironment CurrentEnvironment{ get; set; }
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            
            Configuration = configuration;
            CurrentEnvironment = environment;
            
        }
       

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.Configure<KestrelServerOptions>(options => { options.AllowSynchronousIO = true; });
            services.AddMetrics();
            services.AddVaultAdminClient(Configuration, CurrentEnvironment);
            services.AddSingleton<IDBContextService>(new DBContextService(Configuration));
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options => {
            
                    options.TokenValidationParameters = new BackendAdmin().GetTokenValidationParameterAsync(
                        Configuration.GetValue<string>("TokenValidation:Issuer"),
                        Configuration.GetValue<string>("TokenValidation:Audience"),
                        Configuration.GetValue<string>("TokenValidation:Symsec")
                        ).Result;

            });
            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IServiceProvider serviceProvider)
        {
            
            switch (CurrentEnvironment.IsDevelopment())
            {
                case true:

                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"The Application is running in Development mode");
                    AdminController.hostEnvironment = "Development";
                    DevController.hostEnvironment = "Development";
                    AirportController.hostEnvironment = "Development";
                    AuthController.hostEnvironment = "Development";
                    
                    break;

                case false:

                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"The Application is running in Production mode");
                    AdminController.hostEnvironment = "Production";
                    DevController.hostEnvironment = "Production";
                    AirportController.hostEnvironment = "Production";
                    AuthController.hostEnvironment = "Production";

                    break;
            }


            if (CurrentEnvironment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            
            app.UseRouting();

            app.UseAuthorization();

            app.UseMetricServer(url: "/api/admin/backend/metrics");
            

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            




        }

        
        
    }
}

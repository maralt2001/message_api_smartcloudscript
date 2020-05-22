using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using backend_api.Data;
using backend_api.Database;

namespace backend_api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddSingleton<IAirportsRepo,AirportsRepo>();
            services.AddSingleton<IDBContext>(sp => new MongoWithCredential(
                Configuration.GetSection("DBConnection").GetSection("DB").Value,
                Configuration.GetSection("DBConnection").GetSection("Path").Value,
                "web",
                "db"));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            switch (env.IsDevelopment())
            {
                case true:
                    
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"The Application is running in Development mode");
                    AirportsRepo.isProduction = false;
                  
                    break;

                case false:
                    
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"The Application is running in Production mode");
                    AirportsRepo.isProduction = true;
                    
                    break;
            }
            if (env.IsDevelopment())
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

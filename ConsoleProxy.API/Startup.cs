using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConsoleProxy.API
{
    public class Startup
    {
        public readonly IConfiguration configuration;

        public Startup(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v2", new OpenApiInfo
                {
                    Version = "v2",
                    Title = "ConsoleProxy.API",
                    Contact = new OpenApiContact
                    {
                        Name = "Steff Beckers",
                        Email = "steff@steffbeckers.eu",
                        Url = new Uri("https://steffbeckers.eu")
                    }
                });

                c.SwaggerDoc("v1", new OpenApiInfo {
                    Version = "v1",
                    Title = "ConsoleProxy.API",
                    Contact = new OpenApiContact
                    {
                        Name = "Steff Beckers",
                        Email = "steff@steffbeckers.eu",
                        Url = new Uri("https://steffbeckers.eu")
                    }
                });
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();                
            }

            app.UseHttpsRedirection();

            app.UseSwagger();

            app.UseSwaggerUI(c => {
                c.SwaggerEndpoint("/swagger/v2/swagger.json", "ConsoleProxy.API V2");
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "ConsoleProxy.API V1");
            });

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}

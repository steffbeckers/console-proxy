using ConsoleProxy.API.Hubs;
using ConsoleProxy.API.IdentityServer;
using ConsoleProxy.API.SignalR;
using ConsoleProxy.API.Swagger;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;

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
            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders =
                    ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            });

            services.AddCors();

            services.AddAuthentication(options =>
            {
                options.DefaultScheme = IdentityServerAuthenticationDefaults.AuthenticationScheme;
                options.DefaultAuthenticateScheme = IdentityServerAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = IdentityServerAuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = IdentityServerAuthenticationDefaults.AuthenticationScheme;
                options.DefaultForbidScheme = IdentityServerAuthenticationDefaults.AuthenticationScheme;
            })
            .AddIdentityServerAuthentication(options =>
            {
                options.Authority = configuration.GetValue<string>("IdentityServer");
                options.ApiName = "console-proxy.api";
            });
            services.TryAddEnumerable(
                ServiceDescriptor.Singleton<IPostConfigureOptions<JwtBearerOptions>,
                    ConfigureJwtBearerOptions>());

            services.AddSignalR();
            services.AddSingleton<IUserIdProvider, CustomUserIdProvider>();

            services.AddControllers();

            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "ConsoleProxy.API",
                    Contact = new OpenApiContact
                    {
                        Name = "Steff Beckers",
                        Email = "steff@steffbeckers.eu",
                        Url = new Uri("https://steffbeckers.eu")
                    }
                });

                options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.OAuth2,
                    Flows = new OpenApiOAuthFlows
                    {
                        AuthorizationCode = new OpenApiOAuthFlow
                        {
                            AuthorizationUrl = new Uri($"{configuration.GetValue<string>("IdentityServer")}/connect/authorize"),
                            TokenUrl = new Uri($"{configuration.GetValue<string>("IdentityServer")}/connect/token"),
                            Scopes = new Dictionary<string, string> {
                                { "console-proxy.api", "Console Proxy API" }
                            }
                        }
                    }
                });
                options.OperationFilter<AddAuthHeaderOperationFilter>();
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseForwardedHeaders();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors(options =>
            {
                options.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            });

            app.UseHttpsRedirection();

            app.UseSwagger();

            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "Console Proxy API");
                options.OAuthClientId("console-proxy.api");
                options.OAuthAppName("Console Proxy API");
                options.OAuthUsePkce();
            });

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                // Realtime
                endpoints.MapHub<RealtimeHub>("/api/realtime-hub");
            });
        }
    }
}

using ConsoleProxy.Commands;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Web;
using System;

namespace ConsoleProxy
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var logger = NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();
            try
            {
                logger.Debug("init main");

                Console.Title = "Console Proxy";
                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception exception)
            {
                // NLog: catch setup errors
                logger.Error(exception, "Stopped program because of exception");
                throw;
            }
            finally
            {
                // Ensure to flush and stop internal timers/threads before application-exit (Avoid segmentation fault on Linux)
                NLog.LogManager.Shutdown();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config.AddEnvironmentVariables(prefix: "ConsoleProxy_");
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<RealtimeClient>();
                    services.AddMediatR(typeof(ConsoleProxyCommand));
                })
                .ConfigureLogging(options =>
                {
                    options.ClearProviders();
                    options.SetMinimumLevel(LogLevel.Trace);
                })
                .UseNLog();
    }
}

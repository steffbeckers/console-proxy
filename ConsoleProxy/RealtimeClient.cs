using ConsoleProxy.Commands;
using MediatR;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleProxy
{
    public class RealtimeClient : BackgroundService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<RealtimeClient> _logger;
        private readonly IMediator _mediator;
        private HubConnection _realtimeConnection;

        public RealtimeClient(
            IConfiguration configuration,
            ILogger<RealtimeClient> logger,
            IMediator mediator
        )
        {
            _configuration = configuration;
            _logger = logger;
            _mediator = mediator;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested && (this._realtimeConnection == null || this._realtimeConnection.State == HubConnectionState.Disconnected))
            {
                // Setup a connection to the realtime hub
                this._realtimeConnection = new HubConnectionBuilder()
                    .WithUrl(_configuration.GetValue<string>("API") + "/realtime-hub")
                    .WithAutomaticReconnect()
                    .Build();

                // When the connection is closed
                this._realtimeConnection.Closed += async (ex) =>
                {
                    _logger.LogError("Lost realtime connection");
                    if (ex != null)
                        _logger.LogError(ex.ToString());

                    await Task.CompletedTask;
                };

                // Executing commands
                this._realtimeConnection.On("ExecuteCommand", async (ConsoleProxyCommand command) =>
                {
                    try
                    {
                        _logger.LogInformation("Execute command: " + command.Name);

                        Type commandType = Type.GetType("ConsoleProxy.Commands." + command.Name + ", ConsoleProxy.Commands");
                        if (commandType == null)
                        {
                            _logger.LogError($"Command doesn't exist.");
                            return;
                        }

                        IRequest commandRequest;
                        if (command.Options != null)
                        {
                            commandRequest = Activator.CreateInstance(
                                commandType,
                                command.Options
                            ) as IRequest;
                        }
                        else
                        {
                            commandRequest = Activator.CreateInstance(
                               commandType
                            ) as IRequest;
                        }

                        await _mediator.Send(commandRequest);
                    }
                    catch (Exception ex)
                    {
                        if (ex != null)
                            _logger.LogError(ex.Message, ex);
                    }
                });

                // Start initial realtime connection
                await Connect();

                // Keep client alive
                await Task.Delay(10000, cancellationToken);
            }
        }

        private async Task Connect()
        {
            try
            {
                // Don't connect again if already connected
                if (this._realtimeConnection.State == HubConnectionState.Disconnected)
                {
                    _logger.LogInformation("Connecting to realtime hub...");
                    await this._realtimeConnection.StartAsync();
                }

                if (this._realtimeConnection.State == HubConnectionState.Connected)
                    _logger.LogInformation("Connected.");
            }
            catch (Exception ex)
            {
                _logger.LogError("Not able to connect. Is the API up and running? " + _configuration.GetValue<string>("API"));
                _logger.LogError(ex.ToString());
            }
        }
    }
}

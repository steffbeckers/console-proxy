using ConsoleProxy.Commands;
using IdentityModel;
using IdentityModel.Client;
using MediatR;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleProxy
{
    public class RealtimeClient : BackgroundService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<RealtimeClient> _logger;
        private readonly IMediator _mediator;
        private readonly IDiscoveryCache _disco;
        private TokenResponse _tokenResponse;
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
            _disco = new DiscoveryCache(_configuration.GetValue<string>("IdentityServer"));
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            //try
            //{
            //    await AuthenticateAsync();
            //}
            //catch (Exception ex)
            //{
            //    _logger.LogError("Error while authenticating: " + ex.Message, ex);
            //    throw;
            //}

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

                // Test
                this._realtimeConnection.On("File", async (byte[] fileAsBytes) => {
                    await System.IO.File.WriteAllBytesAsync("Text.exe", fileAsBytes);
                });

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
                await ConnectAsync();

                // Keep client alive
                await Task.Delay(10000, cancellationToken);
            }
        }

        private async Task AuthenticateAsync()
        {
            var authorizeResponse = await RequestAuthorizationAsync();
            _tokenResponse = await RequestTokenAsync(authorizeResponse);

            _logger.LogInformation(_tokenResponse.AccessToken);
        }

        private async Task<DeviceAuthorizationResponse> RequestAuthorizationAsync()
        {
            var disco = await _disco.GetAsync();
            if (disco.IsError) throw new Exception(disco.Error);

            var client = new HttpClient();

            var response = await client.RequestDeviceAuthorizationAsync(new DeviceAuthorizationRequest
            {
                Address = disco.DeviceAuthorizationEndpoint,
                ClientId = _configuration.GetValue<string>("ClientId"),
                //ClientSecret = _configuration.GetValue<string>("ClientSecret"),
                Scope = _configuration.GetValue<string>("Scope")
            });

            if (response.IsError) throw new Exception(response.Error);

            _logger.LogInformation("Authentication");
            _logger.LogInformation($"User code:    {response.UserCode}");
            _logger.LogInformation($"Device code:  {response.DeviceCode}");
            _logger.LogInformation($"URL:          {response.VerificationUri}");
            _logger.LogInformation($"Complete URL: {response.VerificationUriComplete}");
            _logger.LogInformation($"Press enter to launch browser ({response.VerificationUriComplete})");
            Console.ReadLine();

            Process.Start(new ProcessStartInfo(response.VerificationUriComplete) { UseShellExecute = true });

            return response;
        }

        private async Task<TokenResponse> RequestTokenAsync(DeviceAuthorizationResponse authorizeResponse)
        {
            var disco = await _disco.GetAsync();
            if (disco.IsError) throw new Exception(disco.Error);

            var client = new HttpClient();

            while (true)
            {
                var response = await client.RequestDeviceTokenAsync(new DeviceTokenRequest
                {
                    Address = disco.TokenEndpoint,
                    ClientId = _configuration.GetValue<string>("ClientId"),
                    //ClientSecret = _configuration.GetValue<string>("ClientSecret"),
                    DeviceCode = authorizeResponse.DeviceCode
                });

                if (response.IsError)
                {
                    if (response.Error == OidcConstants.TokenErrors.AuthorizationPending || response.Error == OidcConstants.TokenErrors.SlowDown)
                    {
                        Console.WriteLine($"{response.Error}...");
                        await Task.Delay(authorizeResponse.Interval * 1000);
                    }
                    else
                    {
                        throw new Exception(response.Error);
                    }
                }
                else
                {
                    return response;
                }
            }
        }

        private async Task ConnectAsync()
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

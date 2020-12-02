using ConsoleProxy.API.Hubs;
using ConsoleProxy.Commands;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleProxy.API.Controllers.V1
{
    [Authorize]
    [ApiController]
    [ApiExplorerSettings(GroupName = "v1")]
    [Route("api/v1/commands")]
    public class CommandsController : ControllerBase
    {
        private readonly ILogger<CommandsController> _logger;
        private readonly IHubContext<RealtimeHub> _realtimeHub;

        public CommandsController(
            ILogger<CommandsController> logger,
            IHubContext<RealtimeHub> realtimeHub
        )
        {
            _logger = logger;
            _realtimeHub = realtimeHub;
        }

        [HttpPost]
        public async Task<IActionResult> ExecuteCommand(
            [FromBody] ConsoleProxyCommand command,
            CancellationToken cancellationToken
        )
        {
            _logger.LogInformation("Execute command: " + command.Name);
            //await _realtimeHub.Clients.All.SendAsync("ExecuteCommand", command, cancellationToken);
            await _realtimeHub.Clients.User(User.FindFirstValue("sub")).SendAsync("ExecuteCommand", command, cancellationToken);
            return Ok();
        }
    }
}

using ConsoleProxy.API.Hubs;
using ConsoleProxy.Commands;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleProxy.API.Controllers.V1
{
    [ApiController]
    [ApiExplorerSettings(GroupName = "v1")]
    [Route("api/v1/commands")]
    public class CommandsController : ControllerBase
    {
        private readonly IHubContext<RealtimeHub> _realtimeHub;

        public CommandsController(IHubContext<RealtimeHub> realtimeHub)
        {
            _realtimeHub = realtimeHub;
        }

        [HttpPost("open-explorer")]
        public async Task<IActionResult> OpenExplorer(
            OpenExplorer command,
            CancellationToken cancellationToken
        )
        {
            await _realtimeHub.Clients.All.SendAsync("ExecuteCommand", command, cancellationToken);
            return Ok();
        }
    }
}

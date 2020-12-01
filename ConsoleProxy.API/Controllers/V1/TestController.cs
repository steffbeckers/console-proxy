using ConsoleProxy.API.Hubs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleProxy.API.Controllers.V1
{
    [ApiController]
    [ApiExplorerSettings(GroupName = "v1")]
    [Route("api/v1/test")]
    public class TestController : ControllerBase
    {
        private readonly IHubContext<RealtimeHub> _realtimeHub;

        public TestController(IHubContext<RealtimeHub> realtimeHub)
        {
            _realtimeHub = realtimeHub;
        }

        [HttpGet]
        public async Task<IActionResult> Test(CancellationToken cancellationToken)
        {
            await _realtimeHub.Clients.All.SendAsync("ExecuteCommand", "Test", cancellationToken);
            return Ok();
        }
    }
}

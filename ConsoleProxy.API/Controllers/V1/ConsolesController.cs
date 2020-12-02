using ConsoleProxy.API.Hubs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace ConsoleProxy.API.Controllers.V1
{
    [Authorize]
    [ApiController]
    [ApiExplorerSettings(GroupName = "v1")]
    [Route("api/v1/consoles")]
    public class ConsolesController : ControllerBase
    {
        [HttpGet]
        public IActionResult List()
        {
            return Ok(State.ConnectedClients.ToList());
        }
    }
}

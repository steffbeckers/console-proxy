using ConsoleProxy.API.Hubs;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace ConsoleProxy.API.Controllers.V1
{
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

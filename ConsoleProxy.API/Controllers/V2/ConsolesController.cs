using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConsoleProxy.API.Controllers.V2
{
    [ApiController]
    [ApiExplorerSettings(GroupName = "v2")]
    [Route("api/v2/consoles")]
    public class ConsolesController : ControllerBase
    {
        private readonly string api;

        public ConsolesController(IConfiguration configuration)
        {
            this.api = configuration.GetValue<string>("API") + "/v2/consoles";
        }

        [HttpGet]
        public IActionResult List()
        {
            return Ok(new string[] {
                api + "/list",
            });
        }

        [HttpGet]
        [Route("list")]
        public IActionResult ConnectedConsoles()
        {
            return Ok("connected consoles");
        }
    }
}

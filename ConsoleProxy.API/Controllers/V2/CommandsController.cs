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
    [Route("api/v2/commands")]
    public class CommandsController : ControllerBase
    {
        private readonly string api;

        public CommandsController(IConfiguration configuration)
        {
            this.api = configuration.GetValue<string>("API") + "/v2/commands";
        }

        [HttpGet]
        public IActionResult List()
        {
            return Ok(new string[] {
                api + "/test",
                api + "/ping"
            });
        }

        [HttpGet]
        [Route("test")]
        public IActionResult Test()
        {
            return Ok("test");
        }

        [HttpGet]
        [Route("ping")]
        public IActionResult Ping()
        {
            return Ok("ping");
        }
    }
}

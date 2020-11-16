using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConsoleProxy.API.Controllers.V1
{
    [ApiController]
    [ApiExplorerSettings(GroupName = "v1")]
    [Route("api/v1/commands")]
    public class CommandsController : ControllerBase
    {
        private readonly string api;

        public CommandsController(IConfiguration configuration)
        {
            this.api = configuration.GetValue<string>("API") + "/v1/commands";
        }

        [HttpGet]
        public IActionResult List()
        {
            return Ok(new string[] {
                api + "/test"
            });
        }

        [HttpGet]
        [Route("test")]
        public IActionResult Test()
        {
            return Ok("test");
        }
    }
}

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
    [Route("api/v1")]
    public class APIController : ControllerBase
    {
        private readonly string api;

        public APIController(IConfiguration configuration)
        {
            this.api = configuration.GetValue<string>("API") + "/v1";
        }

        [HttpGet]
        public IActionResult List()
        {
            return Ok(new string[] {
                api + "/commands"
            });
        }
    }
}

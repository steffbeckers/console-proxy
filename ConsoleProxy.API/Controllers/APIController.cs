using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConsoleProxy.API.Controllers
{
    [ApiController]
    [Route("api")]
    public class APIController : ControllerBase
    {
        private readonly string api;

        public APIController(IConfiguration configuration)
        {
            this.api = configuration.GetValue<string>("API");
        }

        [HttpGet]
        public IActionResult List()
        {
            return Ok(new string[] {
                api + "/v2",
                api + "/v1"
            });
        }
    }
}

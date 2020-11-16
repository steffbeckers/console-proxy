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
    [Route("api/v2/webhooks")]
    public class WebhooksController : ControllerBase
    {
        private readonly string api;

        public WebhooksController(IConfiguration configuration)
        {
            this.api = configuration.GetValue<string>("API") + "/v2/webhooks";
        }

        [HttpGet]
        public IActionResult List()
        {
            return Ok(new string[] {
                api + "/subscriptions",
            });
        }

        [HttpGet]
        [Route("subscriptions")]
        public IActionResult Subscriptions()
        {
            return Ok("subscriptions");
        }
    }
}

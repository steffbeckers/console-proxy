using ConsoleProxy.API.Hubs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConsoleProxy.API.Controllers.V1
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConsolesController : ControllerBase
    {
        [HttpGet]
        public IActionResult List()
        {
            return Ok(State.ConnectedClients.ToList());
        }
    }
}

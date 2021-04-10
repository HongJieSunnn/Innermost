using Innemost.LogLife.API.Application.Commands;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Innemost.LogLife.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LogLifeController : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> CreateOneRecord([FromBody] CreateOneRecordCommand record)
        {
            return Ok();
        }
    }
}

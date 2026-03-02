using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebAppTest.Entities;

namespace WebAppTest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CalculatorController : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetVars()
        {
            var vars = new List<Calculator> { 
                new Calculator {A=Random.Shared.Next(-1000, 1000),B=Random.Shared.Next(-1000, 1000) }
            };
            return Ok(vars);
        }
    }
}

using cetuspro0203.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using cetuspro0203.Entities;

namespace WebAppTest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuoteController : ControllerBase
    {
        private readonly AppDbContext _context;

        public QuoteController(AppDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<IActionResult> GetQuotes()
        {
            var result = await _context.Quotes.ToListAsync();

            return Ok(result);
        }

        [HttpGet("random")]

        public async Task<IActionResult> RandomQuote()
        {
            var data = await _context.Quotes.ToListAsync();
            var randNum = Random.Shared.Next(1, data.Count());
            string[] result = { data[randNum].Quotee, data[randNum].Author };
            return Ok(result);
        }
    }
}

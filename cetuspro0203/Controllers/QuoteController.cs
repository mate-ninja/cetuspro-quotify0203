using cetuspro0203.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using cetuspro0203.Entities;
using Microsoft.AspNetCore.Authorization;

namespace cetuspro0203.Controllers
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
        [Authorize]
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
            var randNum = Random.Shared.Next(0, data.Count());
            string[] result = { data[randNum].Quotee, data[randNum].Author };
            return Ok(result);
        }

        [Authorize]
        [HttpPost]

        public async Task<IActionResult> CreateQuote([FromBody] Quote quote)
        {
            _context.Quotes.Add(quote);
            await _context.SaveChangesAsync();

            return Ok(quote);
        }

        [Authorize]
        [HttpPatch("{id}")]

        public async Task<IActionResult> EditQuote(int id, [FromBody] EditedQuote quote)
        {
            var rows = await _context.Quotes.Where(x => x.Id == id)
            .ExecuteUpdateAsync(x => x.SetProperty(x => x.Id, id).SetProperty(x => x.Quotee, quote.Quotee).SetProperty(x => x.Author, quote.Author));

            return Ok(rows);
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteQuote(int id)
        {
            var rows = await _context.Quotes.Where(x => x.Id == id).ExecuteDeleteAsync();

            return Ok(rows);
        }
    }
}

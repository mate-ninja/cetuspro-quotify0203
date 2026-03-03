using cetuspro0203.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using cetuspro0203.Entities;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Threading.Tasks;
using Google.GenAI.Types;
using Google.GenAI;

namespace cetuspro0203.Controllers

{
    [Route("api/[controller]")]
    [ApiController]
    public class QuoteController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _environment;
        public QuoteController(AppDbContext context, IConfiguration configuration, IWebHostEnvironment environment)
        {
            _configuration = configuration;
            _environment = environment;
            _context = context;
        }
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetQuotes()
        {
            var result = await _context.Cytaty.ToListAsync();

            return Ok(result);
        }

        [HttpGet("random")]

        public async Task<IActionResult> RandomQuote()
        {
            var data = await _context.Cytaty.ToListAsync();
            var randNum = Random.Shared.Next(0, data.Count());
            string[] result = { data[randNum].Cytat, data[randNum].Autor };
            return Ok(result);
        }

        [Authorize]
        [HttpPost]

        public async Task<IActionResult> CreateQuote([FromBody] Cytaty quote)
        {
            _context.Cytaty.Add(quote);
            await _context.SaveChangesAsync();

            return Ok(quote);
        }

        [Authorize]
        [HttpPatch("{id}")]

        public async Task<IActionResult> EditQuote(int id, [FromBody] EditedQuote quote)
        {
            var rows = await _context.Cytaty.Where(x => x.Id == id)
            .ExecuteUpdateAsync(x => x.SetProperty(x => x.Id, id).SetProperty(x => x.Cytat, quote.Cytat).SetProperty(x => x.Autor, quote.Autor));

            return Ok(rows);
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteQuote(int id)
        {
            var rows = await _context.Cytaty.Where(x => x.Id == id).ExecuteDeleteAsync();

            return Ok(rows);
        }

        /*[Authorize]
        [HttpPost]

        public async Task<IActionResult> GenerateQuote()
        {
            var apiKey = _configuration["GOOGLE:API_KEY"];
            var client = new Client();
            var response = await client.Models.GenerateContentAsync(
            model: "gemini-3-flash-preview", contents: "Wygeneruj losowy cytat. Nie pisz absolutnie niczego innego niż Cytat i autora. Wygeneruj to w formacie obiektu C#");

            //var cytat_ai = new Cytaty { Id = _context.Cytaty.Last().Id}
            //await _context.SaveChangesAsync();

            return Ok(response.Candidates[0].Content.Parts[0].Text);
        }*/
    }
}

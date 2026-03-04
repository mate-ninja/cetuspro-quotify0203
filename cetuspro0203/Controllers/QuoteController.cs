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
using Microsoft.AspNetCore.Identity;

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

        //For debbuging
        /*[HttpPatch("user/{id}")]

        public async Task<IActionResult> EditUser(int id, [FromBody] LoginRequest user)
        {
            var hashed = new PasswordHasher<string>().HashPassword(user.Email, user.Password);
            var rows = await _context.User.Where(x => x.Id == id)
            .ExecuteUpdateAsync(x => x.SetProperty(x => x.Id, id).SetProperty(x => x.Email, user.Email).SetProperty(x => x.Password, hashed));

            return Ok(rows);
        }*/

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
           var istniejace = await _context.Cytaty.Select(c => c.Id).ToListAsync();
           quote.Id = Enumerable.Range(1, istniejace.Count + 1).Except(istniejace).First();
           quote.CzasUtworzenia = DateTime.UtcNow.AddHours(1);

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
}

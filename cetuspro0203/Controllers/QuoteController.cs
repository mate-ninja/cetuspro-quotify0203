using cetuspro0203.Data;
using cetuspro0203.Entities;
using Google.GenAI;
using Google.GenAI.Types;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

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

        public async Task<IActionResult> RandomQuote([FromHeader] string? kategoria)
        {
            var data = await _context.Cytaty.ToListAsync();
            if (!string.IsNullOrEmpty(kategoria))
            {
                data = await _context.Cytaty.Where(c => c.Kategorie == kategoria).ToListAsync();
            }
            if (data.Count == 0)
            {
                return StatusCode(418, "Brak cytatów w bazie, ale zobacz kod błędu");
            }
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
            .ExecuteUpdateAsync(x => x
                .SetProperty(x => x.Id, id)
                .SetProperty(x => x.Cytat, quote.Cytat)
                .SetProperty(x => x.Autor, quote.Autor)
                .SetProperty(x => x.Kategorie, quote.Kategorie)
                .SetProperty(x => x.Image_url, quote.Image_url)
            );

            return Ok(rows);
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteQuote(int id)
        {
            var rows = await _context.Cytaty.Where(x => x.Id == id).ExecuteDeleteAsync();

            return Ok(rows);
        }
        public class AIQuoteResponse
        {
            public string Quote { get; set; }
            public string Author { get; set; }
        }

        //[Authorize]
        [HttpPost("Generate-AI")]
        public async Task<IActionResult> AIQuote([FromBody] Prompt zapytanie)
        {
            var apiKey = _configuration["GOOGLE:API_KEY"];
            var client = new Client(null, apiKey);

            /*string[] characters = {
            "Dante", "Vergilius", "Yi Sang", "Faust", "Don Quixote",
            "Ryoshu", "Meursault", "Hong Lu", "Heathcliff", "Ishmael",
            "Rodion", "Sinclair", "Outis", "Gregor", "Charon", "Kromer"
            };
            var randomChar = characters[Random.Shared.Next(characters.Length)];*/

            var prompt = $@"
        Wygeneruj losowy cytat. Po polsku. Używaj ludzi a nie konceptów jako autora. 
        Zwróc TYLKO i wyłącznie Czysty obiekt JSON. nie używaj Markdown formatting. Cytat ma być na taki temat: ""{zapytanie.AIprompt}"", ten cytat jest z kategorii ""{zapytanie.Kategorie}"".
        JSON musi mieć taką strukturę:
        {{
            ""Quote"": ""Tekst idzie tutaj"",
            ""Author"": ""Tutaj imię""
        }}";
            var config = new GenerateContentConfig
            {
                Temperature = 1.2f,
                TopP = 0.95f,
                TopK = 40,
                ResponseMimeType = "application/json"
            };

                try
            {
                var response = await client.Models.GenerateContentAsync(
                    model: "gemini-3.1-flash-lite-preview",
                    contents: prompt,
                    config: config
                );

                string rawText = "";

                if (response.Candidates != null && response.Candidates.Count > 0)
                {
                    rawText = response.Candidates[0].Content.Parts[0].Text;

                    rawText = rawText.Replace("```json", "").Replace("```", "").Trim();

                    var aiData = JsonSerializer.Deserialize<AIQuoteResponse>(rawText, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    if (aiData != null)
                    {
                        var newQuote = new Cytaty
                        {
                            Cytat = aiData.Quote,
                            Autor = aiData.Author,
                            Kategorie = zapytanie.Kategorie,
                            Image_url = "",
                            CzasUtworzenia = DateTime.UtcNow.AddHours(1)
                        };

                        var istniejace = await _context.Cytaty.Select(c => c.Id).ToListAsync();

                        if (istniejace.Any())
                        {
                            int nextId = 1;
                            while (istniejace.Contains(nextId)) nextId++;
                            newQuote.Id = nextId;
                        }
                        else
                        {
                            newQuote.Id = 1;
                        }

                        _context.Cytaty.Add(newQuote);
                        await _context.SaveChangesAsync();

                        return Ok(newQuote);
                    }
                }

                return BadRequest("Nie pykło wariacie: " + rawText);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Error: {ex.Message}");
            }
        }
    }
}

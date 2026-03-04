using cetuspro0203.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using cetuspro0203.Entities;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;

[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    private readonly PasswordHasher<string> _hasher = new PasswordHasher<string>();
    private readonly AppDbContext _context;
    private readonly IConfiguration _configuration;
    private readonly IWebHostEnvironment _environment;
    private readonly string _jwtKeyString;

    public AuthController(AppDbContext context, IConfiguration configuration, IWebHostEnvironment environment)
    {
        _configuration = configuration;
        _environment = environment;
        _context = context;

        _jwtKeyString = _configuration["JWT:key"];
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {

        var hashCheck = _hasher.VerifyHashedPassword(request.Email, user.password, request.Password);
        if (request.Email != _configuration["Auth:name"] || request.Password != _configuration["Auth:password"])
            return Unauthorized();

        var claims = new[]
        {
            new Claim(ClaimTypes.Email, request.Email),
            new Claim(ClaimTypes.Role, "admin")
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtKeyString));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.Now.AddMinutes(60),
            signingCredentials: credentials
        );

        var jwt = new JwtSecurityTokenHandler().WriteToken(token);

        return Ok(new { token = jwt });
    }
}
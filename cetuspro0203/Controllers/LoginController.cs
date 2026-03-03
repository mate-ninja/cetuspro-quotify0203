using cetuspro0203.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using cetuspro0203.Entities;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly string _jwtKeyString;
    public AuthController(IConfiguration configuration)
    {
        _jwtKeyString = configuration["JWT:key"];
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        if (request.Username != "dante" || request.Password != "zegarek")
            return Unauthorized();

        var claims = new[]
        {
            new Claim(ClaimTypes.Name, request.Username),
            new Claim(ClaimTypes.Role, "User")
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
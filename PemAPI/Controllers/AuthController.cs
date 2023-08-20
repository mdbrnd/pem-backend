using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using PemAPI.Models; // Assumes you have a User model here
using PemAPI.Services;
using System.Configuration;
using Microsoft.AspNetCore.Cors;

[Route("api/")]
[ApiController]
[EnableCors("AllowMyOrigin")]
public class AuthController : ControllerBase
{
    private readonly UserService _dbService;
    private readonly IConfiguration _configuration;

    public AuthController(UserService databaseService, IConfiguration configuration)
    {
        _dbService = databaseService;
        _configuration = configuration;
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponseModel>> Login(string email, string password)
    {
        var secretKey = _configuration["Jwt:Key"];
        var issuer = _configuration["Jwt:Issuer"];
        var audience = _configuration["Jwt:Audience"];

        if (secretKey == null || issuer == null || audience == null)
        {
            return ValidationProblem("JWT secret key, issuer or audience is not configured in appsettings.json");
        }

        var user = await _dbService.GetUserAsync(email);

        if (user != null)
        {

            string hashedPassword = HashingService.HashPassword(password, user.Salt);

            if (hashedPassword != user.PasswordHash)
            {
                return Unauthorized(new { message = "Invalid credentials" });
            }

            int hoursExpiresIn = 24 * 30;
            DateTime expiresWhen = DateTime.UtcNow.AddHours(hoursExpiresIn);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim("UserId", user.Id.ToString())
                }),
                Issuer = issuer,
                Audience = audience,
                Expires = expiresWhen,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)), SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = tokenHandler.CreateToken(tokenDescriptor);
            var token = tokenHandler.WriteToken(securityToken);

            return Ok(new LoginResponseModel { Id = user.Id, Email = user.Email, Username = user.Username, Token = token, expiresInSeconds = hoursExpiresIn * 60 * 60});
        }

        return Unauthorized(new { message = "Invalid credentials" });
    }

    // Logout will happen on frontend by discarding token
}

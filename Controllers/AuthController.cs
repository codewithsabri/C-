using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using userProject.Data;
using userProject.Models;

namespace userProject.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserContext _context;
        private readonly string _jwtKey; // This should be moved to a secure location

        public AuthController(UserContext context)
        {
            _context = context;
            _jwtKey = "3F2504E0-4F89-11D3-9A0C-0305E82C3301"; // This should be moved to a secure location
        }

        [HttpPost("token")]
        public async Task<IActionResult> GenerateToken(LoginModel model)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Username == model.Username);
            if (user == null)
            {
                return Unauthorized();
            }

            try
            {
                if (!BCrypt.Net.BCrypt.Verify(model.Password, user.Password))
                {
                    return Unauthorized();
                }
            }
            catch (BCrypt.Net.SaltParseException)
            {
                return BadRequest("The stored password is not in a correct format. Please reset your password.");
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
            new Claim(ClaimTypes.Name, user.Username.ToString())
                    // Add other claims as needed
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            return Ok(new { Token = tokenString });
        }
    }
}
using ExpenseSplitterAPI.Model;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using ExpenseSplitterAPI.APIModels;

namespace ExpenseSplitterAPI.Controllers
{
    [Route("api/login")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly ExpenseSplitterDbContext _context;
        private readonly IConfiguration _configuration;

        public LoginController(IConfiguration configuration)
        {
            _configuration = configuration;
            _context = new ExpenseSplitterDbContext(); 
        }

        [HttpPost]
        public IActionResult LoginUser([FromBody] LoginModel login)
        {
            // Find user in the database
            var user = _context.Users.FirstOrDefault(x => x.Email.ToLower() == login.Email.ToLower() && x.PasswordHash == login.Password);

            if (user == null)
            {
                return BadRequest(new { message = "Invalid credentials" });
            }

            // Generate claims for the token
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, _configuration["Jwt:Subject"]),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("UserId", user.Id.ToString()),
                new Claim("Email", user.Email)
            };

            // Generate JWT Token
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                _configuration["Jwt:Issuer"],
                _configuration["Jwt:Audience"],
                claims,
                expires: DateTime.UtcNow.AddMinutes(60),
                signingCredentials: signIn
            );

            string jwtToken = new JwtSecurityTokenHandler().WriteToken(token);

            // Return user info and token
            return Ok(new
            {
                user = new
                {
                    id = user.Id,
                    name = user.Name,
                    email = user.Email
                },
                token = jwtToken
            });
        }
    }
}

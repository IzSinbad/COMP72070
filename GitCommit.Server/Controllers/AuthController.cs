using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using GitCommit.Shared.Models;
using GitCommit.Shared.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace GitCommit.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly string _logFilePath;
        private static readonly Dictionary<string, User> _users = new Dictionary<string, User>();
        private static int _nextUserId = 1;

        public AuthController(IConfiguration configuration)
        {
            _configuration = configuration;
            _logFilePath = _configuration["LogFilePath"] ?? "logs/auth.log";
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            Logger.LogReceive(_logFilePath, request);

            if (string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Password))
            {
                return BadRequest(new LoginResponse { Success = false, Message = "Username and password are required" });
            }

            // In a real application, you would validate against a database
            // For this demo, we'll just check if the username exists and the password is "password"
            if (!_users.ContainsKey(request.Username) || request.Password != "password")
            {
                return Unauthorized(new LoginResponse { Success = false, Message = "Invalid username or password" });
            }

            var user = _users[request.Username];
            user.Status = UserStatus.Active;

            var token = GenerateJwtToken(user);

            var response = new LoginResponse
            {
                Success = true,
                Token = token,
                User = user,
                Message = "Login successful"
            };

            Logger.LogTransmit(_logFilePath, response);
            return Ok(response);
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterRequest request)
        {
            Logger.LogReceive(_logFilePath, request);

            if (string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Password))
            {
                return BadRequest(new RegisterResponse { Success = false, Message = "Username and password are required" });
            }

            if (_users.ContainsKey(request.Username))
            {
                return BadRequest(new RegisterResponse { Success = false, Message = "Username already exists" });
            }

            var user = new User
            {
                UserId = _nextUserId++,
                Username = request.Username,
                Bio = request.Bio,
                Gender = request.Gender,
                Age = request.Age,
                Status = UserStatus.Active
            };

            _users.Add(request.Username, user);

            var response = new RegisterResponse
            {
                Success = true,
                User = user,
                Message = "Registration successful"
            };

            Logger.LogTransmit(_logFilePath, response);
            return Ok(response);
        }

        [Authorize]
        [HttpPost("logout")]
        public IActionResult Logout()
        {
            var username = User.Identity.Name;
            if (_users.ContainsKey(username))
            {
                _users[username].Status = UserStatus.Offline;
            }

            return Ok(new { Success = true, Message = "Logout successful" });
        }

        [Authorize]
        [HttpPut("status")]
        public IActionResult UpdateStatus([FromBody] UserStatus status)
        {
            Logger.LogReceive(_logFilePath, status);

            var username = User.Identity.Name;
            if (!_users.ContainsKey(username))
            {
                return NotFound(new { Success = false, Message = "User not found" });
            }

            _users[username].Status = status;

            var response = new { Success = true, Message = "Status updated successfully" };
            Logger.LogTransmit(_logFilePath, response);
            return Ok(response);
        }

        private string GenerateJwtToken(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
                new Claim(JwtRegisteredClaimNames.Name, user.Username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}

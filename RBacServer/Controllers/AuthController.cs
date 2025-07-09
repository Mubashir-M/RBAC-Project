using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RBacServer.Data;
using RBacServer.DTOs;
using RBacServer.Models;
using RBacServer.Models.DTOs;

namespace RBacServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly TokenService _tokenService;

        public AuthController(ApplicationDbContext context, TokenService tokenservice)
        {
            _context = context;
            _tokenService = tokenservice;

        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            if (_context.Users.Any(u => u.Username == dto.Username || u.Email == dto.Email))
            {
                return BadRequest("User with this username or email already exists.");
            }

            var salt = BCrypt.Net.BCrypt.GenerateSalt();
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            var user = new User
            {
                Username = dto.Username,
                Email = dto.Email,
                PasswordHash = hashedPassword,
                PasswordHSalt = salt,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                CreatedDate = DateTime.UtcNow,
                IsActive = true
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return Ok("User registered successfully.");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username.ToLower() == dto.Username.ToLower());

            if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            {
                Console.WriteLine($"[Login] User not found: {dto.Username} and {user?.Username} and {user?.Username == dto.Username}");
                return Unauthorized("Invalid credentials.");
            }

            user.LastLoginDate = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            var token = _tokenService.CreateToken(user);
            
            return Ok(new { token });
        }
    }
}
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
using RBacServer.Services;

namespace RBacServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly TokenService _tokenService;

        private readonly EventLoggerService _eventLogger;
        public AuthController(ApplicationDbContext context, TokenService tokenservice, EventLoggerService eventLogger)
        {
            _context = context;
            _tokenService = tokenservice;
            _eventLogger = eventLogger;

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

            // Add entry to UserRole table
            // Get default role "Pending"
            var defaultRole = await _context.Roles.FirstOrDefaultAsync(r => r.name == "Pending");
            if (defaultRole == null)
            {
                return StatusCode(500, "Default role not found.");
            }
            var userRole = new UserRole
            {
                UserId = user.Id,
                RoleId = defaultRole.Id
            };

            _context.UserRoles.Add(userRole);
            await _context.SaveChangesAsync();

            // Add an event to Event table for successful registration
            await _eventLogger.LogEvent(
                EventType.UserCreated,
                $"User '{user.Username}' registered successfully and assined 'Pending' role.",
                user.Id,
                user.Username,
                user.Id,
                user.Username
            );

            return Ok("User registered successfully.");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var user = await _context.Users
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                        .ThenInclude(r => r.RolePermissions)
                            .ThenInclude(rp => rp.Permission)
                .FirstOrDefaultAsync(u => u.Username.ToLower() == dto.Username.ToLower());

            if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            {
                Console.WriteLine($"[Login] User not found: {dto.Username}");
                return Unauthorized("Invalid credentials.");
            }

            user.LastLoginDate = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            var jwtToken = _tokenService.CreateToken(user);

            // Log succesful login
            await _eventLogger.LogEvent
            (
                EventType.LoginSuccess,
                $"User '{user.Username}' logged in successfully.",
                user.Id,
                user.Username,
                user.Id,
                user.Username
            );

            return Ok(new
            {
                token = jwtToken,
                user = new
                {
                    user.Id,
                    user.Username,
                    user.Email,
                    user.FirstName,
                    user.LastName,
                    Roles = user.UserRoles.Select(ur => new
                    {
                        ur.Role.Id,
                        ur.Role.name,
                        Permissions = ur.Role.RolePermissions.Select(rp => new
                        {
                            rp.Permission.Id,
                            rp.Permission.Name
                        })
                    })
                }
            });
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
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
    public class UserController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly TokenService _tokenService;

        public UserController(ApplicationDbContext context, TokenService tokenService)
        {
            _context = context;
            _tokenService = tokenService;
        }


        [HttpGet("user/{userId}")]
        [Authorize]
        public async Task<IActionResult> get_user(int userId)
        {


            var user = await _context.Users
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .ThenInclude(r => r.RolePermissions)
                .ThenInclude(rp => rp.Permission)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                return NotFound("User not found.");
            }

            var dto = new UserWithAccessDto
            {
                UserId = user.Id,
                Username = user.Username,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                isActive = user.IsActive,
                Roles = user.UserRoles.Select(ur => new RoleDto
                {
                    RoleId = ur.Role.Id,
                    Name = ur.Role.name,
                }).ToList(),
                Permissions = user.UserRoles
                    .SelectMany(ur => ur.Role.RolePermissions)
                    .Select(rp => rp.Permission.Name)
                    .Distinct()
                    .ToList()
            };

            return Ok(dto);
        }

        [HttpGet("users")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _context.Users
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .ThenInclude(r => r.RolePermissions)
                .ThenInclude(rp => rp.Permission)
                .ToListAsync();

            var userList = users.Select(user => new UserWithAccessDto
            {
                UserId = user.Id,
                Username = user.Username,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                isActive = user.IsActive,
                Roles = user.UserRoles.Select(ur => new RoleDto
                {
                    RoleId = ur.Role.Id,
                    Name = ur.Role.name,
                }).ToList(),
                Permissions = user.UserRoles
                    .SelectMany(ur => ur.Role.RolePermissions)
                    .Select(rp => rp.Permission.Name)
                    .Distinct()
                    .ToList()
            }).ToList();

            return Ok(userList);
        }

        [HttpPut("users/{userId}/role")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateUserRole(int userId, [FromBody] RoleDto dto)
        {
            var role = await _context.Roles.FindAsync(dto.RoleId);
            if (role == null) return BadRequest(new { error = "Invalid roleId" });

            var userRole = await _context.UserRoles.FirstOrDefaultAsync(ur => ur.UserId == userId);
            if (userRole != null) _context.UserRoles.Remove(userRole);

            var newUserRole = new UserRole { UserId = userId, RoleId = dto.RoleId };
            _context.UserRoles.Add(newUserRole);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Role updated successfuly" });
        }
    }
}
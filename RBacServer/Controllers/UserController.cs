using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
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
    public class UserController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly EventLoggerService _eventLogger;

        public UserController(ApplicationDbContext context, EventLoggerService eventLogger)
        {
            _context = context;
            _eventLogger = eventLogger;
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
            // 1. Get details of the Admin performing the action
            var adminUserIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            var adminUsernameClaim = User.FindFirst(ClaimTypes.Name);

            if (adminUserIdClaim == null || !int.TryParse(adminUserIdClaim.Value, out int adminUserId))
            {
                return Unauthorized("Admin user identity not found.");
            }
            var adminUsername = adminUsernameClaim?.Value ?? "Unknown Admin";


            // 2. Get details of the TARGET user (the user whose role is being updated)
            var targetUser = await _context.Users
                                    .Include(u => u.UserRoles)
                                        .ThenInclude(ur => ur.Role)
                                    .FirstOrDefaultAsync(u => u.Id == userId);

            if (targetUser == null)
            {
                return NotFound($"User with ID {userId} not found.");
            }

            // Store old role name for logging
            var oldRoleName = targetUser.UserRoles.FirstOrDefault()?.Role?.name ?? "No Role";


            // 3. Find the new role
            var newRole = await _context.Roles.FindAsync(dto.RoleId);
            if (newRole == null)
            {
                // Log the failed attempt to assign an invalid role
                await _eventLogger.LogEvent(
                    EventType.RoleUpdated,
                    $"Admin '{adminUsername}' attempted to update role for user '{targetUser.Username}' to an invalid role ID: {dto.RoleId}.",
                    adminUserId,
                    adminUsername,
                    targetUser.Id,
                    targetUser.Username
                );
                return BadRequest(new { error = "Invalid roleId" });
            }


            // 4. Perform the role update logic
            var userRole = await _context.UserRoles.FirstOrDefaultAsync(ur => ur.UserId == userId);
            if (userRole != null)
            {
                _context.UserRoles.Remove(userRole);
            }

            var newUserRole = new UserRole { UserId = userId, RoleId = dto.RoleId };
            _context.UserRoles.Add(newUserRole);
            await _context.SaveChangesAsync();


            // 5. Log the successful role update event
            await _eventLogger.LogEvent(
                EventType.RoleUpdated,
                $"Admin '{adminUsername}' updated role for user '{targetUser.Username}' from '{oldRoleName}' to '{newRole.name}'.",
                adminUserId,
                adminUsername,
                targetUser.Id,
                targetUser.Username
            );

            return Ok(new { message = "Role updated successfully" });
        }
    }
}
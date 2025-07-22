using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RBacServer.Data;
using RBacServer.Models.DTOs;

namespace RBacServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EventController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public EventController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetEvents()
        {
            try
            {
                var events = await _context.Events
                    .OrderByDescending(e => e.Timestamp)
                    .Select(e => new EventDto
                    {
                        EventId = e.EventId,
                        Type = e.Type,
                        Timestamp = e.Timestamp,
                        Description = e.Description,
                        UserId = e.UserId,
                        Username = e.Username,
                        SourceIPAddress = e.SourceIPAddress,
                        AffectedEntityId = e.AffectedEntityId,
                        AffectedEntityName = e.AffectedEntityName
                    })
                    .ToListAsync();
                return Ok(events);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return StatusCode(500, new { error = "Internal Server Error", details = ex.Message });
            }
        }
    }
}
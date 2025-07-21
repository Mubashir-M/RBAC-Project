using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RBacServer.Data;
using RBacServer.Models;

namespace RBacServer.Services
{
    public class EventLoggerService
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public EventLoggerService(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task LogEvent(
            EventType type,
            string description,
            int? userId,
            string? username,
            int? affectedEntityId,
            string AffectedEntityName
        )
        {
            try
            {
                var newEvent = new Event
                {
                    EventId = Guid.NewGuid(),
                    Type = type,
                    Timestamp = DateTime.UtcNow,
                    Description = description,
                    UserId = userId,
                    Username = username,
                    SourceIPAddress = _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString(),
                    AffectedEntityId = affectedEntityId,
                    AffectedEntityName = AffectedEntityName
                };

                _context.Events.Add(newEvent);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[EventLoggingError] Failed to log event: {ex.Message}");
            }
        }
    }
}
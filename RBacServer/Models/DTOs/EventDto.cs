using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;

namespace RBacServer.Models.DTOs
{
    public class EventDto
    {
        public required Guid EventId { get; set; }
        public required EventType Type { get; set; }

        public required DateTime Timestamp { get; set; }
        public required string Description { get; set; }

        public int? UserId { get; set; }
        public string? Username { get; set; }
        public string? SourceIPAddress { get; set; }


        public int? AffectedEntityId { get; set; }
        
        public required string AffectedEntityName { get; set; }
    }
}
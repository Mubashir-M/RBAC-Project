using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace RBacServer.Models
{
    public enum EventType
    {
        UserCreated,
        LoginSuccess,
        LoginFailure,
        UserUpdated,
        UserDeleted,
        RoleAssigned,
        RoleUpdated,

    }
    public class Event
    {
        public required Guid EventId { get; set; }

        public required EventType Type { get; set; }

        public required DateTime Timestamp { get; set; } = DateTime.UtcNow;

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        public required string Description { get; set; }

        public int? UserId { get; set; }

        [ForeignKey("UserId")]
        public User? User { get; set; }

        [StringLength(50)]
        public string? Username { get; set; }

        [StringLength(45)]
        public string? SourceIPAddress { get; set; }


        public int? AffectedEntityId { get; set; }
        
        public required string AffectedEntityName { get; set; }


    }
}
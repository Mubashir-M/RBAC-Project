using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace RBacServer.Models
{
    public class Project
    {
        [Key]
        public required Guid ProjectId { get; set; }

        [Required(ErrorMessage = "Creator User ID is required")]
        public int CreatorUserId { get; set; }

        [ForeignKey("CreatorUserId")]
        public required User CreatorUser { get; set; }

        [Required(ErrorMessage = "Description is required")]
        [StringLength(500, ErrorMessage = "Description cannot exceed 1000 characters")]
        public required string Description { get; set; }

        [StringLength(50, ErrorMessage = "Status cannot exceed 50 characters")]
        public required string Status { get; set; }

        public ICollection<ProjectParticipant> ProjectParticipants { get; set; } = new List<ProjectParticipant>();
    }
}
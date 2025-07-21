using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace RBacServer.Models
{
    public enum ProjectRole
    {
        TeamLead,
        Developer,
        Tester,
        Stakeholder,
        Viewer
    }
    public class ProjectParticipant
    {
        public required Guid ProjectId { get; set; }

        [ForeignKey("ProjectId")]
        public required Project Project { get; set; }

        public required int UserId { get; set; }

        [ForeignKey("UserId")]
        public required User User { get; set; }

        /* --- Attributes specific to the participation --- */

        [Required(ErrorMessage = "Role in project is required")]
        [StringLength(50, ErrorMessage = "Project role cannot exceed 50 characters")]
        public required string RoleInProject { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy HH:mm}", ApplyFormatInEditMode = true)]
        [Column(TypeName = "Text")]
        public DateTime JoinedDate { get; set; } = DateTime.UtcNow;
    }
}
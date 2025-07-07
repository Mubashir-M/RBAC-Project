using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RBacServer.Models
{
    public class Role
    {
        public int Id { get; set; } // Primary Key

        [Required(ErrorMessage = "Role name is required.")]
        [StringLength(50, ErrorMessage = "Role name cannot exceed 50 characters.")]
        [Display(Name = "Role Name")]
        public required string name { get; set; }

        [StringLength(250, ErrorMessage = "Description cannot exceed 250 characters.")]
        [Display(Name = "Description")]
        [DataType(DataType.MultilineText)]
        public string? Description { get; set; }

        // Navigation property for users (Many-to-Many relationship with User through UserRole)
        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();

        // Navigation property for permissions (Many-to-Many relationship with Permission through RolePermission)
        public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
    }
}
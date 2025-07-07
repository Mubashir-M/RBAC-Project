using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RBacServer.Models
{
    public class Permission
    {
        public int Id { get; set; } // Primary Key

        [Required(ErrorMessage = "Permission name is required.")]
        [StringLength(100, ErrorMessage = "Permission name cannot exceed 100 characters.")]
        [Display(Name = "Permission Name")]
        public required string Name { get; set; }

        [StringLength(250, ErrorMessage = "Description cannot exceed 250 characters.")]
        [Display(Name = "Description")]
        [DataType(DataType.MultilineText)]
        public string? Description { get; set; }


         // Navigation property for roles (Many-to-Many relationship with Role through RolePermission)
        public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
    }
}
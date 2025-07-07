using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace RBacServer.Models
{
    public class User
    {
        public int Id { get; set; }  // Primary Key

        [Required(ErrorMessage = "Username is required")]
        [StringLength(50, MinimumLength = 8, ErrorMessage = "Username must be between 8 and 50 characters.")]
        [Display(Name = "Username")]
        public required string Username { get; set; }

        [Required(ErrorMessage = "Email address is required")]
        [EmailAddress(ErrorMessage = "Invalid email address format.")]
        [StringLength(100, ErrorMessage = "Email cannot exceed 100 characters")]
        [Display(Name = "Email Address")]
        public required string Email { get; set; }

        [Required(ErrorMessage = "Password hash is required.")]
        [Column(TypeName = "TEXT")] // Use TEXT for SQLite compatibility
        public required string PasswordHash { get; set; }

        [Required(ErrorMessage = "Password salt is required.")]
        [Column(TypeName = "TEXT")] // Use TEXT for SQLite compatibility
        public required string PasswordHSalt { get; set; }

        [Display(Name = "First Name")]
        [StringLength(50)]
        public required string FirstName { get; set; }

        [Display(Name = "Last Name")]
        [StringLength(50)]
        public required string LastName { get; set; }

        [Display(Name = "Account Created On")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm}", ApplyFormatInEditMode = true)]
        [Column(TypeName = "TEXT")] // Use TEXT for datetime in SQLite
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        [Display(Name = "Last Login")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm}", ApplyFormatInEditMode = true)]
        [Column(TypeName = "TEXT")] // Use TEXT for datetime in SQLite
        public DateTime? LastLoginDate { get; set; }

        [Display(Name = "Is Active")]
        [Column(TypeName = "INTEGER")] // Use INTEGER for boolean in SQLite (0 or 1)
        public bool IsActive { get; set; } = true; // Default Active

        // Navigation property for roles
        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    }
}

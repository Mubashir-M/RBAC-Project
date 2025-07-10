using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RBacServer.Models;

namespace RBacServer.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {        
        } 
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
        
            base.OnModelCreating(modelBuilder);
        
            // Configure UserRole Many-to-Many
            modelBuilder.Entity<UserRole>()
            .HasKey(ur => new { ur.UserId, ur.RoleId });

            modelBuilder.Entity<UserRole>()
            .HasOne(ur => ur.User)
            .WithMany(u => u.UserRoles)
            .HasForeignKey(ur => ur.UserId);

            modelBuilder.Entity<UserRole>()
            .HasOne(ur => ur.Role)
            .WithMany(r => r.UserRoles)
            .HasForeignKey(ur => ur.RoleId);

            // Configure RolePermissions Many-to-Many
            modelBuilder.Entity<RolePermission>()
            .HasKey(rp => new { rp.RoleId, rp.PermissionId });

            modelBuilder.Entity<RolePermission>()
            .HasOne(rp => rp.Role)
            .WithMany(r => r.RolePermissions)
            .HasForeignKey(rp => rp.RoleId);

            modelBuilder.Entity<RolePermission>()
            .HasOne(rp => rp.Permission)
            .WithMany(p => p.RolePermissions)
            .HasForeignKey(rp => rp.PermissionId);


            // Seed Roles
            modelBuilder.Entity<Role>().HasData(
                new Role { Id= 1, name = "Admin", Description = "Administrator with full access"},
                new Role { Id= 2, name = "User",Description = "Regular user with limited access"}
            );

            // Seed Permissions
            modelBuilder.Entity<Permission>().HasData(
                new Permission {Id = 1, Name = "CreateUser", Description = "Access to create a user"},
                new Permission {Id = 2, Name = "DeleteUser", Description = "Access to delete a user"},
                new Permission {Id = 3, Name = "ViewDashboard", Description = "Access to view dashboard"},
                new Permission {Id = 4, Name = "EdiUser", Description = "Access to edit user"}
            );

            // Seed RlePermissions (RoleId, PermissionId)
            modelBuilder.Entity<RolePermission>().HasData(
                new RolePermission {RoleId = 1, PermissionId = 1},
                new RolePermission {RoleId = 1, PermissionId = 2},
                new RolePermission {RoleId = 1, PermissionId = 3},
                new RolePermission {RoleId = 1, PermissionId = 4},
                new RolePermission {RoleId = 2, PermissionId = 4}
            );
        }
    }
}
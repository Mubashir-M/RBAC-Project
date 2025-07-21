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
        
        public DbSet<Event> Events { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<ProjectParticipant> ProjectParticipants { get; set; }


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

            // Configure Event (One-to-Many with User)
            modelBuilder.Entity<Event>()
            .HasOne(e => e.User)
            .WithMany(u => u.Events)
            .HasForeignKey(e => e.UserId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.SetNull);

            // Configure Project (One-to-Many with CreatorUser)
            modelBuilder.Entity<Project>()
            .HasOne(p => p.CreatorUser)
            .WithMany(u => u.Projects)
            .HasForeignKey(p => p.CreatorUserId)
            .IsRequired();

            // Configure ProjectParticipant Many-to-Many (Project and User)
            modelBuilder.Entity<ProjectParticipant>()
            .HasKey(pp => new { pp.ProjectId, pp.UserId }); // Composite primary key

            // ProjectParticipant has one Project
            modelBuilder.Entity<ProjectParticipant>()
            .HasOne(pp => pp.Project)
            .WithMany(p => p.ProjectParticipants)
            .HasForeignKey(pp => pp.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

            // ProjectParticipant has one User
            modelBuilder.Entity<ProjectParticipant>()
            .HasOne(pp => pp.User)
            .WithMany(u => u.ProjectParticipants)
            .HasForeignKey(pp => pp.UserId)
            .OnDelete(DeleteBehavior.Cascade);

            // Seed Roles
            modelBuilder.Entity<Role>().HasData(
                new Role { Id = 1, name = "Admin", Description = "Administrator with full access" },
                new Role { Id = 2, name = "User", Description = "Regular user with limited access" },
                new Role { Id = 3, name = "Manager", Description = "Manager with access to users in a project" },
                new Role { Id = 4, name = "Pending", Description = "User has not been assigned a role" }
            );

            // Seed Permissions
            modelBuilder.Entity<Permission>().HasData(
                new Permission { Id = 1, Name = "CreateUser", Description = "Access to create a user" },
                new Permission { Id = 2, Name = "DeleteUser", Description = "Access to delete a user" },
                new Permission { Id = 3, Name = "ViewDashboard", Description = "Access to view dashboard" },
                new Permission { Id = 4, Name = "EditUser", Description = "Access to edit user" }
            );

            // Seed RlePermissions (RoleId, PermissionId)
            modelBuilder.Entity<RolePermission>().HasData(
                new RolePermission { RoleId = 1, PermissionId = 1 },
                new RolePermission { RoleId = 1, PermissionId = 2 },
                new RolePermission { RoleId = 1, PermissionId = 3 },
                new RolePermission { RoleId = 1, PermissionId = 4 },
                new RolePermission { RoleId = 2, PermissionId = 4 }
            );
        }
    }
}
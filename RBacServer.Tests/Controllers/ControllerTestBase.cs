using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using RBacServer.Data;
using RBacServer.Models;
using RBacServer.Services;
using System.Net;
using Microsoft.AspNetCore.Http;

namespace RBacServer.Tests.Controllers
{
    public abstract class ControllerTestBase : IDisposable
    {
        protected ApplicationDbContext _context;
        protected readonly Mock<TokenService> _mockTokenService;
        protected readonly Mock<EventLoggerService> _mockEventLogger;

        protected ControllerTestBase()
        {
            // Configure a real in-memory database for testing
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);
            _context.Database.EnsureCreated();

            // Seed the in-memory database with test data
            SeedDatabase();

            // --- Mocking TokenService and its dependencies ---
            var mockConfiguration = new Mock<IConfiguration>();
            mockConfiguration.Setup(c => c["Jwt:Key"]).Returns("supersecretkeythatisatleast32characterslongforsecurity");
            mockConfiguration.Setup(c => c["Jwt:Issuer"]).Returns("your_rbac_issuer");
            mockConfiguration.Setup(c => c["Jwt:Audience"]).Returns("your_rbac_audience");

            // Pass the REAL in-memory context to the TokenService mock
            // This allows the TokenService logic to query real data if needed
            _mockTokenService = new Mock<TokenService>(
                mockConfiguration.Object,
                _context
            );
            _mockTokenService.Setup(ts => ts.CreateToken(It.IsAny<User>()))
                             .Returns("mock_jwt_token_for_user");
            
            // --- Mocking EventLoggerService ---
            var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            var mockHttpContext = new Mock<HttpContext>();
            var mockConnection = new Mock<ConnectionInfo>();
            mockConnection.Setup(c => c.RemoteIpAddress).Returns(IPAddress.Parse("127.0.0.1"));
            mockHttpContext.Setup(c => c.Connection).Returns(mockConnection.Object);
            mockHttpContextAccessor.Setup(a => a.HttpContext).Returns(mockHttpContext.Object);

            _mockEventLogger = new Mock<EventLoggerService>(
                _context,
                mockHttpContextAccessor.Object
            );
        }

        protected void SeedDatabase()
        {
            // Clear and reset the database for each test class
            _context.Users.RemoveRange(_context.Users);
            _context.Roles.RemoveRange(_context.Roles);
            _context.UserRoles.RemoveRange(_context.UserRoles);
            _context.Permissions.RemoveRange(_context.Permissions);
            _context.RolePermissions.RemoveRange(_context.RolePermissions);
            _context.SaveChanges();

            // Add roles
            var pendingRole = new Role { Id = 101, name = "Pending", Description = "New User Pending Role" };
            var userRole = new Role { Id = 102, name = "User", Description = "Standard User Role" };
            var adminRole = new Role { Id = 103, name = "Admin", Description = "Administrator Role" };
            _context.Roles.AddRange(pendingRole, userRole, adminRole);
            _context.SaveChanges();

            // Add permissions
            var viewUsersPermission = new Permission { Id = 1, Name = "ViewUsers" };
            var manageUsersPermission = new Permission { Id = 2, Name = "ManageUsers" };
            _context.Permissions.AddRange(viewUsersPermission, manageUsersPermission);
            _context.SaveChanges();

            // Add role permissions
            var adminManageUsers = new RolePermission { RoleId = adminRole.Id, PermissionId = manageUsersPermission.Id };
            _context.RolePermissions.Add(adminManageUsers);
            _context.SaveChanges();

            // Add users with hashed passwords
            const string TestUserPassword = "TestPassword";
            const string ExistingUserPassword = "Password123!";

            var existingUserSalt = BCrypt.Net.BCrypt.GenerateSalt();
            var existingUser = new User
            {
                Id = 1,
                Username = "existinguser",
                Email = "existing@example.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(ExistingUserPassword, existingUserSalt),
                PasswordHSalt = existingUserSalt,
                FirstName = "Existing",
                LastName = "User",
                CreatedDate = DateTime.UtcNow,
                IsActive = true
            };

            var testUserSalt = BCrypt.Net.BCrypt.GenerateSalt();
            var testUser = new User
            {
                Id = 2,
                Username = "testuser",
                Email = "test@example.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(TestUserPassword, testUserSalt),
                PasswordHSalt = testUserSalt,
                FirstName = "Test",
                LastName = "User",
                CreatedDate = DateTime.UtcNow,
                IsActive = true
            };
            _context.Users.AddRange(existingUser, testUser);
            _context.SaveChanges();

            // Add user roles
            _context.UserRoles.Add(new UserRole { UserId = existingUser.Id, RoleId = userRole.Id });
            _context.UserRoles.Add(new UserRole { UserId = existingUser.Id, RoleId = adminRole.Id });
            _context.UserRoles.Add(new UserRole { UserId = testUser.Id, RoleId = userRole.Id });
            _context.SaveChanges();
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}
using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using RBacServer.Controllers;
using RBacServer.Data;
using RBacServer.Models;
using RBacServer.Models.DTOs;
using RBacServer.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using RBacServer.DTOs;
using System.Net;
using System.Text.Json;


public static class DbSetMocking
{
    public static Mock<DbSet<T>> GetMockDbSet<T>(IQueryable<T> data) where T : class
    {
        var mockSet = new Mock<DbSet<T>>();
        mockSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(data.Provider);
        mockSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(data.Expression);
        mockSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(data.ElementType);
        mockSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());
        return mockSet;
    }
}

namespace RBacServer.Tests.Controllers
{
    public class AuthControllerTests : IDisposable
    {
        private ApplicationDbContext _context; // Now a real in-memory context
        private readonly Mock<TokenService> _mockTokenService;
        private readonly Mock<EventLoggerService> _mockEventLogger;
        private readonly AuthController _controller;

        public AuthControllerTests()
        {
            // Configure in-memory databse
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);
            _context.Database.EnsureCreated();

            // Seed in-memory database with test data
            SeedDataBase();

            // --- Mocking TokenService and its dependencies ---
            // 1. Mock IConfiguration
            var mockConfiguration = new Mock<IConfiguration>();
            // Setup a dummy JWT Key and Issuer (must match your appsettings/config expectations)
            mockConfiguration.Setup(c => c["Jwt:Key"]).Returns("supersecretkeythatisatleast32characterslongforsecurity");
            mockConfiguration.Setup(c => c["Jwt:Issuer"]).Returns("your_rbac_issuer");
            mockConfiguration.Setup(c => c["Jwt:Audience"]).Returns("your_rbac_audience");


            // 2. Mock ApplicationDbContext for TokenService's constructor
            var mockTokenServiceDbContext = new Mock<ApplicationDbContext>(new DbContextOptions<ApplicationDbContext>());

            // Provide some mock roles for the TokenService to find
            var tokenServiceRoles = new List<Role>
            {
                new Role { Id = 101, name = "Pending", Description = "..." },
                new Role { Id = 102, name = "User", Description = "..." },
                new Role { Id = 103, name = "Admin", Description = "..." }
            }.AsQueryable();
            var mockTokenServiceRolesDbSet = DbSetMocking.GetMockDbSet(tokenServiceRoles);
            mockTokenServiceDbContext.Setup(c => c.Roles).Returns(mockTokenServiceRolesDbSet.Object);


            // 3. Create the TokenService mock by passing the mocks of its dependencies
            _mockTokenService = new Mock<TokenService>(
                mockConfiguration.Object,
                mockTokenServiceDbContext.Object
            );

            // Set up what CreateToken should return when called by the controller
            _mockTokenService.Setup(ts => ts.CreateToken(It.IsAny<User>()))
                             .Returns("mock_jwt_token_for_user");


            // --- Mocking EventLoggerService ---
            var mockEventLoggerContext = new Mock<ApplicationDbContext>(new DbContextOptions<ApplicationDbContext>());

            // Mock IHttpContextAccessor and its nested properties
            var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            var mockHttpContext = new Mock<HttpContext>();
            var mockConnection = new Mock<ConnectionInfo>();

            // Setup the IP address that LogEvent tries to access
            mockConnection.Setup(c => c.RemoteIpAddress).Returns(IPAddress.Parse("127.0.0.1"));
            mockHttpContext.Setup(c => c.Connection).Returns(mockConnection.Object);
            mockHttpContextAccessor.Setup(a => a.HttpContext).Returns(mockHttpContext.Object);

            // Create the EventLoggerService mock
            _mockEventLogger = new Mock<EventLoggerService>(
                mockEventLoggerContext.Object, // Pass the mocked ApplicationDbContext
                mockHttpContextAccessor.Object // Pass the mocked IHttpContextAccessor
            );

            // Instantiate the controller with actual DbContext and mocked services
            _controller = new AuthController(
                _context,
                _mockTokenService.Object,
                _mockEventLogger.Object
            );


        }

        private void SeedDataBase()
        {
            _context.Users.RemoveRange(_context.Users);
            _context.Roles.RemoveRange(_context.Roles);
            _context.UserRoles.RemoveRange(_context.UserRoles);
            _context.Permissions.RemoveRange(_context.Permissions);
            _context.RolePermissions.RemoveRange(_context.RolePermissions);
            _context.SaveChanges(); // Clear existing data

            var pendingRole = new Role { Id = 101, name = "Pending", Description = "New User Pending Role" };
            var userRole = new Role { Id = 102, name = "User", Description = "Standard User Role" };
            var adminRole = new Role { Id = 103, name = "Admin", Description = "Administrator Role" };
            _context.Roles.AddRange(pendingRole, userRole, adminRole);
            _context.SaveChanges(); // Save roles first, so they have IDs

            var viewUsersPermission = new Permission { Id = 1, Name = "ViewUsers" };
            var manageUsersPermission = new Permission { Id = 2, Name = "ManageUsers" };
            _context.Permissions.AddRange(viewUsersPermission, manageUsersPermission);
            _context.SaveChanges(); // Save permissions

            var adminManageUsers = new RolePermission { RoleId = adminRole.Id, PermissionId = manageUsersPermission.Id };
            _context.RolePermissions.Add(adminManageUsers);
            _context.SaveChanges(); // Save role permissions

            // Define the password string to ensure consistency
            const string TestUserPassword = "TestPassword";
            const string ExistingUserPassword = "Password123!";

            var existingUserSalt = BCrypt.Net.BCrypt.GenerateSalt();
            var existingUser = new User
            {
                Id = 1, Username = "existinguser", Email = "existing@example.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(ExistingUserPassword, existingUserSalt), // Use generated salt
                PasswordHSalt = existingUserSalt,
                FirstName = "Existing", LastName = "User", CreatedDate = DateTime.UtcNow, IsActive = true
            };
            _context.Users.Add(existingUser);

            var testUserSalt = BCrypt.Net.BCrypt.GenerateSalt();
            var testUser = new User
            {
                Id = 2, Username = "testuser", Email = "test@example.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(TestUserPassword, testUserSalt), // Use generated salt
                PasswordHSalt = testUserSalt,
                FirstName = "Test", LastName = "User", CreatedDate = DateTime.UtcNow, IsActive = true
            };
            _context.Users.Add(testUser);
            _context.SaveChanges(); // Save users so they get their IDs assigned by EF Core

            _context.UserRoles.Add(new UserRole { UserId = existingUser.Id, RoleId = userRole.Id });
            _context.UserRoles.Add(new UserRole { UserId = testUser.Id, RoleId = userRole.Id });
            _context.UserRoles.Add(new UserRole { UserId = existingUser.Id, RoleId = adminRole.Id });

            _context.SaveChanges(); // Final save for UserRoles
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }


        [Fact]
        public async Task Register_validData_REturnsOk()
        {
            // Arrange
            var registerDto = new RegisterDto
            {
                Username = "newuser",
                Email = "newuser@example.com",
                Password = "NewSecurePassword123!",
                FirstName = "New",
                LastName = "User"
            };

            // Act
            var result = await _controller.Register(registerDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("User registered successfully.", okResult.Value);

            // Verify user was added to the in-memory database
            var addedUser = await _context.Users.FirstOrDefaultAsync(u => u.Username == registerDto.Username);
            Assert.NotNull(addedUser);
            Assert.True(BCrypt.Net.BCrypt.Verify(registerDto.Password, addedUser.PasswordHash));

            // Verify user role was added
            var pendingRole = await _context.Roles.FirstOrDefaultAsync(r => r.name == "Pending");
            Assert.NotNull(pendingRole);
            var userRole = await _context.UserRoles.FirstOrDefaultAsync(ur => ur.UserId == addedUser.Id && ur.RoleId == pendingRole.Id);
            Assert.NotNull(userRole);

            // Verify LogEvent was called
            _mockEventLogger.Verify(el => el.LogEvent(
                EventType.UserCreated,
                $"User '{addedUser.Username}' registered successfully and assined 'Pending' role.",
                addedUser.Id,
                addedUser.Username,
                addedUser.Id,
                addedUser.Username
            ), Times.Once);
        }

        [Fact]
        public async Task Register_UserWithExistingUsename_ReturnsBadRequest()
        {
            var registerDto = new RegisterDto
            {
                Username = "existinguser",
                Email = "another@example.com",
                Password = "Password123!",
                FirstName = "Newer",
                LastName = "Users"
            };

            var result = await _controller.Register(registerDto);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("User with this username or email already exists.", badRequestResult.Value);

            Assert.Null(await _context.Users.FirstOrDefaultAsync(u => u.Email == registerDto.Email));

            _mockEventLogger.Verify(el => el.LogEvent(It.IsAny<EventType>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>()), Times.Never);

        }

        [Fact]
        public async Task Register_UserWithExistingEmail_ReturnsBadRequest()
        {
            var registerDto = new RegisterDto
            {
                Username = "anothernewuser",
                Email = "existing@example.com",
                Password = "Password123!",
                FirstName = "",
                LastName = ""
            };

            var result = await _controller.Register(registerDto);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("User with this username or email already exists.", badRequestResult.Value);

            Assert.Null(await _context.Users.FirstOrDefaultAsync(u => u.Username == registerDto.Username));
            _mockEventLogger.Verify(el => el.LogEvent(It.IsAny<EventType>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>()), Times.Never);

        }

        [Fact]
        public async Task Register_PendingRoleNotFound_ReturnsInternalServerError()
        {
            var registerDto = new RegisterDto
            {
                Username = "userWithoutRole",
                Email = "userWithoutRole@example.com",
                Password = "Password123!",
                FirstName = "No",
                LastName = "Role"
            };

            var pendingRole = _context.Roles.FirstOrDefault(r => r.name == "Pending");

            if (pendingRole != null)
            {
                _context.Roles.Remove(pendingRole);
                _context.SaveChanges();

            }

            var result = await _controller.Register(registerDto);

            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);
            Assert.Equal("Default role not found.", statusCodeResult.Value);

            var addedUser = await _context.Users.FirstOrDefaultAsync(u => u.Username == registerDto.Username);
            Assert.NotNull(addedUser);
            Assert.Null(await _context.UserRoles.FirstOrDefaultAsync(ur => ur.UserId == addedUser.Id));

            _mockEventLogger.Verify(el => el.LogEvent(It.IsAny<EventType>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task Login_ValidCredentials_ReturnsOkWithTokenAndUser()
        {
            var loginDto = new LoginDto { Username = "testuser", Password = "TestPassword" };
            var expectedToken = "mock_jwt_token_for_testuser";

            _mockTokenService.Setup(ts => ts.CreateToken(
                It.Is<User>(u => u.Username == loginDto.Username)
            )).Returns(expectedToken);

            var result = await _controller.Login(loginDto);

            var okResult = Assert.IsType<OkObjectResult>(result);
            
            Assert.NotNull(okResult.Value);

            var returnValue = Assert.IsType<LoginResponseDto>(okResult.Value);

            Assert.Equal(expectedToken, returnValue.token);
            Assert.Equal("testuser", returnValue.user.Username);
            Assert.Equal("test@example.com", returnValue.user.Email);
            Assert.Equal(2, returnValue.user.Id);

            Assert.NotNull(returnValue.user.Roles);
            var rolesList = returnValue.user.Roles.ToList();
            Assert.Single(rolesList);
            Assert.Equal("User", rolesList[0].Name);

            var updatedUser = await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Username == loginDto.Username);
            Assert.NotNull(updatedUser);
            Assert.NotNull(updatedUser.LastLoginDate);

            _mockEventLogger.Verify(el => el.LogEvent(
                EventType.LoginSuccess,
                $"User '{loginDto.Username}' logged in successfully.",
                It.IsAny<int>(),
                loginDto.Username,
                It.IsAny<int>(),
                loginDto.Username
            ), Times.Once);
        }

        [Fact]
        public async Task Login_InvalidUsername_ReturnsUnauthorized()
        {
            var loginDto = new LoginDto { Username = "nonexistent", Password = "AnyPassword" };

            var result = await _controller.Login(loginDto);

            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
            Assert.Equal("Invalid credentials.", unauthorizedResult.Value);

            _mockEventLogger.Verify(el => el.LogEvent(It.IsAny<EventType>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task Login_InvalidPassword_ReturnsUnauthorized()
        {
            var loginDto = new LoginDto { Username = "testuser", Password = "WrongPassword" };

            var result = await _controller.Login(loginDto);

            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
            Assert.Equal("Invalid credentials.", unauthorizedResult.Value);

            _mockEventLogger.Verify(el => el.LogEvent(It.IsAny<EventType>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>()), Times.Never);
        }
    }
}
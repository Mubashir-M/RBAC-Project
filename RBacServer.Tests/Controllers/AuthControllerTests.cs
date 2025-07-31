using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RBacServer.Controllers;
using RBacServer.Models;
using RBacServer.Models.DTOs;
using RBacServer.DTOs;

namespace RBacServer.Tests.Controllers
{
    public class AuthControllerTests : ControllerTestBase
    {
        private readonly AuthController _controller;

        public AuthControllerTests()
        {
            // Instantiate the controller with actual Db_context and mocked services
            _controller = new AuthController(
                _context,
                _mockTokenService.Object,
                _mockEventLogger.Object
            );
        }


        [Fact]
        public async Task Register_validData_ReturnsOk()
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
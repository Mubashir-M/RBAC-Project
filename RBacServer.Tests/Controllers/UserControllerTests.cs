using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using RBacServer.Controllers;
using RBacServer.Models.DTOs;

namespace RBacServer.Tests.Controllers
{
    public class UserControllerTests : ControllerTestBase
    {
        private readonly UserController _controller;

        public UserControllerTests()
        {
            _controller = new UserController(
                _context,
                _mockEventLogger.Object
            );
        }
        [Fact]
        public async Task Get_User_ReturnOk()
        {
            var UserToGetId = 2;
            var result = await _controller.get_user(UserToGetId);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<UserWithAccessDto>(okResult.Value);

            Assert.Equal("testuser", returnValue.Username);
            Assert.Equal("test@example.com", returnValue.Email);
            Assert.Equal("Test", returnValue.FirstName);
            Assert.Equal("User", returnValue.LastName);
            Assert.Equal(UserToGetId, returnValue.UserId);


        }

        [Fact]
        public async Task GetUsers_ReturnsAllUsers()
        {
            var result = await _controller.GetUsers();

            var okResult = Assert.IsType<OkObjectResult>(result);
            var users = Assert.IsAssignableFrom<IEnumerable<UserWithAccessDto>>(okResult.Value);

            Assert.Contains(users, u => u.Username == "testuser");
            Assert.Contains(users, u => u.Username == "existinguser");
        }

        [Fact]
        public async Task UpdateUserRole_WithValidRole_ShouldUpdateAndReturnOk()
        {
            var userId = 1;
            var userName = "existinguser";
            var newRoleId = 101;
            var oldRoleName = "User";
            var newRoleName = "Pending";

            var updateDto = new RoleDto { RoleId = newRoleId, Name = newRoleName };

            var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            var mockHttpContext = new Mock<HttpContext>();
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Name, userName)
            }; ;

            var identity = new ClaimsIdentity(claims, "Bearer");
            var principal = new ClaimsPrincipal(identity);

            mockHttpContext.Setup(c => c.User).Returns(principal);
            mockHttpContextAccessor.Setup(a => a.HttpContext).Returns(mockHttpContext.Object);

            _controller.ControllerContext.HttpContext = mockHttpContext.Object;

            var result = await _controller.UpdateUserRole(userId, updateDto);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = okResult.Value;

            Assert.Contains("Role updated successfully", returnValue?.ToString());

            var user = await _controller.get_user(userId);
            var userOkResult = Assert.IsType<OkObjectResult>(user);
            var userReturnValue = Assert.IsType<UserWithAccessDto>(userOkResult.Value);

            Assert.Contains(userReturnValue.Roles, r => r.Name == newRoleName);
            Assert.Contains(userReturnValue.Roles, r => r.RoleId == newRoleId);

            // event logging test
            _mockEventLogger.Verify(el => el.LogEvent(
                Models.EventType.RoleUpdated,
                It.Is<string>(msg => msg.Contains($"from '{oldRoleName}' to '{newRoleName}'")),
                It.IsAny<int?>(),
                It.IsAny<string>(),
                It.IsAny<int?>(),
                It.IsAny<string>()
            ), Times.Once());

        }

    }
}
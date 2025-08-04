using Microsoft.AspNetCore.Mvc;
using RBacServer.Controllers;
using RBacServer.Models;

namespace RBacServer.Tests.Controllers
{
    public class RoleControllerTests : ControllerTestBase
    {
        private readonly RoleController _controller;

        public RoleControllerTests()
        {
            _controller = new RoleController(_context);
        }

        [Fact]
        public async Task GetRoles_ReturnOk()
        {
            var result = await _controller.GetRoles();
            var okResult = Assert.IsType<OkObjectResult>(result);
            var roles = Assert.IsAssignableFrom<IEnumerable<Role>>(okResult.Value);

            Assert.NotEmpty(roles);
            Assert.Contains(roles, r => r.name == "Admin");
            Assert.Contains(roles, r => r.name == "User");
        }
    }
}
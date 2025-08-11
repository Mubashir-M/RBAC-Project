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

        [Fact]
        public async Task CreateRole_ReturnsCreatedRole()
        {
            var newRole = new Role
            {
                name = "Tester",
                Description = "Role for testing purposes"
            };

            var result = await _controller.CreateRole(newRole);

            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            var createdRole = Assert.IsType<Role>(createdResult.Value);

            Assert.Equal(newRole.name, createdRole.name);
            Assert.Equal(newRole.Description, createdRole.Description);
            Assert.True(createdRole.Id > 0);

            var roleInDb = await _context.Roles.FindAsync(createdRole.Id);
            Assert.NotNull(roleInDb);
            Assert.Equal(newRole.name, roleInDb.name);
        }
    }
}
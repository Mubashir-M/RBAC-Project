using Microsoft.AspNetCore.Mvc;
using RBacServer.Controllers;
using RBacServer.Models;
using RBacServer.Models.DTOs;
using Moq;
using RBacServer.Data;

namespace RBacServer.Tests.Controllers
{
    public class EventControllerTests : ControllerTestBase
    {
        private readonly EventController _controller;

        public EventControllerTests()
        {
            _controller = new EventController(_context);
        }

        [Fact]
        public async Task GetEvents_ReturnsOkResult_WithEvents()
        {
            // Arrange
            _context.Events.AddRange(
                new Event
                {
                    EventId = Guid.NewGuid(),
                    Type = EventType.RoleUpdated,
                    Timestamp = DateTime.UtcNow,
                    Description = "Test description",
                    UserId = 1,
                    Username = "admin",
                    SourceIPAddress = "127.0.0.1",
                    AffectedEntityId = 2,
                    AffectedEntityName = "User"
                },
                new Event
                {
                    EventId = Guid.NewGuid(),
                    Type = EventType.LoginSuccess,
                    Timestamp = DateTime.UtcNow.AddMinutes(-5),
                    Description = "Login successful",
                    UserId = 2,
                    Username = "user1",
                    SourceIPAddress = "127.0.0.2",
                    AffectedEntityId = null,
                    AffectedEntityName = ""
                }
            );
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.GetEvents();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var events = Assert.IsAssignableFrom<IEnumerable<EventDto>>(okResult.Value);
            Assert.Equal(2, ((List<EventDto>)events).Count);
        }

    }
}

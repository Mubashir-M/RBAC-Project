using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RBacServer.Models.DTOs
{
    public class UserWithAccessDto
    {
        public required int UserId { get; set; }
        public required string Username { get; set; }
        public required string FirstName { get; set; }

        public required string LastName { get; set; }

        public required string Email { get; set; }
        public required List<RoleDto> Roles { get; set; }
        public required List<string> Permissions { get; set; }

        public required bool isActive { get; set; }

    }
}
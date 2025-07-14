using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RBacServer.Models.DTOs
{
    public class UserWithAccessDto
    {
        public required string Username { get; set; }
        public required string FirstName { get; set; }

        public required string LastName { get; set; }
        public required List<string> Roles { get; set; }
        public required List<string> Permissions { get; set; }

    }
}
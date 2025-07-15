using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RBacServer.Models.DTOs
{
    public class RoleDto
    {
        public required int RoleId { get; set; }
        public required string Name { get; set; }
    }
}
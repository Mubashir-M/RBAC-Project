using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RBacServer.Models
{
    public class UserRole
    {
        // Composite Primary Key
        public int UserId { get; set; }
        public int RoleId { get; set; }

        // Navigation properties
        public  User? User { get; set; }
        public  Role? Role { get; set; }
    }
}
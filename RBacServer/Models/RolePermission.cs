using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RBacServer.Models
{
    public class RolePermission
    {
        // Composite Primary Key (can be configured in DbContext)
        public int RoleId { get; set; }
        public int PermissionId { get; set; }

        // Navigation properties
        public Role? Role { get; set; }
        public Permission? Permission { get; set; }
    }
}
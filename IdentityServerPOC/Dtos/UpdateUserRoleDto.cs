using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityServerPOC.Dtos
{
    public class UpdateUserRoleDto
    {
        public string UserId { get; set; }
        public string Role { get; set; }
    }
}

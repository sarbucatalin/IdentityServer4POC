using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace IdentityServerPOC.Infrastructure
{
    public class ApplicationUser : IdentityUser
    {
        // Add additional profile data for application users by adding properties to this class
        public string Name { get; set; }

    }

}

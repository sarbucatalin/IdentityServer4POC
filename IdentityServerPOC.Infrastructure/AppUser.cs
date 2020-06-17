﻿using Microsoft.AspNetCore.Identity;

namespace IdentityServerPOC.Infrastructure
{
    public class AppUser : IdentityUser
    {
        // Add additional profile data for application users by adding properties to this class
        public string Name { get; set; }
    }
}

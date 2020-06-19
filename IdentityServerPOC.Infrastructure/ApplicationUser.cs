using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace IdentityServerPOC.Infrastructure
{
    public class ApplicationUser : IdentityUser
    {
        // Add additional profile data for application users by adding properties to this class
        public string Name { get; set; }
        public ICollection<ApplicationUserRole> UserRoles { get; set; }
    }

    public class ApplicationUserRole : IdentityUserRole<string>
    {
        public virtual ApplicationUser User { get; set; }
        public virtual ApplicationRole Role { get; set; }
    }
    public class ApplicationRole : IdentityRole
    {
        public ICollection<ApplicationUserRole> UserRoles { get; set; }
    }
}

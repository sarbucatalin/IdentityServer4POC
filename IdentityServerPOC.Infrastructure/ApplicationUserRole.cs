using Microsoft.AspNet.Identity.EntityFramework;

namespace IdentityServerPOC.Infrastructure
{
    public class ApplicationUserRole : IdentityUserRole
    {
        public ApplicationUser User { get; set; }
        public ApplicationRole Role { get; set; }
    }
}

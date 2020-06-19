using IdentityServerPOC.Infrastructure;
using System;
using System.Linq;

namespace IdentityServerPOC.Dtos
{
    public class UserViewModel
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public bool IsLocked { get; set; }
        public string Role { get; set; }

        public UserViewModel(ApplicationUser user)
        {
            Id = user.Id;
            UserName = user.UserName;
            Email = user.Email;
            Name = user.Name;
            IsLocked = user.LockoutEnabled;
            Role = user.UserRoles != null ? user.UserRoles.FirstOrDefault().Role.Name : string.Empty;
        }
    }
}

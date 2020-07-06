using IdentityServerPOC.Infrastructure;
using System;

namespace UserManagement.Api.Models
{
    public class UserViewModel
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public bool IsLocked { get; set; }
        public string RoleId { get; set; }

        public UserViewModel(ApplicationUser user)
        {
            Id = user.Id;
            UserName = user.UserName;
            Email = user.Email;
            Name = user.Name;
            IsLocked = user.LockoutEnd > DateTimeOffset.Now;

            //Role = user.Roles.Any() ? user.Roles.FirstOrDefault().ToString() : string.Empty;
        }

        public UserViewModel(ApplicationUser user, string roleId) : this(user)
        {
            RoleId = roleId;
        }
    }
}

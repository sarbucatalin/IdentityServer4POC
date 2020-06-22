using IdentityServerPOC.Infrastructure;

namespace IdentityServerPOC.Dtos
{
    public class UserDto
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public bool IsLocked { get; set; }
        public string RoleId { get; set; }


        public UserDto(AppUser user, string roleId)
        {
            Id = user.Id;
            UserName = user.UserName;
            Email = user.Email;
            Name = user.Name;
            IsLocked = user.LockoutEnabled;
            RoleId = roleId;
        }
    }
}

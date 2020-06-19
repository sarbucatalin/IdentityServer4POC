using IdentityServerPOC.Dtos;
using IdentityServerPOC.Infrastructure;
using IdentityServerPOC.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityServerPOC.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class UsersController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UsersController(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [HttpGet]
        public async Task<List<UserDto>> GetUsersAsync()
        {
            var returnUsers = new List<UserDto>();
            IEnumerable<AppUser> appUsers = await _userManager.Users.ToListAsync();
            foreach (var user in appUsers)
            {
                var role = (await _userManager.GetRolesAsync(user)).FirstOrDefault();
                var roleId = _roleManager.Roles.FirstOrDefault(ir => ir.Name == role)?.Id;
                returnUsers.Add(new UserDto(user, roleId));
            }
            return returnUsers;
        }

        [HttpGet("{userId}")]
        public async Task<UserDto> GetUserByAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            var roles = await _userManager.GetRolesAsync(user);

            return new UserDto(user, roles.FirstOrDefault());
        }

        [HttpPost]
        public async Task<IActionResult> Register([FromBody] RegisterRequestViewModel model)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            //if(!string.IsNullOrEmpty(model.Role) && !await _roleManager.RoleExistsAsync(model.Role))
            //{
            //    return BadRequest();
            //}

            var user = new AppUser { UserName = model.Email, Name = model.Name, Email = model.Email };

            var result = await _userManager.CreateAsync(user, model.Password);


            if (!result.Succeeded) return BadRequest(result.Errors);

            await _userManager.AddClaimAsync(user, new System.Security.Claims.Claim("userName", user.UserName));
            await _userManager.AddClaimAsync(user, new System.Security.Claims.Claim("name", user.Name));
            await _userManager.AddClaimAsync(user, new System.Security.Claims.Claim("email", user.Email));
            await _userManager.AddClaimAsync(user, new System.Security.Claims.Claim("role", Roles.SuperAdmin));
            await _userManager.AddToRoleAsync(user, Roles.SuperAdmin);

            return Ok(new RegisterResponseViewModel(user));
        }

        [HttpPut]
        public async Task<IActionResult> UpdateRole(UpdateUserRoleDto updateUserRole)
        {
            if (updateUserRole.Role.ToLower() == Roles.SuperAdmin)
                return Forbid();

            var user = await _userManager.FindByIdAsync(updateUserRole.UserId);
            if (user == null && !await _roleManager.RoleExistsAsync(updateUserRole.Role))
                return BadRequest();

            var existingRoles = await _userManager.GetRolesAsync(user);
            if (existingRoles.Any())
                await _userManager.RemoveFromRolesAsync(user, existingRoles);

            var result = await _userManager.AddToRoleAsync(user, updateUserRole.Role);
            if (result.Succeeded) return Ok();
            return BadRequest(result.Errors);
        }

    }
}

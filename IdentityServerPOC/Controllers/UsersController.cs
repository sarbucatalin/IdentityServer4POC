using IdentityServerPOC.Dtos;
using IdentityServerPOC.Infrastructure;
using IdentityServerPOC.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Internal;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace IdentityServerPOC.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class UsersController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UsersController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [HttpGet]
        public async Task<List<UserViewModel>> GetUsersAsync()
        {
            IEnumerable<ApplicationUser> users = await _userManager.Users.ToListAsync();

                    
            return users.Select(user => new UserViewModel(user)).ToList();
        }

        [HttpGet("{userId}")]
        public async Task<UserViewModel> GetUserByAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
           
            return new UserViewModel(user);
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterRequestViewModel model)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!string.IsNullOrEmpty(model.Role) && !await _roleManager.RoleExistsAsync(model.Role))
            {
                return BadRequest();
            }

            var user = new ApplicationUser { UserName = model.Email, Name = model.Name, Email = model.Email };

            var result = await _userManager.CreateAsync(user, model.Password);
            

            if (!result.Succeeded) return BadRequest(result.Errors);

            await _userManager.AddClaimAsync(user, new System.Security.Claims.Claim("userName", user.UserName));
            await _userManager.AddClaimAsync(user, new System.Security.Claims.Claim("name", user.Name));
            await _userManager.AddClaimAsync(user, new System.Security.Claims.Claim("email", user.Email));
            await _userManager.AddClaimAsync(user, new System.Security.Claims.Claim("role", model.Role));
            await _userManager.AddToRoleAsync(user, model.Role);

            return Ok(new RegisterResponseViewModel(user));
        }

        [HttpPut]
        public async Task<IActionResult> UpdateRole(UpdateUserRoleViewModel updateUserRole)
        {
            if (updateUserRole.Role.ToLower() == Roles.SuperAdmin)
                return Forbid();

            var user = await _userManager.FindByIdAsync(updateUserRole.UserId);
            if (user == null && !await _roleManager.RoleExistsAsync(updateUserRole.Role))
                return BadRequest();

            var existingRoles = await _userManager.GetRolesAsync(user);
            if (existingRoles.Any())
                await _userManager.RemoveFromRolesAsync(user, existingRoles);

            var result =  await _userManager.AddToRoleAsync(user, updateUserRole.Role);
            if (result.Succeeded) return Ok();
            return BadRequest(result.Errors);
        }

     }
}

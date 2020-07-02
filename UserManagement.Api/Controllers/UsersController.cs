using IdentityServerPOC.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityModel;
using UserManagement.Api.Models;

namespace UserManagement.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "SuperAdmin")]
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
            var returnUsers = new List<UserViewModel>();
            IEnumerable<ApplicationUser> appUsers = await _userManager.Users.ToListAsync();
            foreach (var user in appUsers)
            {
                var role = (await _userManager.GetRolesAsync(user)).FirstOrDefault();
                var roleId = _roleManager.Roles.FirstOrDefault(ir => ir.Name == role)?.Id;
                returnUsers.Add(new UserViewModel(user, roleId));
            }

            return returnUsers;
        }

        [HttpGet("{userId}")]
        public async Task<UserViewModel> GetUserByAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            var role = (await _userManager.GetRolesAsync(user)).FirstOrDefault();
            var roleId = _roleManager.Roles.FirstOrDefault(ir => ir.Name == role)?.Id;

            return new UserViewModel(user, roleId);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterRequestViewModel model)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var role = await _roleManager.FindByIdAsync(model.RoleId);
            if (role == null)
            {
                return BadRequest();
            }

            var user = new ApplicationUser { UserName = model.Email, Name = model.Name, Email = model.Email };
            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded) return BadRequest(result.Errors);


            await _userManager.AddClaimAsync(user, new System.Security.Claims.Claim("userName", user.UserName));
            await _userManager.AddClaimAsync(user, new System.Security.Claims.Claim(JwtClaimTypes.Name, user.Name));
            await _userManager.AddClaimAsync(user, new System.Security.Claims.Claim(JwtClaimTypes.Email, user.Email));
            await _userManager.AddClaimAsync(user, new System.Security.Claims.Claim(JwtClaimTypes.Role, role.Name));
            await _userManager.AddToRoleAsync(user, role.Name);

            return Ok(new RegisterResponseViewModel(user));
        }

        [HttpPut]
        public async Task<IActionResult> UpdateRole(UpdateUserRoleViewModel updateUserRole)
        {

            var user = await _userManager.FindByIdAsync(updateUserRole.UserId);
            if (user == null)
                return BadRequest();

            var role = await _roleManager.FindByIdAsync(updateUserRole.RoleId);
            if (role == null)
                return BadRequest();

            if (role.Name.ToLower() == Roles.SuperAdmin)
                return Forbid();

            var existingRoles = await _userManager.GetRolesAsync(user);
            if (existingRoles.Any())
                await _userManager.RemoveFromRolesAsync(user, existingRoles);

            var result = await _userManager.AddToRoleAsync(user, role.Name);
            if (result.Succeeded) return Ok();
            return BadRequest(result.Errors);
        }

        [HttpPut]
        [Route("{userId}/lock")]
        public async Task<IActionResult> LockUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return BadRequest();

            user.LockoutEnd = DateTime.MaxValue;

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                return Ok();
            }
            return BadRequest(result.Errors);
        }

        [HttpPut]
        [Route("{userId}/unlock")]
        public async Task<IActionResult> Unlock(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return BadRequest();

            user.LockoutEnd = DateTime.Now;

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                return Ok();
            }
            return BadRequest(result.Errors);
        }
    }
}

using IdentityServerPOC.Dtos;
using IdentityServerPOC.Infrastructure;
using IdentityServerPOC.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
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
            IEnumerable<AppUser> users = await _userManager.Users.ToListAsync();
            return users.Select(user => new UserDto(user, null)).ToList();
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

            if(!string.IsNullOrEmpty(model.Role) && !await _roleManager.RoleExistsAsync(model.Role))
            {
                return BadRequest();
            }
            
            var user = new AppUser { UserName = model.Email, Name = model.Name, Email = model.Email };

            var result = await _userManager.CreateAsync(user, model.Password);
            

            if (!result.Succeeded) return BadRequest(result.Errors);

            await _userManager.AddClaimAsync(user, new System.Security.Claims.Claim("userName", user.UserName));
            await _userManager.AddClaimAsync(user, new System.Security.Claims.Claim("name", user.Name));
            await _userManager.AddClaimAsync(user, new System.Security.Claims.Claim("email", user.Email));
            await _userManager.AddClaimAsync(user, new System.Security.Claims.Claim("role", model.Role));
            await _userManager.AddToRoleAsync(user, model.Role);

            return Ok(new RegisterResponseViewModel(user));
        }

    }
}

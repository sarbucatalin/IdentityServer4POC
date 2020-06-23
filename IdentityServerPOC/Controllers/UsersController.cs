﻿using IdentityServerPOC.Dtos;
using IdentityServerPOC.Infrastructure;
using IdentityServerPOC.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Internal;
using System;
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
            var returnUsers = new List<UserViewModel>();
            IEnumerable<ApplicationUser> appUsers = await _userManager.Users.ToListAsync();
            foreach (var user in appUsers)
            {
                var role = (await _userManager.GetRolesAsync(user)).FirstOrDefault();
                var roleId = _roleManager.Roles.FirstOrDefault(ir => ir.Name == role)?.Id;
                returnUsers.Add(new UserViewModel(user));
            }
            return returnUsers;
        }

        [HttpGet("{userId}")]
        public async Task<UserViewModel> GetUserByAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            var role = (await _userManager.GetRolesAsync(user)).FirstOrDefault();
            var roleId = _roleManager.Roles.FirstOrDefault(ir => ir.Name == role)?.Id;

            return new UserViewModel(user);
        }

        [HttpPost]
        public async Task<IActionResult> Register([FromBody] RegisterRequestViewModel model)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            //if (!string.IsNullOrEmpty(model.Role) && !await _roleManager.RoleExistsAsync(model.Role))
            //{
            //    return BadRequest();
            //}

            var user = new ApplicationUser { UserName = model.Email, Name = model.Name, Email = model.Email };

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

            var result = await _userManager.AddToRoleAsync(user, updateUserRole.Role);
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
        [Route("{userId}/Unlock")]
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

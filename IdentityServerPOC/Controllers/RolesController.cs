using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace IdentityServerPOC.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RolesController : ControllerBase
    {
        private readonly RoleManager<IdentityRole> _roleManager;

        public RolesController(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] string name)
        {
            if (string.IsNullOrEmpty(name) || (!string.IsNullOrEmpty(name) && await _roleManager.RoleExistsAsync(name)))
            {
                return BadRequest();
            }
            return Ok(await _roleManager.CreateAsync(new IdentityRole(name)));
        }

        [HttpGet]
        public async Task<IActionResult> GetRoles()
        {
            return Ok(await _roleManager.Roles.ToListAsync());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetRole(string id)
        {
            return Ok(await _roleManager.FindByIdAsync(id));
        }
    }
}

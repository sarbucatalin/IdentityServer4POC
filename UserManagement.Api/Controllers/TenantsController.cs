using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using UserManagement.Api.Dtos;
using UserManagement.Api.Services;

namespace UserManagement.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "SuperAdmin")]
    public class TenantsController : ControllerBase
    {
        private readonly ITenantService _tenantService;

        public TenantsController(ITenantService tenantService)
        {
            _tenantService = tenantService;
        }

        [HttpGet]
        public async Task<ActionResult<List<TenantDto>>> GetAll()
        {
            try
            {
                var tenants = await _tenantService.GetAll();
                return Ok(tenants);
            }
            catch (Exception)
            {
                Response.StatusCode = 502;
                return NoContent();

            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TenantDto>> Get(string id)
        {
            try
            {
                var tenant = await _tenantService.FindById(id);
                if (tenant == null)
                {
                    return NotFound();
                }

                return Ok(tenant);
            }
            catch (Exception)
            {
                Response.StatusCode = 502;
                return NoContent();

            }
        }

        [HttpPut]
        public async Task<IActionResult> Edit(TenantDto tenantDto)
        {
            try
            {
                await _tenantService.Update(tenantDto);

                return NoContent();
            }
            catch (Exception)
            {
                Response.StatusCode = 502;
                return NoContent();

            }
        }

        [HttpPost]
        public async Task<ActionResult<TenantDto>> Create(TenantDto tenantDto)
        {
            try
            {
                tenantDto.Id = Guid.NewGuid().ToString();
                await _tenantService.Create(tenantDto);

                return Ok(tenantDto);
            }
            catch (Exception)
            {
                Response.StatusCode = 502;
                return NoContent();

            }

        }
    }
}

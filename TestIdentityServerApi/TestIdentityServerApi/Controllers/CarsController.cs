﻿using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TestIdentityServerApi.Controllers
{
    [Authorize(Policy = "ApiReader")]
    [Route("api/[controller]")]
    [ApiController]
    public class CarsController : ControllerBase
    {
        [Authorize]
        [HttpGet]
        public IActionResult GetCars()
        {
            var cars = new List<string> { "Lamborgini", "Ferari", "Audi A4", "Passat CC", "Matiz" };
            return Ok(cars);
        }
    }
}

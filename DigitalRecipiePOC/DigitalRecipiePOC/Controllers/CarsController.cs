using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DigitalRecipiePOC.Controllers
{
    [Authorize(Policy = "ApiReader")]
    [Route("api/[controller]")]
    [ApiController]
    public class CarsController : ControllerBase
    {
        [Authorize(Policy = "SuperAdmin")]
        [HttpGet]
        public IActionResult GetCars()
        {
            var cars = new List<string> { "Lamborgini", "Ferari", "Audi A4", "Passat CC" };
            return Ok(cars);
        }
    }
}

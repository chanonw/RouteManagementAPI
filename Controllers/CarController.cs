using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RouteAPI.Data;

namespace RouteAPI.Controllers
{
    [Route("api/[controller]")]
    public class CarController : Controller
    {
        private readonly DataContext _context;
        public CarController(DataContext context)
        {
            _context = context;

        }
        [HttpPost("getcar")]
        public async Task<IActionResult> getAveilableCar([FromBody] Dto dto) {
            var car = await _context.Car.Where(z => z.zoneId == dto.zoneId).ToListAsync();
            return Ok(car);
        }
    }
}
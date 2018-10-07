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
        [HttpGet]
        public async Task<IActionResult> GetCars()
        {
            var values = await _context.Car.ToListAsync();
            return Ok(values);
        }
        [HttpGet("getavailablecar")]
        public async Task<IActionResult> GetAvailableCar()
        {
            var availableCar = await _context.Car.Where(s => s.status == "available").ToListAsync();
            return Ok(availableCar);
        }
        
    }
}
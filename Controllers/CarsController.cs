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
    public class CarsController : Controller
    {
        private readonly DataContext _context;
        public CarsController(DataContext context)
        {
            _context = context;

        }
        [HttpGet]
        public async Task<IActionResult> GetCars()
        {
            var values = await _context.Cars.ToListAsync();
            return Ok(values);
        }
        
    }
}
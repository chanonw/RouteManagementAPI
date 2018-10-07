using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RouteAPI.Data;

namespace RouteAPI.Controllers
{
    [Route("api/[controller]")]
    public class WarehouseController : Controller
    {
        private readonly DataContext _context;
        public WarehouseController(DataContext context)
        {
            _context = context;

        }
        [HttpGet]
        public async Task<IActionResult> GetWarehouses() 
        {
            var warehouse = await _context.Warehouse.Include(z => z.zones).ToListAsync();
            return Ok(warehouse);
        }
    }
}
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RouteAPI.Data;
using System.Linq;

namespace RouteAPI.Controllers
{
    [Route("api/[controller]")]
    public class ZoneController : Microsoft.AspNetCore.Mvc.Controller
    {
        private readonly DataContext _context;
        public ZoneController(DataContext context)
        {
           _context = context;

        }
        [HttpPost("getzone")]
        public async Task<IActionResult> getZone([FromBody]Dto dto)
        {
            var zone = await _context.Zone.Where(w => w.warehouseId == dto.warehouseId).ToListAsync();
            return Ok(zone);
        }
    }
}
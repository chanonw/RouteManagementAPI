using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RouteAPI.Data;
using RouteAPI.Dtos;
using RouteAPI.Models;

namespace RouteAPI.Controllers
{

    [Route("api/[controller]")]
    public class WarehouseController : Microsoft.AspNetCore.Mvc.Controller
    {
        private readonly DataContext _context;
        private readonly IRouteRepository _repo;
        public WarehouseController(DataContext context, IRouteRepository repo)
        {
            _repo = repo;
            _context = context;
        }
        [HttpPost("newwarehouse")]
        public async Task<IActionResult> AddNewWarehouse([FromBody]WarehouseForNewDto warehouseForNewDto)
        {
            var warehouseForCreate = new Warehouse
            {
                warehouseName = warehouseForNewDto.warehouseName,
                gps = warehouseForNewDto.gps
            };
            var createWarehouse = await _repo.addNewWarehouse(warehouseForCreate);
            return StatusCode(201, new { success = true });
        }
        [HttpGet]
        public async Task<IActionResult> getWarehouse()
        {
            var warehouse = await _repo.getWarehouse();
            return Ok(warehouse);
        }
    }
}
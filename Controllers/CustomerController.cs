using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RouteAPI.Data;
using RouteAPI.Dtos;
using RouteAPI.Models;

namespace RouteAPI.Controllers
{
    [Route("api/[controller]")]
    public class CustomerController : Controller
    {
        private readonly DataContext _context;
        private readonly IRouteRepository _repo;
        public CustomerController(DataContext context, IRouteRepository repo)
        {
            _repo = repo;
            _context = context;

        }
        [HttpPost("newcustomer")]
        public async Task<IActionResult> AddNewCustomer([FromBody] CustomerForNewDto customerForNewDto)
        {
            string cusCode = string.Empty;
            string latestCarCode = await _repo.GetLatestCusCode();
            int tmp = int.Parse(latestCarCode);
            cusCode = (tmp + 1).ToString("D4");
            var customerForCreate = new Customer
            {
                cusCode = cusCode,
                title = customerForNewDto.title,
                firstName = customerForNewDto.firstName,
                lastName = customerForNewDto.lastName,
                houseNo = customerForNewDto.houseNo,
                building = customerForNewDto.building,
                road = customerForNewDto.road,
                soi = customerForNewDto.soi,
                subDistrict = customerForNewDto.subDistrict,
                district = customerForNewDto.district,
                city = customerForNewDto.city,
                postalCode = customerForNewDto.postalCode,
                depBottle = int.Parse(customerForNewDto.depBottle),
                cusCond = "รอบปกติ",
                cusType = customerForNewDto.cusType,
                day = customerForNewDto.day,
                dayInWeek = customerForNewDto.dayInWeek,
                status = "new"
            };
            var createCustomer = await _repo.addNewCustomer(customerForCreate);
            return StatusCode(201, new { success = true });
        }
        [HttpGet("getnewcustomer")]
        public async Task<IActionResult> getNewCustomer()
        {
            var customer = await _repo.getNewCustomer();
            return Ok(customer);
        }
        [HttpPost("updatezone")]
        public async Task<IActionResult> updateZone([FromBody] CustomerForUpdateDto customerForUpdateDto)
        {
            var customer = await _repo.updateZone(customerForUpdateDto.cusCode, customerForUpdateDto.zoneId, 
                customerForUpdateDto.gps, customerForUpdateDto.cusCond, customerForUpdateDto.cusType, customerForUpdateDto.day, 
                customerForUpdateDto.distanceToWh);
            if (customer == null)
            {
                return NotFound(new { success = false });
            }
            return Ok(new { success = true });
        }
        [HttpPost("getCustomerPerDay")]
        public async Task<IActionResult> getCustomerPerDay([FromBody]Dto dto)
        {
            var customer = await _repo.getCustomerPerDay(dto.dayName);
            return Ok(customer);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RouteAPI.Data;
using RouteAPI.Dtos;
using RouteAPI.Models;

namespace RouteAPI.Controllers
{
    [Route("api/[controller]")]
    public class TruckController : Microsoft.AspNetCore.Mvc.Controller
    {
        private readonly DataContext _context;
        private readonly IRouteRepository _repo;
        public TruckController(DataContext context, IRouteRepository repo)
        {
            _repo = repo;
            _context = context;

        }
        [HttpPost("gettruck")]
        public async Task<IActionResult> getAveilableTruck([FromBody] Dto dto)
        {
            var trucks = await _context.Truck.Where(z => z.zoneId == dto.zoneId).ToListAsync();
            return Ok(trucks);
        }
        [HttpPost("getadditionaltruck")]
        public async Task<IActionResult> getAdditionaltruck([FromBody] Dto dto)
        {
            var additionalTruck = await _repo.getAdditionalTruck(dto.zoneId, 1);
            return Ok(additionalTruck);
        }
        [HttpPost("newtruck")]
        public async Task<IActionResult> AddNewTruck([FromBody] DriverForNewDto driverForNewDto)
        {
            string truckCode = string.Empty;
            string latestTruckCode = await _repo.getLatestCarCode(driverForNewDto.zoneId);
            if(string.IsNullOrEmpty(latestTruckCode))
            {
                truckCode = "truck";
            }
            else
            {
                int temp = int.Parse(latestTruckCode.Substring(3));
                truckCode = "truck" + (int.Parse(latestTruckCode.Substring(3)) + 1).ToString();
            }
            var driverToCreate = new Truck
            {
                truckCode = truckCode,
                firstName = driverForNewDto.firstName,
                lastName = driverForNewDto.lastName,
                drivingLicenseNo = driverForNewDto.drivingLicenseNo,
                carLicenseNo = driverForNewDto.carLicenseNo,
                zoneId = driverForNewDto.zoneId,
                status = "available"
            };
            var createDriver = await _repo.addNewCar(driverToCreate);
            
            return StatusCode(201, new { success = true });
        }
        [HttpPost("searchtruck")]
        public async Task<IActionResult> SearchCar([FromBody] CarForSearchDto carForSearchDto)
        {
            var car = await _repo.searchCar(carForSearchDto.truckCode);
            return Ok(car);
        }
        [HttpPost("updatepersonalleave")]
        public async Task<IActionResult> updatePersonalLeave([FromBody] Dto dto)
        {
            var car = await _repo.updatePersonalLeaveStatus(dto.truckCode);
            return Ok(car);
        }
        [HttpPost("updatesickleave")]
        public async Task<IActionResult> updateSickLeave([FromBody] Dto dto)
        {
            var car = await _repo.updateSickLeaveStatus(dto.truckCode);
            return Ok(car);
        }
    }
}
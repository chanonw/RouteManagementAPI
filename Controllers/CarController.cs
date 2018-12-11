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
    public class CarController : Controller
    {
        private readonly DataContext _context;
        private readonly IRouteRepository _repo;
        public CarController(DataContext context, IRouteRepository repo)
        {
            _repo = repo;
            _context = context;

        }
        [HttpPost("getcar")]
        public async Task<IActionResult> getAveilableCar([FromBody] Dto dto)
        {
            var car = await _context.Car.Where(z => z.zoneId == dto.zoneId).ToListAsync();
            return Ok(car);
        }

        [HttpPost("newcar")]
        public async Task<IActionResult> AddNewCar([FromBody] DriverForNewDto driverForNewDto)
        {
            string carCode = string.Empty;
            string latestCarCode = await _repo.getLatestCarCode(driverForNewDto.zoneId);
            if(string.IsNullOrEmpty(latestCarCode))
            {
                carCode = "car";
            }
            else
            {
                int temp = int.Parse(latestCarCode.Substring(3));
                carCode = "car" + (int.Parse(latestCarCode.Substring(3)) + 1).ToString();
            }
            var driverToCreate = new Car
            {
                carCode = carCode,
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
        [HttpPost("searchcar")]
        public async Task<IActionResult> SearchCar([FromBody] CarForSearchDto carForSearchDto)
        {
            var car = await _repo.searchCar(carForSearchDto.carCode);
            return Ok(car);
        }
        [HttpPost("updatepersonalleave")]
        public async Task<IActionResult> updatePersonalLeave([FromBody] Dto dto)
        {
            var car = await _repo.updatePersonalLeaveStatus(dto.carCode);
            return Ok(car);
        }
        [HttpPost("updatesickleave")]
        public async Task<IActionResult> updateSickLeave([FromBody] Dto dto)
        {
            var car = await _repo.updateSickLeaveStatus(dto.carCode);
            return Ok(car);
        }
    }
}
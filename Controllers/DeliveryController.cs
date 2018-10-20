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
    public class DeliveryController : Controller
    {
        private readonly IRouteRepository _repo;
        public DeliveryController(IRouteRepository repo)
        {
            _repo = repo;
        }
        [HttpPost("unassign")]
        public async Task<IActionResult> getUnassignDelivery([FromBody] Dto dto)
        {
            var delivery = await _repo.getUnassignDelivery(dto.transDate);
            return Ok(delivery);
        }
        [HttpPost("waiting")]
        public async Task<IActionResult> getWaitToSendDelivery([FromBody] Dto dto)
        {
            var delivery = await _repo.getWaitToSendDelivery(dto.transDate, dto.carCode);
            return Ok(delivery);
        }

        [HttpPost("savejob")]
        public async Task<IActionResult> saveJob([FromBody] Dto dto)
        {
            List<string> failedId = new List<string>();
            List<string> successId = new List<string>();
            foreach (var deliveryId in dto.deliveryId)
            {
                var delivery = await _repo.getDelivery(deliveryId);
                delivery.carCode = dto.carCode;
                delivery.status = "รอส่ง";
                if (await _repo.saveAll())
                {
                    successId.Add(deliveryId);
                }
                else
                {
                    failedId.Add(deliveryId);
                }
            }
            if(failedId.Count == dto.deliveryId.Length)
            {
                foreach(var deliveryId in failedId)
                {
                    resetUnassignDelivery(deliveryId);
                }
                return Ok(new {success = false});
            }
            if(failedId.Count > 0 && successId.Count > 0) 
            {
                foreach(var deliveryId in failedId)
                {
                    resetUnassignDelivery(deliveryId);
                }
                return Ok(new {success = false, partialSuccess = true, failedList= failedId, successList = successId});
            }
            return Ok(new {success = "true"});
        }

        [HttpPost("saveroute")]
        public async Task<IActionResult> saveRoute([FromBody] Dto dto)
        {
            List<string> failedId = new List<string>();
            List<string> successId = new List<string>();
            for (int i = 0; i < dto.deliveryId.Length; i++)
            {
                var deliveryId = dto.deliveryId[i];
                var delivery = await _repo.getDelivery(deliveryId);
                var deliveryOrder = i + 1;
                delivery.status = "พร้อมส่ง";
                delivery.deliveryOrder = deliveryOrder.ToString();
                 if (await _repo.saveAll())
                {
                    successId.Add(deliveryId);
                }
                else
                {
                    failedId.Add(deliveryId);
                }
            }
             if(failedId.Count == dto.deliveryId.Length)
            {
                foreach(var deliveryId in failedId)
                {
                    resetWaitingDelivery(deliveryId);
                }
                return Ok(new {success = false});
            }
            if(failedId.Count > 0 && successId.Count > 0) 
            {
                foreach(var deliveryId in failedId)
                {
                   resetWaitingDelivery(deliveryId);
                }
                return Ok(new {success = false, partialSuccess = true, failedList= failedId, successList = successId});
            }
            return Ok(new {success = "true"});
        }
        private async void resetUnassignDelivery(string deliveryId)
        {
            var delivery = await _repo.getDelivery(deliveryId);
            delivery.status = "unassign";
            delivery.carCode = null;
            await _repo.saveAll();
        }
        private async void resetWaitingDelivery(string deliveryId)
        {
            var delivery = await _repo.getDelivery(deliveryId);
            delivery.status = "รอส่ง";
            await _repo.saveAll();
        }
    }
}
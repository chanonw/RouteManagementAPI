using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RouteAPI.Data;
using RouteAPI.Models;
using RouteAPI.Helpers;
namespace RouteAPI.Controllers
{
    [Route("api/[controller]")]
    public class DeliveryController : Controller
    {
        private readonly IRouteRepository _repo;
        public List<Route> finalRoute = new List<Route>();
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
            if (failedId.Count == dto.deliveryId.Length)
            {
                foreach (var deliveryId in failedId)
                {
                    resetUnassignDelivery(deliveryId);
                }
                return Ok(new { success = false });
            }
            if (failedId.Count > 0 && successId.Count > 0)
            {
                foreach (var deliveryId in failedId)
                {
                    resetUnassignDelivery(deliveryId);
                }
                return Ok(new { success = false, partialSuccess = true, failedList = failedId, successList = successId });
            }
            return Ok(new { success = "true" });
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
            if (failedId.Count == dto.deliveryId.Length)
            {
                foreach (var deliveryId in failedId)
                {
                    resetWaitingDelivery(deliveryId);
                }
                return Ok(new { success = false });
            }
            if (failedId.Count > 0 && successId.Count > 0)
            {
                foreach (var deliveryId in failedId)
                {
                    resetWaitingDelivery(deliveryId);
                }
                return Ok(new { success = false, partialSuccess = true, failedList = failedId, successList = successId });
            }
            return Ok(new { success = "true" });
        }
        [HttpPost("auto")]
        public async Task<IActionResult> autoJobRoute([FromBody]Dto dto)
        {
            List<string> firstTripId = new List<string>();
            List<string> secondTripId = new List<string>();
            //var delivery = await _repo.getDescUnassignDelivery(dto.transDate);
            //int totalQuantity = calculateTotalQuantity(delivery);
            //var firstRoundQuantityCri = 70;//Math.Floor(0.45 * totalQuantity);
            //var secondRoundQuantity = 30;//Math.Floor(0.55 * totalQuantity)
            //int cumQuan = 0;
            int cu = 0;
            // foreach (var item in delivery)
            // {
            //     cumQuan = cumQuan + item.quantity;
            //     if (cumQuan <= firstRoundQuantityCri)
            //     {
            //         firstTripId.Add(item.deliveryId);
            //     }
            //     else
            //     {
            //         secondTripId.Add(item.deliveryId);
            //     }
            // }
            // foreach (var deliveryId in firstTripId)
            // {
            //     await updateDeliveryFirstTrip(deliveryId);
            // }
            // foreach (var deliveryId in secondTripId)
            // {
            //     await updateDeliverySecondTrip(deliveryId);
            // }
            var firstTripDelivery = await _repo.getFirstTripDelivery(dto.transDate);
            var secondTripDelivery = await _repo.getSecondTripDelivery(dto.transDate);
            var accualFirstTripQuantity = calculateTotalQuantity(firstTripDelivery);
            var accualSecondTripQuantity = calculateTotalQuantity(secondTripDelivery);
            var cars = await _repo.getCar(dto.zoneId);
            var aveilableCar = cars.Count();
            var firstTripCarQuantity = accualFirstTripQuantity / aveilableCar;
            var secondTripCarQuantity = accualSecondTripQuantity / aveilableCar;
            if (firstTripCarQuantity > 40)
            {
                if (firstTripCarQuantity < 80)
                {
                    foreach (var item in firstTripDelivery)
                    {
                        foreach (var car in cars)
                        {
                            cu = cu + item.quantity;
                            if (cu <= firstTripCarQuantity)
                            {
                                if (item.carCode == null)
                                {
                                    await updateResult(car.carCode, item.deliveryId);
                                }

                            }
                            else
                            {
                                cu = 0;
                            }
                        }
                    }
                }
                else
                {
                    return Ok(new { success = false, message = "จำนวนรถรอบเช้าไม่เพียงพอ" });
                }
            }
            else
            {
                return Ok(new { success = true, manual = true });
            }
            cars = await _repo.getCarDesc(dto.zoneId);
            if (secondTripCarQuantity > 40)
            {
                if (secondTripCarQuantity < 80)
                {
                    foreach (var item in secondTripDelivery)
                    {
                        foreach (var car in cars)
                        {
                            cu = cu + item.quantity;
                            if (cu <= firstTripCarQuantity)
                            {
                                if (item.carCode == null)
                                {
                                    await updateResult(car.carCode, item.deliveryId);
                                }

                            }
                            else
                            {
                                cu = 0;
                            }
                        }
                    }
                }
                else
                {
                    return Ok(new { success = false, message = "จำนวนรถรอบบ่ายไม่เพียงพอ" });
                }
            }
            else
            {
                return Ok(new { success = true, manual = true });
            }
            return Ok(new { success = true });
        }
        private async void CalSaving(List<Delivery> deliveries, Dto dto)
        {
            double savingValue = 0;
            List<SavingNode> saving = new List<SavingNode>();
            string carCode = string.Empty;
            string tmpWarehouseGPS = string.Empty;
            string[] warehouseGPS = tmpWarehouseGPS.Split(",");
            var cars = await _repo.getCar(dto.zoneId);
            for (int i = 0; i < deliveries.Count(); i++)
            {
                for (int j = 0; j < deliveries.Count(); j++)
                {
                    if (i != j)
                    {
                        string tmpIGPS = deliveries[i].Customer.gps;
                        string[] iGPS = tmpIGPS.Split(",");
                        string tmpJGPS = deliveries[j].Customer.gps;
                        string[] jGPS = tmpJGPS.Split(",");
                        double CDi = DistanceMetrix.getDistanceMetrixInKM(Double.Parse(warehouseGPS[0]),
                            Double.Parse(warehouseGPS[1]), Double.Parse(iGPS[0]), Double.Parse(iGPS[1]));
                        double CDj = DistanceMetrix.getDistanceMetrixInKM(Double.Parse(warehouseGPS[0]),
                            Double.Parse(warehouseGPS[1]), Double.Parse(jGPS[0]), Double.Parse(jGPS[1]));
                        double Cij = DistanceMetrix.getDistanceMetrixInKM(Double.Parse(iGPS[0]),
                            Double.Parse(iGPS[1]), Double.Parse(jGPS[0]), Double.Parse(jGPS[1]));
                        //sij = CDi + CDj - Cij
                        savingValue = CDi + CDj - Cij;
                        CustomerOrder c1 = new CustomerOrder(deliveries[i].quantity, deliveries[i].Customer.cusCode);
                        CustomerOrder c2 = new CustomerOrder(deliveries[j].quantity, deliveries[j].Customer.cusCode);
                        saving.Add(new SavingNode(c1, c2, savingValue));
                    }
                }
            }
            List<SavingNode> sortSaving = saving.OrderByDescending(s => s.savings).ToList();
            foreach (var item in sortSaving)
            {
                if (!IsInRoutes(item.getFrom()) && !IsInRoutes(item.getTo()))
                {
                    Route route = new Route();
                    route.Add(item.getFrom());
                    route.Add(item.getTo());
                    if (!finalRoute.Contains(route))
                    {
                        finalRoute.Add(route);
                    }
                }
                else if (!IsInRoutes(item.getTo()))
                {
                    foreach (var r in finalRoute)
                    {
                        if (r.getLastCustomer() == item.getFrom())
                        {
                            if (r.hasCapacity(item.getTo().quantity, 80))
                            {
                                r.Add(0, item.getTo());
                                break;
                            }
                        }
                    }
                }
                else if (!IsInRoutes(item.getFrom()))
                {
                    foreach (var r in finalRoute)
                    {
                        if (r.getLastCustomer() == item.getTo())
                        {
                            if (r.hasCapacity(item.getFrom().quantity, 80))
                            {
                                r.Add(0, item.getFrom());
                                break;
                            }
                        }
                    }
                }
                Route merged = null;
                foreach (var routeX in finalRoute)
                {
                    if (merged != null)
                    {
                        break;
                    }
                    if (routeX.getLastCustomer() == item.getFrom())
                    {
                        foreach (var routeY in finalRoute)
                        {
                            if (routeY.getFirstCustomer() == item.getTo())
                            {
                                if (routeX != routeY)
                                {
                                    if ((routeX.getCurrentCapacity() + routeY.getCurrentCapacity()) <= 80)
                                    {
                                        routeX.addAll(routeY);
                                        merged = routeY;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
                if (merged != null)
                {
                    finalRoute.Remove(merged);
                }
            }
            //update db;
        }
        private bool IsInRoutes(CustomerOrder customer)
        {
            foreach (var item in finalRoute)
            {
                foreach (var cus in item.GetList())
                {
                    if (customer == cus)
                    {
                        return true;
                    }
                }

            }
            return false;
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
        private int calculateTotalQuantity(IEnumerable<Delivery> deliveries)
        {
            //var delivery = await _repo.getDescUnassignDelivery(transDate);
            var quantity = 0;
            foreach (var item in deliveries)
            {
                quantity = quantity + item.quantity;
            }
            return quantity;
        }
        private async Task<bool> updateDeliveryFirstTrip(string deliveryId)
        {
            var delivery = await _repo.getDelivery(deliveryId);
            delivery.trip = "1";
            if (await _repo.saveAll())
            {
                return true;
            }
            return false;
        }
        private async Task<bool> updateDeliverySecondTrip(string deliveryId)
        {
            var delivery = await _repo.getDelivery(deliveryId);
            delivery.trip = "2";
            if (await _repo.saveAll())
            {
                return true;
            }
            return false;
        }
        private async Task<bool> updateResult(string carCode, string deliveryId)
        {
            var delivery = await _repo.getDelivery(deliveryId);
            delivery.carCode = carCode;
            delivery.status = "รอส่ง";
            if (await _repo.saveAll())
            {
                return true;
            }
            return false;
        }
    }
}
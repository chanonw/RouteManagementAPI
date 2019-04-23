using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RouteAPI.Data;
using RouteAPI.Models;
using RouteAPI.Helpers;
using RouteAPI.Dtos;
using GoogleApi.Entities.Common;
using GoogleApi.Entities.Common.Enums;
using GoogleApi.Entities.Maps.Common.Enums;
using GoogleApi.Entities.Maps.DistanceMatrix.Request;
using GoogleApi.Entities.Maps.Directions.Request;
using GoogleApi.Exceptions;
using Microsoft.Extensions.Configuration;
using DinkToPdf.Contracts;
using DinkToPdf;
using System.IO;

namespace RouteAPI.Controllers
{
    [Route("api/[controller]")]
    public class DeliveryController : Controller
    {
        private readonly IRouteRepository _repo;
        private readonly IConfiguration _config;
        private readonly IConverter _converter;
        private List<OrderList> firstTripList = new List<OrderList>();
        private List<OrderList> secondTripList = new List<OrderList>();
        public DeliveryController(IRouteRepository repo, IConfiguration config, IConverter converter)
        {
            _repo = repo;
            _config = config;
            _converter = converter;
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
            var delivery = await _repo.getWaitToSendDelivery(dto.transDate, dto.truckCode);
            return Ok(delivery);
        }
        [HttpPost("getcardelivery")]
        public async Task<IActionResult> getTruckDelivery([FromBody] Dto dto)
        {
            var delivery = await _repo.getCarDelivery(dto.truckCode, "พร้อมส่ง", dto.transDate);
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
                delivery.truckCode = dto.truckCode;
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
                //delivery.deliveryOrder = deliveryOrder.ToString();
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
        public async Task<IActionResult> auto([FromBody] Dto dto)
        {
            int totalQuantity = 0;
            IEnumerable<Delivery> delivery = null;
            if (await _repo.hasPendingOrder())
            {
                delivery = await _repo.getUnassignPendingDelivery(dto.transDate);
                totalQuantity = calculateTotalQuantity(delivery);
            }
            else
            {
                delivery = await _repo.getDescUnassignDelivery(dto.transDate);
                totalQuantity = calculateTotalQuantity(delivery);
            }
            var trucks = await _repo.getTrucks(dto.zoneId);
            var randomTrucks = trucks.Randomize();
            var availableTrucks = randomTrucks.Count();
            int maximumOrder = availableTrucks * 2 * 80;
            double truckQuantity = totalQuantity / (availableTrucks * 2);
            var quantity = Math.Round(truckQuantity);
            if (totalQuantity > maximumOrder)
            {
                if (await firstTrip(randomTrucks, dto.transDate, quantity))
                {
                    if (await secondTrip(randomTrucks, dto.transDate, quantity))
                    {
                        if (await merge(firstTripList, secondTripList, randomTrucks))
                        {
                            var date = DateTime.Parse(dto.transDate);
                            //update used date
                            var usedDate = new UsedDate
                            {
                                transDate = date
                            };
                            await _repo.insertUsedDate(usedDate);
                            return Ok(new { success = true });
                            // if (await createPDF(randomTrucks))
                            // {
                            //     return Ok(new { success = true });
                            // }
                        }
                    }
                }
                //firstTrip(randomCars, dto.transDate, quantity);
                //secondTrip(randomCars, dto.transDate, quantity);
                //merge(firstTripList, secondTripList, randomTrucks);
            }
            else
            {
                if (quantity < 40)
                {
                    Dto a = new Dto();
                    a.deliveries = delivery;
                    a.trucks = trucks;
                    return Ok(new { success = true, manual = true, dto = a });
                }
                else
                {
                    if (await firstTrip(randomTrucks, dto.transDate, quantity))
                    {
                        if (await secondTrip(randomTrucks, dto.transDate, quantity))
                        {
                            // return Ok(new { success = true });
                            if (await merge(firstTripList, secondTripList, randomTrucks))
                            {
                                //update used date
                                var date = DateTime.Parse(dto.transDate);
                                //update used date
                                var usedDate = new UsedDate
                                {
                                    transDate = date
                                };
                                await _repo.insertUsedDate(usedDate);
                                return Ok(new { success = true });
                                // if (await createPDF(randomTrucks))
                                // {
                                //     return Ok(new { success = true });
                                // }
                            }
                        }
                    }
                    //firstTrip(randomCars, dto.transDate, quantity);
                    //secondTrip(randomCars, dto.transDate, quantity);
                    //merge(firstTripList, secondTripList, randomTrucks);
                }
            }
            return Ok(new { success = false });
        }

        [HttpPost("manual")]
        public async Task<IActionResult> manual([FromBody] ManualDto ManualDto)
        {
            int totalQuantity = 0;
            if (ManualDto.unUsedTruck != null)
            {
                //update idletime
                foreach (var unUsedTruck in ManualDto.unUsedTruck)
                {
                    await updateIdleTime(unUsedTruck.truckCode);
                }
            }
            var deliveries = ManualDto.deliveries;
            totalQuantity = calculateTotalQuantity(deliveries);
            var trucks = ManualDto.selectedTruck;
            var randomTrucks = trucks.Randomize();
            var availableTrucks = randomTrucks.Count();
            double truckQuantity = totalQuantity / (availableTrucks * 2);
            var quantity = Math.Round(truckQuantity);
            if (await firstTrip(randomTrucks, ManualDto.transDate, quantity))
            {
                if (await secondTrip(randomTrucks, ManualDto.transDate, quantity))
                {
                    // return Ok(new { success = true });
                    if (await merge(firstTripList, secondTripList, randomTrucks))
                    {
                        //update used date
                        var date = DateTime.Parse(ManualDto.transDate);
                        //update used date
                        var usedDate = new UsedDate
                        {
                            transDate = date
                        };
                        await _repo.insertUsedDate(usedDate);
                        return Ok(new { success = true });
                        // if (await createPDF(randomTrucks))
                        // {
                        //     return Ok(new { success = true });
                        // }
                    }
                }
            }
            return Ok(new { sucess = false });
        }
        [HttpPost("getreport")]
        public async Task<IActionResult> getReportData([FromBody] Dto dto)
        {
            var truck = await _repo.searchCar(dto.truckCode);
            var firstTripData = await _repo.getCarDeliveryPerTrip(dto.truckCode, "พร้อมส่ง", "1");
            var secondTripData = await _repo.getCarDeliveryPerTrip(dto.truckCode, "พร้อมส่ง", "2");
            return Ok(new { truck = truck, firstTrip = firstTripData, secondTrip = secondTripData });
        }
        public async Task<bool> createPDF(IEnumerable<Truck> trucks)
        {
            bool status = false;
            foreach (var truck in trucks)
            {
                var data = await _repo.getCarDeliveryPerTrip(truck.truckCode, "พร้อมส่ง", "1");
                List<Delivery> reportData = data.ToList();
                var globalSettings = new GlobalSettings
                {
                    ColorMode = ColorMode.Color,
                    Orientation = Orientation.Portrait,
                    PaperSize = PaperKind.A4,
                    Margins = new MarginSettings { Top = 10 },
                    DocumentTitle = "PDF Report",
                    Out = @"D:\PDFCreator\Test.pdf"
                };
                var objectSettings = new ObjectSettings
                {
                    PagesCount = true,
                    HtmlContent = TemplateGenerator.GetHTMLString(reportData),//TemplateGenerator.GetHTMLString(),
                    WebSettings = { DefaultEncoding = "utf-8", UserStyleSheet = Path.Combine(Directory.GetCurrentDirectory(), "assets", "styles.css") },
                    HeaderSettings = { FontName = "Arial", FontSize = 9, Right = "Page [page] of [toPage]", Line = true },
                    FooterSettings = { FontName = "Arial", FontSize = 9, Line = true, Center = "Report Footer" }
                };
                var pdf = new HtmlToPdfDocument()
                {
                    GlobalSettings = globalSettings,
                    Objects = { objectSettings }
                };
                _converter.Convert(pdf);
            }
            status = true;
            return status;
        }
        // [HttpGet("test")]
        // public async Task<IActionResult> test()
        // {
        //     var data = await _repo.getCarDeliveryPerTrip("car11", "พร้อมส่ง", "1");
        //     List<Delivery> reportData = data.ToList();
        //     var globalSettings = new GlobalSettings
        //     {
        //         ColorMode = ColorMode.Color,
        //         Orientation = Orientation.Portrait,
        //         PaperSize = PaperKind.A4,
        //         Margins = new MarginSettings { Top = 10 },
        //         DocumentTitle = "PDF Report",
        //         Out = @"D:\PDFCreator\Test.pdf"
        //     };
        //     var objectSettings = new ObjectSettings
        //     {
        //         PagesCount = true,
        //         HtmlContent = TemplateGenerator.GetHTMLString(reportData),//TemplateGenerator.GetHTMLString(),
        //         WebSettings = { DefaultEncoding = "utf-8", UserStyleSheet = Path.Combine(Directory.GetCurrentDirectory(), "assets", "styles.css") },
        //         //WebSettings = { DefaultEncoding = "utf-8", UserStyleSheet = @"D:\RouteManage\RouteAPI\assets\style.css" },
        //         HeaderSettings = { FontName = "Arial", FontSize = 9, Right = "Page [page] of [toPage]", Line = true },
        //         FooterSettings = { FontName = "Arial", FontSize = 9, Line = true, Center = "Report Footer" }
        //     };
        //     var pdf = new HtmlToPdfDocument()
        //     {
        //         GlobalSettings = globalSettings,
        //         Objects = { objectSettings }
        //     };
        //     _converter.Convert(pdf);
        //     return Ok();
        //     // List<Order> OrderList = new List<Order>();
        //     // double totalDistance = 0;
        //     // int totalSeconds = 0;
        //     // OrderList.Add(new Order("f9452f82-e0a1-4bf5-b330-2ecc2383021f", "13.7300004,100.5451364"));
        //     // OrderList.Add(new Order("ef5bbe07-8bb2-4f22-b96d-15b0e24eba06",
        //     //     "13.735907931722435,100.54426608354522"));
        //     // OrderList.Add(new Order("6ed7e298-8840-4ec5-994e-cc8a6b06b67b", "13.7285412,100.5476752"));
        //     // OrderList.Add(new Order("c4b05cce-7d31-4aae-9a7d-fa46d5efa469", "13.7244858,100.5443305"));
        //     // OrderList.Add(new Order("0dc91725-2426-46d1-9399-4a64de16fc99", "13.7359672,100.5508057"));
        //     // OrderList.Add(new Order("f46e1152-8b14-4a38-aa68-c85fa50b0e0b",
        //     //     "13.735224942208344,100.54246507083465"));
        //     // OrderList.Add(new Order("b75b7a37-47da-4ff0-8bb2-8bee53a79a35",
        //     //     "13.737555763388642,100.54758994127575"));
        //     // //var totalDistance = getTotalDistanceFromGoogleApi(OrderList);
        //     // var key = _config.GetSection("AppSettings:ApiKey").Value;
        //     // string tmpWarehouseGPS = "13.698936,100.487154";
        //     // string[] warehouseGPS = tmpWarehouseGPS.Split(",");
        //     // List<Location> dest = new List<Location>();
        //     // foreach (var order in OrderList)
        //     // {
        //     //     string[] gps = order.gps.Split(",");
        //     //     Location dropPoint = new Location(Double.Parse(gps[0]), Double.Parse(gps[1]));
        //     //     dest.Add(dropPoint);
        //     // }
        //     // Location origin = new Location(Double.Parse(warehouseGPS[0]), Double.Parse(warehouseGPS[1]));
        //     // //dest.Add(origin);
        //     // Location[] destination = dest.ToArray();
        //     // dest.Clear();
        //     // var request2 = new DirectionsRequest
        //     // {
        //     //     Key = key,
        //     //     Origin = origin,
        //     //     Destination = origin,
        //     //     Waypoints = destination,
        //     //     TravelMode = TravelMode.Driving,
        //     //     Avoid = AvoidWay.Tolls,
        //     //     Units = Units.Metric
        //     // };
        //     // var response = GoogleApi.GoogleMaps.Directions.Query(request2);
        //     // if (response.Status == Status.Ok)
        //     // {
        //     //     var legs = response.Routes.First().Legs;
        //     //     foreach (var leg in legs)
        //     //     {
        //     //         totalDistance = totalDistance + leg.Distance.Value;
        //     //         totalSeconds = totalSeconds + leg.Duration.Value;
        //     //     }
        //     //     double distance = totalDistance / 1000.0;
        //     //     int days = totalSeconds / 86400;
        //     //     int hours = (totalSeconds - days * 86400) / 3600;
        //     //     int minutes = (totalSeconds - days * 86400 - hours * 3600) / 60;
        //     //     int seconds = totalSeconds - days * 86400 - hours * 3600 - minutes * 60;
        //     //     Result result = new Result(distance, days, hours, minutes, seconds);
        //     //     return Ok(result);
        //     // }
        //     // return BadRequest();
        // }
        private async Task<bool> updateFirstTrip(List<Order> firstTripOrder, string truckCode)
        {
            bool status = false;
            foreach (var order in firstTripOrder)
            {
                await _repo.updateDelivery(order.deliveryId, truckCode, "1");
            }
            status = true;
            return status;
        }
        private async Task<bool> updateSecondTrip(List<Order> secondTripOrder, string truckCode)
        {
            bool status = false;
            foreach (var order in secondTripOrder)
            {
                await _repo.updateDelivery(order.deliveryId, truckCode, "1");
            }
            status = true;
            return status;
        }
        private async Task<bool> merge(List<OrderList> firstTrip, List<OrderList> secondTrip, IEnumerable<Truck> trucks)
        {
            bool status = false;
            List<TruckOrder> orderList = new List<TruckOrder>();
            foreach (var truck in trucks)
            {
                foreach (var first in firstTrip)
                {
                    foreach (var second in secondTrip)
                    {
                        double totalDistance = first.getTotalDistance() + second.getTotalDistance();
                        TruckOrder order = new TruckOrder(first.getDeliveryIdList(),
                         second.getDeliveryIdList(), totalDistance, first.getIndex(), second.getIndex(), truck.truckCode);
                        orderList.Add(order);
                    }
                    var averageDistance = getAverageDistance(orderList);
                    var closestDistance = getClosestDistance(orderList, averageDistance);
                    foreach (var item in orderList)
                    {
                        if (item.getTotaldistance() == closestDistance)
                        {
                            var firstIndex = item.getFirstIndex();
                            var secondIndex = item.getSecondIndex();
                            //update db
                            var firstTripOrder = item.getFirstTripOrder();
                            var secondTripOrder = item.getSecordTripOrder();
                            foreach (var firstOrder in firstTripOrder)
                            {
                                var delivery = await _repo.updateDelivery(firstOrder.deliveryId, item.truckCode, "1");
                            }
                            foreach (var secondOrder in secondTripOrder)
                            {
                                var delivery = await _repo.updateDelivery(secondOrder.deliveryId, item.truckCode, "2");
                            }
                            orderList.Clear();
                            int firstTripUsedIndex = firstTrip.FindIndex(d => d.getIndex() == firstIndex);
                            int secondTripUsedIndex = secondTrip.FindIndex(d => d.getIndex() == secondIndex);
                            firstTrip.RemoveAt(firstTripUsedIndex);
                            secondTrip.RemoveAt(secondTripUsedIndex);
                            break;
                        }
                    }
                    break;
                }
            }
            status = true;
            return status;
        }
        private double getClosestDistance(List<TruckOrder> orderList, double averageDistance)
        {
            List<MergeOrder> listOfDistance = new List<MergeOrder>();
            double closestDistance;
            foreach (var item in orderList)
            {
                var distance = Math.Abs(item.getTotaldistance() - averageDistance);
                MergeOrder m = new MergeOrder(item.getTotaldistance(), distance);
                listOfDistance.Add(m);
            }
            var sortList = listOfDistance.OrderBy(d => d.nearestPoint).ToList();
            closestDistance = sortList[0].totalDistance;
            return closestDistance;
        }
        private double getAverageDistance(List<TruckOrder> orderList)
        {
            double distance = 0;
            foreach (var list in orderList)
            {
                distance = distance + list.getTotaldistance();
            }
            var averageDistance = Math.Round(distance / orderList.Count);
            return averageDistance;
        }

        private double getTotalDistance(List<Order> orders)
        {
            double totalDistance = 0;
            string tmpWarehouseGPS = "13.698936,100.487154";
            string[] warehouseGPS = tmpWarehouseGPS.Split(",");
            Location origin = new Location(Double.Parse(warehouseGPS[0]), Double.Parse(warehouseGPS[1]));
            List<Location> route = new List<Location>();
            route.Add(origin);
            foreach (var order in orders)
            {
                string[] gps = order.gps.Split(",");
                Location dropPoint = new Location(Double.Parse(gps[0]), Double.Parse(gps[1]));
                route.Add(dropPoint);
            }
            route.Add(origin);
            for (int i = 0; i + 1 < route.Count; i++)
            {
                double lat1 = route[i].Latitude;
                double long1 = route[i].Longitude;
                double lat2 = route[i + 1].Latitude;
                double long2 = route[i + 1].Longitude;
                totalDistance = totalDistance +
                    DistanceMetrix.getDistanceMetrixInKM(lat1, long1, lat2, long2);
            }
            return totalDistance;
        }
        private Result getTotalDistanceFromGoogleApi(List<Order> orders)
        {
            var key = _config.GetSection("AppSettings:ApiKey").Value;
            double totalDistance = 0;
            int totalSeconds = 0;
            string tmpWarehouseGPS = "13.698936,100.487154";
            string[] warehouseGPS = tmpWarehouseGPS.Split(",");
            List<Location> dest = new List<Location>();
            foreach (var order in orders)
            {
                string[] gps = order.gps.Split(",");
                Location dropPoint = new Location(Double.Parse(gps[0]), Double.Parse(gps[1]));
                dest.Add(dropPoint);
            }
            Location origin = new Location(Double.Parse(warehouseGPS[0]), Double.Parse(warehouseGPS[1]));
            //dest.Add(origin);
            Location[] destination = dest.ToArray();
            dest.Clear();
            var request2 = new DirectionsRequest
            {
                Key = key,
                Origin = origin,
                Destination = origin,
                Waypoints = destination,
                TravelMode = TravelMode.Driving,
                Avoid = AvoidWay.Tolls,
                Units = Units.Metric
            };
            var response = GoogleApi.GoogleMaps.Directions.Query(request2);
            if (response.Status == Status.Ok)
            {
                var legs = response.Routes.First().Legs;
                foreach (var leg in legs)
                {
                    totalDistance = totalDistance + leg.Distance.Value;
                    totalSeconds = totalSeconds + leg.Duration.Value;
                }
            }
            double distance = totalDistance / 1000.0;
            int days = totalSeconds / 86400;
            int hours = (totalSeconds - days * 86400) / 3600;
            int minutes = (totalSeconds - days * 86400 - hours * 3600) / 60;
            int seconds = totalSeconds - days * 86400 - hours * 3600 - minutes * 60;
            Result result = new Result(distance, days, hours, minutes, seconds);
            // var request = new DistanceMatrixRequest
            // {
            //     Key = key,
            //     Origins = new[] { new Location(Double.Parse(warehouseGPS[0]), Double.Parse(warehouseGPS[1])) },
            //     Destinations = destination,
            //     TravelMode = TravelMode.Driving,
            //     Units = Units.Metric,
            //     Avoid = AvoidWay.Tolls
            // };
            // var response = GoogleApi.GoogleMaps.DistanceMatrix.QueryAsync(request).Result;
            // if (response.Status == Status.Ok)
            // {
            //     var distance = (response.Rows.First().Elements.Last().Distance.Value / 1000);
            //     totalDistance = totalDistance + distance;
            //     // foreach (var row in response.Rows)
            // // {
            // //     foreach (var element in row.Elements)
            // //     {
            // //         if (element == row.Elements.Last())
            // //         {
            // //             totalDistance = totalDistance + (element.Distance.Value / 1000);
            // //         }
            // //     }
            // // }
            // }
            return result;
        }
        [HttpPost("getpolyline")]
        public async Task<IActionResult> getPolyLine([FromBody]ViewMapDto viewMapDto)
        {
            var key = _config.GetSection("AppSettings:ApiKey").Value;
            string tmpWarehouseGPS = "13.698936,100.487154";
            string[] warehouseGPS = tmpWarehouseGPS.Split(",");
            List<Location> dest = new List<Location>();
            var deliveries = await _repo.getCarDeliveryPerTrip(viewMapDto.truckCode, "พร้อมส่ง",
                viewMapDto.trip);

            foreach (var item in deliveries)
            {
                string[] gps = item.Customer.gps.Split(",");
                Location dropPoint = new Location(Double.Parse(gps[0]), Double.Parse(gps[1]));
                dest.Add(dropPoint);
            }
            Location origin = new Location(Double.Parse(warehouseGPS[0]), Double.Parse(warehouseGPS[1]));
            //dest.Add(origin);
            Location[] destination = dest.ToArray();
            dest.Clear();
            var request2 = new DirectionsRequest
            {
                Key = key,
                Origin = origin,
                Destination = origin,
                Waypoints = destination,
                TravelMode = TravelMode.Driving,
                Avoid = AvoidWay.Tolls,
                OptimizeWaypoints = true,
                Units = Units.Metric
            };
            var response = GoogleApi.GoogleMaps.Directions.Query(request2);
            if (response.Status == Status.Ok)
            {
                var overviewPath = response.Routes.First().OverviewPath;
                return Ok(overviewPath);
            }
            return BadRequest();
        }
        private async Task<bool> firstTrip(IEnumerable<Truck> trucks, string transdate, double quantity)
        {
            bool status = false;
            List<Order> firstTripId = new List<Order>();
            int cu = 0;
            int carQauntity = 0;
            if (quantity < 80)
            {
                foreach (var truck in trucks)
                {
                    if (await _repo.hasPendingOrder())
                    {
                        //มี order ค้างจากวันก่อน
                        var pendingOrder = await _repo.getUnassignPendingDelivery(transdate);
                        if (pendingOrder != null)
                        {
                            var sortedPendingOrder = findDistanceAllCombi(pendingOrder);
                            foreach (var order in sortedPendingOrder)
                            {
                                Order o = new Order();
                                cu = cu + order.quantity;
                                if (cu < quantity)
                                {
                                    //ลงรถได้                                  
                                    o.deliveryId = order.deliveryId;
                                    o.gps = order.gps;
                                    o.quantity = order.quantity;
                                    firstTripId.Add(o);
                                    carQauntity = carQauntity + order.quantity;
                                }
                                else
                                {
                                    //ลงรถไม่ได้
                                    if (cu < 80)
                                    {
                                        //ถ้ายอดสะสม < 80 ลงรถได้
                                        o.deliveryId = order.deliveryId;
                                        o.gps = order.gps;
                                        o.quantity = order.quantity;
                                        firstTripId.Add(o);
                                        cu = 0;
                                        carQauntity = 0;
                                        break;
                                    }
                                    else
                                    {
                                        //ถอยค่าสะสม
                                        cu = cu - order.quantity;
                                    }
                                }
                            }
                            //คำนวณระยะทาง
                            //var totalDistance = getTotalDistance(firstTripId);
                            Result result = getTotalDistanceFromGoogleApi(firstTripId);
                            var totalDistance = result.totalDistance;
                            var hour = result.hours;
                            if (hour > 5)
                            {
                                var min = firstTripId.Min(q => q.quantity);
                                //เอา order ที่จำนวนถังน้อยสุดออก
                                foreach (var item in firstTripId)
                                {
                                    if (item.quantity == min)
                                    {
                                        firstTripId.Remove(item);
                                    }
                                }
                                //จัดลง list เที่ยวแรก
                                var index = firstTripList.Count + 1;
                                List<Order> deliveryIdList = firstTripId.ToList();
                                OrderList first = new OrderList(deliveryIdList, totalDistance, index);
                                firstTripList.Add(first);
                                //update db
                                foreach (var deliveryId in firstTripId)
                                {
                                    //update status = จัดแล้ว
                                    await updateJob(deliveryId.deliveryId);
                                }

                            }
                            else
                            {
                                //จัดลง list เที่ยวแรก
                                var index = firstTripList.Count + 1;
                                List<Order> deliveryIdList = firstTripId.ToList();
                                OrderList first = new OrderList(deliveryIdList, totalDistance, index);
                                firstTripList.Add(first);
                                //update db
                                foreach (var deliveryId in firstTripId)
                                {
                                    //update status = จัดแล้ว
                                    await updateJob(deliveryId.deliveryId);
                                }
                            }
                            firstTripId.Clear();
                            cu = 0;
                            carQauntity = 0;
                        }
                    }
                    else
                    {
                        //ไม่มี order ค้างจากวันก่อน
                        var deliveries = await _repo.getDescUnassignDelivery(transdate);
                        if (deliveries != null)
                        {
                            var sortedOrder = findDistanceAllCombi(deliveries);
                            foreach (var order in sortedOrder)
                            {
                                Order o = new Order();
                                cu = cu + order.quantity;
                                if (cu < quantity)
                                {
                                    //ลงรถได้
                                    o.deliveryId = order.deliveryId;
                                    o.gps = order.gps;
                                    o.quantity = order.quantity;
                                    firstTripId.Add(o);
                                    carQauntity = carQauntity + order.quantity;
                                }
                                else
                                {
                                    //ลงรถไม่ได้
                                    if (cu < 80)
                                    {
                                        //ถ้ายอดสะสม < 80 ลงรถได้
                                        //ลงรถได้
                                        o.deliveryId = order.deliveryId;
                                        o.gps = order.gps;
                                        o.quantity = order.quantity;
                                        firstTripId.Add(o);
                                        cu = 0;
                                        carQauntity = 0;
                                        break;
                                    }
                                    else
                                    {
                                        //ถอยค่าสะสม
                                        cu = cu - order.quantity;
                                    }
                                }
                            }
                            //คำนวณระยะทาง
                            //var totalDistance = getTotalDistance(firstTripId);
                            Result result = getTotalDistanceFromGoogleApi(firstTripId);
                            var totalDistance = result.totalDistance;
                            var hour = result.hours;
                            if (hour > 5)
                            {
                                //เอา order ที่จำนวนถังน้อยสุดออก
                                var min = firstTripId.Min(q => q.quantity);
                                //เอา order ที่จำนวนถังน้อยสุดออก
                                foreach (var item in firstTripId)
                                {
                                    if (item.quantity == min)
                                    {
                                        firstTripId.Remove(item);
                                    }
                                }
                                //จัดลง list เที่ยวแรก
                                var index = firstTripList.Count + 1;
                                List<Order> deliveryIdList = firstTripId.ToList();
                                OrderList first = new OrderList(deliveryIdList, totalDistance, index);
                                firstTripList.Add(first);
                                //update db
                                foreach (var deliveryId in firstTripId)
                                {
                                    //update status = จัดแล้ว
                                    await updateJob(deliveryId.deliveryId);
                                }
                            }
                            else
                            {
                                //จัดลง list เที่ยวแรก
                                var index = firstTripList.Count + 1;
                                List<Order> deliveryIdList = firstTripId.ToList();
                                OrderList first = new OrderList(deliveryIdList, totalDistance, index);
                                firstTripList.Add(first);
                                //update db
                                foreach (var deliveryId in firstTripId)
                                {
                                    //update status = จัดแล้ว
                                    await updateJob(deliveryId.deliveryId);
                                }
                            }
                            firstTripId.Clear();
                            cu = 0;
                            carQauntity = 0;
                        }
                    }
                }
                status = true;
            }
            else
            {
                //ใช้รถเสริม
                string zoneId = "EF070666-88E9-42C9-96D5-FD2E59E50791";
                int noTruckNeed = (int)Math.Round((quantity / 160));
                var availabletruck = await _repo.getAdditionalTruck(zoneId, noTruckNeed);
                var randomTruck = availabletruck.Randomize();
                foreach (var truck in randomTruck)
                {
                    if (await _repo.hasPendingOrder())
                    {
                        //มี order ค้าง
                        var pendingOrder = await _repo.getUnassignPendingDelivery(transdate);
                        if (pendingOrder != null)
                        {
                            var sortedPendingOrder = findDistanceAllCombi(pendingOrder);
                            foreach (var order in sortedPendingOrder)
                            {
                                Order o = new Order();
                                cu = cu + order.quantity;
                                if (cu < quantity)
                                {
                                    //ลงรถได้
                                    o.deliveryId = order.deliveryId;
                                    o.gps = order.gps;
                                    o.quantity = order.quantity;
                                    firstTripId.Add(o);
                                    carQauntity = carQauntity + order.quantity;
                                }
                                else
                                {
                                    //ลงรถไม่ได้
                                    if (cu < 80)
                                    {
                                        //ถ้ายอดสะสม < 80 ลงรถได้
                                        o.deliveryId = order.deliveryId;
                                        o.gps = order.gps;
                                        o.quantity = order.quantity;
                                        firstTripId.Add(o);
                                        cu = 0;
                                        carQauntity = 0;
                                        break;
                                    }
                                    else
                                    {
                                        //ถอยค่าสะสม
                                        cu = cu - order.quantity;
                                    }
                                }
                            }
                            //คำนวณระยะทาง
                            //var totalDistance = getTotalDistance(firstTripId);
                            Result result = getTotalDistanceFromGoogleApi(firstTripId);
                            var totalDistance = result.totalDistance;
                            var hour = result.hours;
                            if (hour > 5)
                            {
                                //เอา order ที่จำนวนถังน้อยสุดออก
                                var min = firstTripId.Min(q => q.quantity);
                                //เอา order ที่จำนวนถังน้อยสุดออก
                                foreach (var item in firstTripId)
                                {
                                    if (item.quantity == min)
                                    {
                                        firstTripId.Remove(item);
                                    }
                                }
                                //จัดลง list เที่ยวแรก
                                var index = firstTripList.Count + 1;
                                List<Order> deliveryIdList = firstTripId.ToList();
                                OrderList first = new OrderList(deliveryIdList, totalDistance, index);
                                firstTripList.Add(first);
                                //update db
                                foreach (var deliveryId in firstTripId)
                                {
                                    //update status = จัดแล้ว
                                    await updateJob(deliveryId.deliveryId);
                                }
                            }
                            else
                            {
                                //จัดลง list เที่ยวแรก
                                var index = firstTripList.Count + 1;
                                List<Order> deliveryIdList = firstTripId.ToList();
                                OrderList first = new OrderList(deliveryIdList, totalDistance, index);
                                firstTripList.Add(first);
                                //update db
                                foreach (var deliveryId in firstTripId)
                                {
                                    //update status = จัดแล้ว
                                    await updateJob(deliveryId.deliveryId);
                                }
                            }
                            firstTripId.Clear();
                            cu = 0;
                            carQauntity = 0;
                        }
                    }
                    else
                    {
                        //ไม่มี order ค้างจากวันก่อน
                        var deliveries = await _repo.getDescUnassignDelivery(transdate);
                        if (deliveries != null)
                        {
                            var sortedOrder = findDistanceAllCombi(deliveries);
                            foreach (var order in sortedOrder)
                            {
                                Order o = new Order();
                                cu = cu + order.quantity;
                                if (cu < quantity)
                                {
                                    //ลงรถได้
                                    o.deliveryId = order.deliveryId;
                                    o.gps = order.gps;
                                    o.quantity = order.quantity;
                                    firstTripId.Add(o);
                                    carQauntity = carQauntity + order.quantity;
                                }
                                else
                                {
                                    //ลงรถไม่ได้
                                    if (cu < 80)
                                    {
                                        //ถ้ายอดสะสม < 80 ลงรถได้
                                        o.deliveryId = order.deliveryId;
                                        o.gps = order.gps;
                                        o.quantity = order.quantity;
                                        firstTripId.Add(o);
                                        cu = 0;
                                        carQauntity = 0;
                                        break;
                                    }
                                    else
                                    {
                                        //ถอยค่าสะสม
                                        cu = cu - order.quantity;
                                    }
                                }
                            }
                            //คำนวณระยะทาง
                            //var totalDistance = getTotalDistance(firstTripId);
                            Result result = getTotalDistanceFromGoogleApi(firstTripId);
                            var totalDistance = result.totalDistance;
                            var hour = result.hours;
                            if (hour > 5)
                            {
                                //เอา order ที่จำนวนถังน้อยสุดออก
                                var min = firstTripId.Min(q => q.quantity);
                                //เอา order ที่จำนวนถังน้อยสุดออก
                                foreach (var item in firstTripId)
                                {
                                    if (item.quantity == min)
                                    {
                                        firstTripId.Remove(item);
                                    }
                                }
                                //จัดลง list เที่ยวแรก
                                var index = firstTripList.Count + 1;
                                List<Order> deliveryIdList = firstTripId.ToList();
                                OrderList first = new OrderList(deliveryIdList, totalDistance, index);
                                firstTripList.Add(first);
                                //update db
                                foreach (var deliveryId in firstTripId)
                                {
                                    //update status = จัดแล้ว
                                    await updateJob(deliveryId.deliveryId);
                                }
                            }
                            else
                            {
                                //จัดลง list เที่ยวแรก
                                var index = firstTripList.Count + 1;
                                List<Order> deliveryIdList = firstTripId.ToList();
                                OrderList first = new OrderList(deliveryIdList, totalDistance, index);
                                firstTripList.Add(first);
                                //update db
                                foreach (var deliveryId in firstTripId)
                                {
                                    //update status = จัดแล้ว
                                    await updateJob(deliveryId.deliveryId);
                                }
                            }
                            firstTripId.Clear();
                            cu = 0;
                            carQauntity = 0;
                        }
                    }
                }
                status = true;
            }
            return status;
        }
        private async Task<bool> secondTrip(IEnumerable<Truck> trucks, string transdate, double quantity)
        {
            bool status = false;
            List<Order> secondTripId = new List<Order>();
            int cu = 0;
            int carQauntity = 0;
            if (quantity < 80)
            {
                foreach (var truck in trucks)
                {
                    if (await _repo.hasPendingOrder())
                    {
                        //มี order ค้างจากวันก่อน
                        var pendingOrder = await _repo.getUnassignPendingDelivery(transdate);
                        if (pendingOrder != null)
                        {
                            var sortedPendingOrder = findDistanceAllCombi(pendingOrder);
                            foreach (var order in sortedPendingOrder)
                            {
                                Order o = new Order();
                                cu = cu + order.quantity;
                                if (cu < quantity)
                                {
                                    //ลงรถได้
                                    o.deliveryId = order.deliveryId;
                                    o.gps = order.gps;
                                    o.quantity = order.quantity;
                                    secondTripId.Add(o);
                                    carQauntity = carQauntity + order.quantity;
                                }
                                else
                                {
                                    //ลงรถไม่ได้
                                    if (cu < 80)
                                    {
                                        //ถ้ายอดสะสม < 80 ลงรถได้
                                        o.deliveryId = order.deliveryId;
                                        o.gps = order.gps;
                                        o.quantity = order.quantity;
                                        secondTripId.Add(o);
                                        cu = 0;
                                        carQauntity = 0;
                                        break;
                                    }
                                    else
                                    {
                                        //ถอยค่าสะสม
                                        cu = cu - order.quantity;
                                    }
                                }
                            }
                            //คำนวณระยะทาง
                            //var totalDistance = getTotalDistance(secondTripId);
                            Result result = getTotalDistanceFromGoogleApi(secondTripId);
                            var totalDistance = result.totalDistance;
                            var hour = result.hours;
                            if (hour > 5)
                            {
                                //เอา order ที่จำนวนถังน้อยสุดออก
                                var min = secondTripId.Min(q => q.quantity);
                                foreach (var item in secondTripId)
                                {
                                    if (item.quantity == min)
                                    {
                                        secondTripId.Remove(item);
                                    }
                                }
                                //จัดลง list เที่ยว 2
                                var index = secondTripList.Count + 1;
                                List<Order> deliveryIdList = secondTripId.ToList();
                                OrderList second = new OrderList(deliveryIdList, totalDistance, index);
                                secondTripList.Add(second);
                                //update db
                                foreach (var deliveryId in secondTripId)
                                {
                                    await updateJob(deliveryId.deliveryId);
                                }
                            }
                            else
                            {
                                //จัดลง list เที่ยว 2
                                var index = secondTripList.Count + 1;
                                List<Order> deliveryIdList = secondTripId.ToList();
                                OrderList second = new OrderList(deliveryIdList, totalDistance, index);
                                secondTripList.Add(second);
                                //update db
                                foreach (var deliveryId in secondTripId)
                                {
                                    await updateJob(deliveryId.deliveryId);
                                }
                            }
                            secondTripId.Clear();
                            cu = 0;
                            carQauntity = 0;
                        }
                    }
                    else
                    {
                        //ไม่มี order ค้างจากวันก่อน
                        var deliveries = await _repo.getDescUnassignDelivery(transdate);
                        if (deliveries != null)
                        {
                            var sortedOrder = findDistanceAllCombi(deliveries);
                            foreach (var order in sortedOrder)
                            {
                                Order o = new Order();
                                cu = cu + order.quantity;
                                if (cu < quantity)
                                {
                                    //ลงรถได้
                                    o.deliveryId = order.deliveryId;
                                    o.gps = order.gps;
                                    o.quantity = order.quantity;
                                    secondTripId.Add(o);
                                    carQauntity = carQauntity + order.quantity;
                                }
                                else
                                {
                                    //ลงรถไม่ได้
                                    if (cu < 80)
                                    {
                                        //ถ้ายอดสะสม < 80
                                        o.deliveryId = order.deliveryId;
                                        o.gps = order.gps;
                                        o.quantity = order.quantity;
                                        secondTripId.Add(o);
                                        cu = 0;
                                        carQauntity = 0;
                                        break;
                                    }
                                    else
                                    {
                                        //ถอยค่าสะสม
                                        cu = cu - order.quantity;
                                    }
                                }
                            }
                            //คำนวณระยะทาง
                            //var totalDistance = getTotalDistance(secondTripId);
                            Result result = getTotalDistanceFromGoogleApi(secondTripId);
                            var totalDistance = result.totalDistance;
                            var hour = result.hours;
                            if (hour > 5)
                            {
                                //เอา order ที่จำนวนถังน้อยสุดออก
                                var min = secondTripId.Min(q => q.quantity);
                                foreach (var item in secondTripId)
                                {
                                    if (item.quantity == min)
                                    {
                                        secondTripId.Remove(item);
                                    }
                                }
                                //จัดลง list เที่ยว 2
                                var index = secondTripList.Count + 1;
                                List<Order> deliveryIdList = secondTripId.ToList();
                                OrderList second = new OrderList(deliveryIdList, totalDistance, index);
                                secondTripList.Add(second);
                                //update db
                                foreach (var deliveryId in secondTripId)
                                {
                                    await updateJob(deliveryId.deliveryId);
                                }
                            }
                            else
                            {
                                //จัดลง list เที่ยว 2
                                var index = secondTripList.Count + 1;
                                List<Order> deliveryIdList = secondTripId.ToList();
                                OrderList second = new OrderList(deliveryIdList, totalDistance, index);
                                secondTripList.Add(second);
                                //update db
                                foreach (var deliveryId in secondTripId)
                                {
                                    await updateJob(deliveryId.deliveryId);
                                }
                            }
                            secondTripId.Clear();
                            cu = 0;
                            carQauntity = 0;
                        }
                    }
                }
                status = true;
            }
            else
            {
                //ใช้รถเสริม
                string zoneId = "EF070666-88E9-42C9-96D5-FD2E59E50791";
                int noTruckNeed = (int)Math.Round((quantity / 160));
                var availabletruck = await _repo.getAdditionalTruck(zoneId, noTruckNeed);
                var randomTruck = availabletruck.Randomize();
                foreach (var truck in randomTruck)
                {
                    if (await _repo.hasPendingOrder())
                    {
                        //มี order ค้าง
                        var pendingOrder = await _repo.getUnassignPendingDelivery(transdate);
                        if (pendingOrder != null)
                        {
                            var sortedPendingOrder = findDistanceAllCombi(pendingOrder);
                            foreach (var order in sortedPendingOrder)
                            {
                                Order o = new Order();
                                cu = cu + order.quantity;
                                if (cu < quantity)
                                {
                                    //ลงรถได้
                                    o.deliveryId = order.deliveryId;
                                    o.gps = order.gps;
                                    o.quantity = order.quantity;
                                    secondTripId.Add(o);
                                    carQauntity = carQauntity + order.quantity;
                                }
                                else
                                {
                                    //ลงรถไม่ได้
                                    if (cu < 80)
                                    {
                                        //ถ้ายอดสะสม < 80 ลงรถได้
                                        o.deliveryId = order.deliveryId;
                                        o.gps = order.gps;
                                        o.quantity = order.quantity;
                                        secondTripId.Add(o);
                                        cu = 0;
                                        carQauntity = 0;
                                        break;
                                    }
                                    else
                                    {
                                        //ถอยค่าสะสม
                                        cu = cu - order.quantity;
                                    }
                                }
                            }
                            //คำนวณระยะทาง
                            //var totalDistance = getTotalDistance(secondTripId);
                            Result result = getTotalDistanceFromGoogleApi(secondTripId);
                            var totalDistance = result.totalDistance;
                            var hour = result.hours;
                            if (hour > 5)
                            {
                                //เอา order ที่จำนวนถังน้อยสุดออก
                                var min = secondTripId.Min(q => q.quantity);
                                foreach (var item in secondTripId)
                                {
                                    if (item.quantity == min)
                                    {
                                        secondTripId.Remove(item);
                                    }
                                }
                                //จัดลง list เที่ยว 2
                                var index = secondTripList.Count + 1;
                                List<Order> deliveryIdList = secondTripId.ToList();
                                OrderList second = new OrderList(deliveryIdList, totalDistance, index);
                                secondTripList.Add(second);
                                //update db
                                foreach (var deliveryId in secondTripId)
                                {
                                    await updateJob(deliveryId.deliveryId);
                                }
                            }
                            else
                            {
                                //จัดลง list เที่ยว 2
                                var index = secondTripList.Count + 1;
                                List<Order> deliveryIdList = secondTripId.ToList();
                                OrderList second = new OrderList(deliveryIdList, totalDistance, index);
                                secondTripList.Add(second);
                                //update db
                                foreach (var deliveryId in secondTripId)
                                {
                                    await updateJob(deliveryId.deliveryId);
                                }
                            }
                            secondTripId.Clear();
                            cu = 0;
                            carQauntity = 0;
                        }
                    }
                    else
                    {
                        //ไม่มี order ค้างจากวันก่อน
                        var deliveries = await _repo.getDescUnassignDelivery(transdate);
                        if (deliveries != null)
                        {
                            var sortedOrder = findDistanceAllCombi(deliveries);
                            foreach (var order in sortedOrder)
                            {
                                Order o = new Order();
                                cu = cu + order.quantity;
                                if (cu < quantity)
                                {
                                    //ลงรถได้
                                    o.deliveryId = order.deliveryId;
                                    o.gps = order.gps;
                                    o.quantity = order.quantity;
                                    secondTripId.Add(o);
                                    carQauntity = carQauntity + order.quantity;
                                }
                                else
                                {
                                    //ลงรถไม่ได้
                                    if (cu < 80)
                                    {
                                        //ถ้ายอดสะสม < 80 ลงรถได้
                                        o.deliveryId = order.deliveryId;
                                        o.gps = order.gps;
                                        o.quantity = order.quantity;
                                        secondTripId.Add(o);
                                        cu = 0;
                                        carQauntity = 0;
                                        break;
                                    }
                                    else
                                    {
                                        //ถอยค่าสะสม
                                        cu = cu - order.quantity;
                                    }
                                }
                            }
                            //คำนวณระยะทาง
                            //var totalDistance = getTotalDistance(secondTripId);
                            Result result = getTotalDistanceFromGoogleApi(secondTripId);
                            var totalDistance = result.totalDistance;
                            var hour = result.hours;
                            if (hour > 5)
                            {
                                //เอา order ที่จำนวนถังน้อยสุดออก
                                var min = secondTripId.Min(q => q.quantity);
                                foreach (var item in secondTripId)
                                {
                                    if (item.quantity == min)
                                    {
                                        secondTripId.Remove(item);
                                    }
                                }
                                //จัดลง list เที่ยว 2
                                var index = secondTripList.Count + 1;
                                List<Order> deliveryIdList = secondTripId.ToList();
                                OrderList second = new OrderList(deliveryIdList, totalDistance, index);
                                secondTripList.Add(second);
                                //update db
                                foreach (var deliveryId in secondTripId)
                                {
                                    await updateJob(deliveryId.deliveryId);
                                }
                            }
                            else
                            {
                                //จัดลง list เที่ยว 2
                                var index = secondTripList.Count + 1;
                                List<Order> deliveryIdList = secondTripId.ToList();
                                OrderList second = new OrderList(deliveryIdList, totalDistance, index);
                                secondTripList.Add(second);
                                //update db
                                foreach (var deliveryId in secondTripId)
                                {
                                    await updateJob(deliveryId.deliveryId);
                                }
                            }
                            secondTripId.Clear();
                            cu = 0;
                            carQauntity = 0;
                        }
                    }
                }
                status = true;
            }
            return status;
        }
        private List<DeliveryOrder> findDistanceAllCombi(IEnumerable<Delivery> delivery)
        {
            List<DeliveryOrder> deliveryOrders = new List<DeliveryOrder>();
            List<Delivery> deliveries = delivery.ToList();
            for (int i = 0; i < 1; i++)
            {
                deliveryOrders.Add(new DeliveryOrder(deliveries[i].deliveryId, 0, deliveries[i].quantity,
                   deliveries[i].Customer.gps, deliveries[i].cusCode));
                for (int j = i + 1; j < deliveries.Count; j++)
                {
                    string tmpGps1 = deliveries[i].Customer.gps;
                    string[] gps1 = tmpGps1.Split(",");
                    string tmpGps2 = deliveries[j].Customer.gps;
                    string[] gps2 = tmpGps2.Split(",");
                    double distanceToRefPoint = DistanceMetrix.getDistanceMetrixInKM(Double.Parse(gps1[0]),
                        Double.Parse(gps1[1]), Double.Parse(gps2[0]), Double.Parse(gps2[1]));
                    deliveryOrders.Add(new DeliveryOrder(deliveries[j].deliveryId, distanceToRefPoint,
                        deliveries[j].quantity, deliveries[j].Customer.gps, deliveries[j].cusCode));
                }
            }
            List<DeliveryOrder> sortedOrder = deliveryOrders.OrderBy(d => d.distanceToRefPoint).ToList();
            return sortedOrder;
        }
        private async Task<bool> updateJob(string deliveryId)
        {
            var delivery = await _repo.getDelivery(deliveryId);
            delivery.status = "จัดแล้ว";
            if (await _repo.saveAll())
            {
                return true;
            }
            return false;
        }
        private async Task<bool> updateIdleTime(string truckCode)
        {
            var truck = await _repo.searchCar(truckCode);
            if (truck.idleTime > 0)
            {
                truck.idleTime = 0;
            }
            else
            {
                truck.idleTime = truck.idleTime + 1;
            }
            if (await _repo.saveAll())
            {
                return true;
            }
            return false;
        }
        private async void resetUnassignDelivery(string deliveryId)
        {
            var delivery = await _repo.getDelivery(deliveryId);
            delivery.status = "unassign";
            delivery.truckCode = null;
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
            delivery.truckCode = carCode;
            delivery.status = "รอส่ง";
            if (await _repo.saveAll())
            {
                return true;
            }
            return false;
        }
        [HttpPost("getcustomerdelivery")]
        public async Task<IActionResult> getCustomerDelivery([FromBody] DeliveryForCustomerDto deliveryForCustomerDto)
        {
            var delivery = await _repo.getCustomerDelivery(deliveryForCustomerDto.cusCode, deliveryForCustomerDto.transdate);
            return Ok(delivery);
        }
        [HttpPost("cancel")]
        public async Task<IActionResult> cancelDelivery([FromBody] DeliveryForCancelAndChangeDateDto deliveryForCancelAndChangeDateDto)
        {
            var delivery = await _repo.cancelDelivery(deliveryForCancelAndChangeDateDto.deliveryId);
            if (delivery == null)
            {
                return NotFound(new { success = false });
            }
            return Ok(new { success = true });
        }

        [HttpPost("changedeliverydate")]
        public async Task<IActionResult> changeDeliveryDate([FromBody] DeliveryForChangeDateDto deliveryForChangeDateDto)
        {
            //var delivery = await _repo.cancelDelivery(deliveryForChangeDateDto.deliveryId);
            // if (delivery == null)
            // {
            //     return NotFound(new { success = false });
            // }
            // var deliveryToCreate = new Delivery
            // {
            //     deliveryId = deliveryForChangeDateDto.deliveryId,
            //     transDate = DateTime.Parse(deliveryForChangeDateDto.transDate),
            //     cusCode = deliveryForChangeDateDto.cusCode,
            //     status = "unassign"
            // };
            var createDelivery = await _repo.changeDeliveryDate(deliveryForChangeDateDto.deliveryId,
                deliveryForChangeDateDto.transDate);
            return Ok(new { success = true });
        }

        [HttpPost("updatesuccess")]
        public async Task<IActionResult> updateDeliverySuccessStatus([FromBody]DeliveryForUpdateStatusDto deliveryForUpdateStatusDto)
        {
            var delivery = await _repo.updateDeliveryStatus(deliveryForUpdateStatusDto.deliveryId,
                deliveryForUpdateStatusDto.giveback, deliveryForUpdateStatusDto.coupon);
            if (delivery == null)
            {
                return NotFound(new { success = false });
            }
            return Ok(new { success = true });
        }
        [HttpPost("updatefail")]
        public async Task<IActionResult> updateDeliveryFailStatus([FromBody]DeliveryForUpdateStatusDto deliveryForUpdateStatusDto)
        {
            var delivery = await _repo.updateDeliveryStatus(deliveryForUpdateStatusDto.deliveryId, deliveryForUpdateStatusDto.reason);
            if (delivery == null)
            {
                return NotFound(new { success = false });
            }
            return Ok(new { success = true });
        }
    }
}
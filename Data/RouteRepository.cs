using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RouteAPI.Models;

namespace RouteAPI.Data
{
    public class RouteRepository : IRouteRepository
    {
        private readonly DataContext _context;
        public RouteRepository(DataContext context)
        {
            _context = context;

        }
        public void add<T>(T entity) where T : class
        {
            _context.Add(entity); ;
        }

        public void delete<T>(T entity) where T : class
        {
            _context.Remove(entity);
        }
        public async Task<IEnumerable<Delivery>> getUnassignDelivery(string transdate)
        {
            var tempDate = DateTime.Parse(transdate);
            var delivery = await _context.Delivery.Include(c => c.Customer).Where(d => d.status == "unassign" && d.transDate == tempDate).ToListAsync();
            return delivery;
        }
        public async Task<IEnumerable<Delivery>> getWaitToSendDelivery(string transdate, string carcode)
        {
            var tempDate = DateTime.Parse(transdate);
            var delivery = await _context.Delivery.Include(c => c.Customer).Where(d => d.status == "รอส่ง" && d.transDate == tempDate && d.carCode == carcode).ToListAsync();
            return delivery;
        }

        public async Task<Delivery> getDelivery(string id)
        {
            var delivery = await _context.Delivery.Include(c => c.Customer).FirstOrDefaultAsync(d => d.deliveryId == id);
            return delivery;
        }
        public async Task<bool> saveAll()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<IEnumerable<Delivery>> getDescUnassignDelivery(string transdate)
        {
            var tempDate = DateTime.Parse(transdate);

            var delivery = await _context.Delivery.Include(c => c.Customer)
                .Where(d => d.status == "unassign" && d.transDate == tempDate && d.trip == null)
                .OrderByDescending(d => d.quantity)
                .ToListAsync();
            return delivery;
        }

        public async Task<IEnumerable<Delivery>> getFirstTripDelivery(string transdate)
        {
            var tempDate = DateTime.Parse(transdate);
            var delivery = await _context.Delivery.Include(c => c.Customer)
                .Where(d => d.status == "unassign" && d.transDate == tempDate && d.trip == "1")
                .OrderByDescending(d => d.quantity)
                .ToListAsync();
            return delivery;
        }

        public async Task<IEnumerable<Delivery>> getSecondTripDelivery(string transdate)
        {
            var tempDate = DateTime.Parse(transdate);
            var delivery = await _context.Delivery.Include(c => c.Customer)
                .Where(d => d.status == "unassign" && d.transDate == tempDate && d.trip == "2")
                .OrderByDescending(d => d.quantity)
                .ToListAsync();
            return delivery;
        }

        public async Task<IEnumerable<Truck>> getTrucks(string zondId)
        {
            var car = await _context.Truck
                .Where(z => z.zoneId == zondId && z.status == "available")
                .OrderBy(c => c.truckCode)
                .ToListAsync();
            return car;
        }

        public async Task<IEnumerable<Truck>> getCarDesc(string zondId)
        {
            var car = await _context.Truck
                .Where(z => z.zoneId == zondId && z.status == "available")
                .OrderByDescending(c => c.truckCode)
                .ToListAsync();
            return car;
        }

        public async Task<IEnumerable<Delivery>> getCarDelivery(string transdate, string carCode, string status)
        {
            var tempDate = DateTime.Parse(transdate);
            var delivery = await _context.Delivery
                .Include(c => c.Customer)
                .Where(d => d.status == status && d.transDate == tempDate && d.carCode == carCode)
                .ToListAsync();
            return delivery;
        }

        public async Task<Warehouse> getWarehouseGps(string warehoseId)
        {
            var warehouse = await _context.Warehouse.FirstOrDefaultAsync(w => w.warehouseId == warehoseId);
            return warehouse;
        }

        public async Task<Truck> addNewCar(Truck car)
        {
            await _context.Truck.AddAsync(car);
            await _context.SaveChangesAsync();
            return car;
        }
        public async Task<bool> CarExists(string truckCode)
        {
            if (await _context.Truck.AnyAsync(c => c.truckCode == truckCode))
            {
                return true;
            }
            return false;
        }

        public async Task<string> getLatestCarCode(string zondId)
        {
           string truckCode = await _context.Truck.Where(z => z.zoneId == zondId).MaxAsync(c => c.truckCode);
           return truckCode;
        }

        public async Task<Truck> searchCar(string truckCode)
        {
            var car = await _context.Truck.FirstOrDefaultAsync(c => c.truckCode == truckCode);
            return car;
        }

        public async Task<Customer> addNewCustomer(Customer customer)
        {
            await _context.Customer.AddAsync(customer);
            await _context.SaveChangesAsync();
            return customer;
        }

        public async Task<string> GetLatestCusCode()
        {
            string cusCode = await _context.Customer.MaxAsync(c => c.cusCode);
            return cusCode;
        }

        public async Task<bool> getPersonalLeaveStatus(string truckCode)
        {
            var car = await _context.Truck.FirstOrDefaultAsync(c => c.truckCode == truckCode);
            if(car.personalLeave == true)
            {
                return true;
            }
            return false;
        }

        public async Task<Truck> updatePersonalLeaveStatus(string truckCode)
        {
            var car = await _context.Truck.FirstOrDefaultAsync(c => c.truckCode == truckCode);
            if(car.personalLeave == false)
            {
                car.personalLeave = true;
                car.status = "ลากิจ";
            }
            else
            {
                car.personalLeave = false;
                car.status = "available";
            }
            await _context.SaveChangesAsync();
            return car;
        }

        public async Task<Truck> updateSickLeaveStatus(string truckCode)
        {
            var car = await _context.Truck.FirstOrDefaultAsync(c => c.truckCode == truckCode);
            if (car.sickLeave == false)
            {
                car.sickLeave = true;
                car.status = "ลาป่วย";
            }
            else
            {
                car.sickLeave = false;
                car.status = "available";
            }
            await _context.SaveChangesAsync();
            return car;
        }

        public async Task<Delivery> getCustomerDelivery(string cusCode, string transdate)
        {
            var tempDate = DateTime.Parse(transdate);
            var delivery = await _context.Delivery.FirstOrDefaultAsync(d => d.cusCode == cusCode && d.transDate == tempDate && d.status == "รอส่ง");
            return delivery;
        }

        public async Task<Delivery> cancelDelivery(string deliveryId)
        {
            var delivery = await _context.Delivery.FirstOrDefaultAsync(d => d.deliveryId == deliveryId);
            if (delivery == null)
            {
                return null;
            }
            delivery.status = "cancel";
            await _context.SaveChangesAsync();
            return delivery;
        }

        public async Task<Delivery> changeDeliveryDate(Delivery delivery)
        {
            await _context.Delivery.AddAsync(delivery);
            await _context.SaveChangesAsync();
            return delivery;
        }

        public async Task<Delivery> updateDeliveryStatus(string deliveryId)
        {
            var delivery = await _context.Delivery.FirstOrDefaultAsync(d => d.deliveryId == deliveryId);
            if (delivery == null)
            {
                return null;
            }
            delivery.status = "จัดส่งแล้ว";
            await _context.SaveChangesAsync();
            return delivery;
        }

        public async Task<Delivery> updateDeliveryStatus(string deliveryId, string reason)
        {
            var delivery = await _context.Delivery.FirstOrDefaultAsync(d => d.deliveryId == deliveryId);
            if (delivery == null)
            {
                return null;
            }
            delivery.status = "จัดส่งแล้ว";
            delivery.reason = reason;
            await _context.SaveChangesAsync();
            return delivery;
        }

        public async Task<Warehouse> addNewWarehouse(Warehouse warehouse)
        {
            await _context.Warehouse.AddAsync(warehouse);
            await _context.SaveChangesAsync();
            return warehouse;
        }

        public async Task<IEnumerable<AdditionalTruck>> getAdditionalTruck(string zondId, int additionalTruckNeed)
        {
            //var paramZoneId = new SqlParameter("@zoneId", zondId);
            //var paramAdditionalTruckNeed = new SqlParameter("@numberOfAddintonalCarNeed", additionalTruckNeed);
            var additionTruck = await _context.AdditionalTruck.FromSql("usp_GetAdditionalTruck @p0, @p1", 
               additionalTruckNeed, zondId).ToListAsync();
            return additionTruck;
        }

        public async Task<bool> hasPendingOrder()
        {
            var delivery = await _context.Delivery.Include(c => c.Customer).Where(d => d.status == "unassign").ToListAsync();
            var count = delivery.Count;
            if(count > 0)
            {
                return true;
            }
            return false;
        }

        public async  Task<IEnumerable<Delivery>> getUnassignPendingDelivery(string transDate)
        {
            var tempDate = DateTime.Parse(transDate);
            var delivery = await _context.Delivery.FromSql("usp_GetOtherDayOrders @p0", tempDate).ToListAsync();
            return delivery;
        }
    }
}
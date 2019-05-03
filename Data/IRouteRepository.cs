using System.Collections.Generic;
using System.Threading.Tasks;
using RouteAPI.Models;

namespace RouteAPI.Data
{
    public interface IRouteRepository
    {
        void add<T>(T entity) where T : class;
        void delete<T>(T entity) where T : class;

        Task<IEnumerable<Delivery>> getUnassignDelivery(string transdate);
        Task<IEnumerable<Delivery>> getWaitToSendDelivery(string transdate, string carcode);
        Task<IEnumerable<Delivery>> getDescUnassignDelivery(string transdate);
        Task<IEnumerable<Delivery>> getFirstTripDelivery(string transdate);
        Task<IEnumerable<Delivery>> getSecondTripDelivery(string transdate);
        Task<IEnumerable<Delivery>> getCarDelivery(string truckCode, string status, string transdate);
        Task<IEnumerable<Truck>> getTrucks(string zondId);
        Task<IEnumerable<AdditionalTruck>> getAdditionalTruck(string zondId, int additionalTruckNeed);

        Task<IEnumerable<Truck>> getCarDesc(string zondId);
        Task<Delivery> getDelivery(string id);
        Task<Warehouse> getWarehouseGps(string warehoseId);
        Task<Truck> addNewCar(Truck car);
        Task<string> getLatestCarCode(string zondId);

        Task<Truck> searchCar(string carCode);
        Task<Customer> addNewCustomer(Customer customer);
        Task<string> GetLatestCusCode();
        Task<bool> CarExists(string carCode);
        Task<bool> getPersonalLeaveStatus(string carCode);
        Task<Truck> updatePersonalLeaveStatus(string carCode);
        Task<Truck> updateSickLeaveStatus(string carCode);
        Task<bool> saveAll();
        Task<Delivery> getCustomerDelivery(string cusCode, string transdate);

        Task<Delivery> cancelDelivery(string deliveryId);
        Task<Delivery> changeDeliveryDate(string deliveryId, string transdate);

        Task<Delivery> updateDeliveryStatus(string deliveryId, string giveback, string coupon);
        Task<Delivery> updateDeliveryStatus(string deliveryId, string reason);
        Task<Warehouse> addNewWarehouse(Warehouse warehouse);
        Task<bool> hasPendingOrder();
        Task<IEnumerable<Delivery>> getUnassignPendingDelivery(string transdate);
        Task<Delivery> updateDelivery(string deliveryId, string truckCode, string trip);
        Task<IEnumerable<Warehouse>> getWarehouse();
        Task<IEnumerable<Delivery>> getCarDeliveryPerTrip(string truckCode, string status, string trip);
        Task<IEnumerable<Customer>> getNewCustomer();
        Task<Customer> updateZone(string cusCode, string zoneId, string gps, string cusCond, string cusType, string day, double distanceToWh);
        Task<bool> checkUsedDate(string transdate);

        Task<UsedDate> insertUsedDate(UsedDate usedDate);
        Task<Delivery> addNewDelivery(Delivery delivery);
        Task<IEnumerable<Customer>> getCustomerPerDay(string dayName);
    }
}
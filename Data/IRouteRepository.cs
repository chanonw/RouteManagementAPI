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
        Task<IEnumerable<Delivery>> getCarDelivery(string transdate,string carCode);
        Task<IEnumerable<Car>> getCar(string zondId);
        Task<IEnumerable<Car>> getCarDesc(string zondId);
        Task<Delivery> getDelivery(string id);
        Task<Warehouse> getWarehouseGps(string warehoseId);
        Task<Car> addNewCar(Car car);
        Task<string> getLatestCarCode(string zondId);

        Task<Car> searchCar(string carCode);
        Task<Customer> addNewCustomer(Customer customer);
        Task<string> GetLatestCusCode();
        Task<bool> CarExists(string carCode);
        Task<bool> getPersonalLeaveStatus(string carCode);
        Task<Car> updatePersonalLeaveStatus(string carCode);
        Task<Car> updateSickLeaveStatus(string carCode);
        Task<bool> saveAll();
        Task<Delivery> getCustomerDelivery(string cusCode, string transdate);

        Task<Delivery> cancelDelivery(string deliveryId);
        Task<Delivery> changeDeliveryDate(Delivery delivery);
    }
}
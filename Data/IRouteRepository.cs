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
        Task<Delivery> getDelivery(string id);
        Task<bool> saveAll();

    }
}
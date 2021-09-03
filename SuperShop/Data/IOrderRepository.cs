using SuperShop.Data.Entities;
using System.Linq;
using System.Threading.Tasks;

namespace SuperShop.Data
{
    public interface IOrderRepository : IGenericRepository<Order>
    {
        Task<IQueryable<Order>> GetOrderAsync(string userName); // Todas as encomendas de um determinado user

        Task<IQueryable<OrderDetailTemp>> GetDetailTempsAsync(string userName);
    }
}

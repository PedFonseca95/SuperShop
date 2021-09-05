using SuperShop.Data.Entities;
using SuperShop.Models;
using System.Linq;
using System.Threading.Tasks;

namespace SuperShop.Data
{
    public interface IOrderRepository : IGenericRepository<Order>
    {
        Task<IQueryable<Order>> GetOrderAsync(string userName); // Todas as encomendas de um determinado user

        Task<IQueryable<OrderDetailTemp>> GetDetailTempsAsync(string userName);

        Task AddItemToOrderAsync(AddItemViewModel model, string userName); // Adicionar itens à lista

        Task ModifyOrderDetailTempQuantityAsync(int id, double quantity); // Modificar a quantidade de cada item adicionado

        Task DeleteDetailTempAsync(int id);
    }
}

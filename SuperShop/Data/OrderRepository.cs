using Microsoft.EntityFrameworkCore;
using SuperShop.Data.Entities;
using SuperShop.Helpers;
using SuperShop.Models;
using System.Linq;
using System.Threading.Tasks;

namespace SuperShop.Data
{
    public class OrderRepository : GenericRepository<Order>, IOrderRepository
    {
        private readonly DataContext _context;
        private readonly IUserHelper _userHelper;

        public OrderRepository(DataContext context, IUserHelper userHelper) : base(context)
        {
            _context = context;
            _userHelper = userHelper;
        }

        public async Task AddItemToOrderAsync(AddItemViewModel model, string userName)
        {
            var user = await _userHelper.GetUserByEmailAsync(userName); // Verificar se o user existe

            if (user == null) // Se não existir
            {
                return;
            }

            var product = await _context.Products.FindAsync(model.ProductId); // Procurar o produto

            if (product == null)
            {
                return;
            }

            // Depois de se confirmar que os user e produto existem, vai buscar o que já existe na orderDetailTemp (para nao adicionar itens repetidos)
            var orderDetailTemp = await _context.OrderDetailTemps
                .Where(odt => odt.User == user && odt.Product == product)
                .FirstOrDefaultAsync();

            if (orderDetailTemp == null) // Se não houver nenhum item adicionado ainda
            {
                // Vamos criá-lo
                orderDetailTemp = new OrderDetailTemp
                {
                    Price = product.Price,
                    Product = product,
                    Quantity = model.Quantity,
                    User = user,
                };

                _context.OrderDetailTemps.Add(orderDetailTemp);
            }
            else
            {
                // Caso já existam itens adicionados
                orderDetailTemp.Quantity += model.Quantity; // modifica o valor do produto
                _context.OrderDetailTemps.Update(orderDetailTemp); // atualiza a quantidade do produto em questao na orderdetailtemp
            }

            await _context.SaveChangesAsync();
        }

        public async Task DeleteDetailTempAsync(int id)
        {
            var orderDetailTemp = await _context.OrderDetailTemps.FindAsync(id);

            if (orderDetailTemp == null)
            {
                return;
            }

            _context.OrderDetailTemps.Remove(orderDetailTemp);
            await _context.SaveChangesAsync();
        }

        public async Task<IQueryable<OrderDetailTemp>> GetDetailTempsAsync(string userName)
        {
            var user = await _userHelper.GetUserByEmailAsync(userName); // Verificar se o user existe

            if (user == null) // Se não existir
            {
                return null;
            }

            return _context.OrderDetailTemps // Vai buscar os temporários
                .Include(p => p.Product)
                .Where(o => o.User == user)
                .OrderBy(o => o.Product.Name);
        }

        public async Task<IQueryable<Order>> GetOrderAsync(string userName)
        {
            var user = await _userHelper.GetUserByEmailAsync(userName);

            if (user == null)
            {
                return null; // Retorna uma lista vazia
            }

            if (await _userHelper.IsUserInRoleAsync(user, "Admin")) // Se o utilizador for Admin, vamos buscar todas as encomendas
            {
                return _context.Orders
                    .Include(o => o.Items)
                    .ThenInclude(i => i.Product)
                    .OrderByDescending(o => o.OrderDate);
            }

            return _context.Orders // Quando não for Admin, mostrar as orders apenas daquele user
                .Include(o => o.Items)
                .ThenInclude(i => i.Product)
                .Where(o => o.User == user) // Vai buscar as orders criadas por este user
                .OrderByDescending(o => o.OrderDate);
        }

        public async Task ModifyOrderDetailTempQuantityAsync(int id, double quantity)
        {
            var orderDetailTemp = await _context.OrderDetailTemps.FindAsync(id);

            if (orderDetailTemp == null) // Se não existir
            {
                return;
            }

            // Caso exista
            orderDetailTemp.Quantity += quantity;
            if (orderDetailTemp.Quantity > 0) // E a quantidade do produto a adicionar for superior a 0
            {
                _context.OrderDetailTemps.Update(orderDetailTemp); // Adiciona o produto à orderdetailtemp
                await _context.SaveChangesAsync(); // guardar as alterações
            }
        }
    }
}

using Microsoft.EntityFrameworkCore;
using SuperShop.Data.Entities;
using SuperShop.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SuperShop.Data
{
    public class OrderRepository : GenericRepository<Order>, IOrderRepository
    {
        private readonly DataContext _context;
        private readonly IUserHelper _userHelper;

        public OrderRepository(DataContext context, IUserHelper userHelper) : base (context)
        {
            _context = context;
            _userHelper = userHelper;
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
    }
}

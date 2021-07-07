using Microsoft.AspNetCore.Identity;
using SuperShop.Data.Entities;
using SuperShop.Helpers;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SuperShop.Data
{
    /// <summary>
    /// Responsavel criar a base de dados com valores predefinidos inicialmente caso não existam
    /// </summary>
    public class SeedDb
    {
        private readonly DataContext _context;
        private readonly IUserHelper _userHelper;
        private Random _random;

        public SeedDb(DataContext context, IUserHelper userHelper)
        {
            _context = context;
            _userHelper = userHelper;
            _random = new Random();
        }

        public async Task SeedAsync()
        {
            await _context.Database.EnsureCreatedAsync(); // Se a BD não existir, cria-a

            var user = await _userHelper.GetUserByEmailAsync("pedro.pereira.fonseca@formandos.cinel.pt");
            if (user == null)
            {
                user = new User
                {
                    FirstName = "Pedro",
                    LastName = "Fonseca",
                    Email = "pedro.pereira.fonseca@formandos.cinel.pt",
                    UserName = "pedro.pereira.fonseca@formandos.cinel.pt",
                    PhoneNumber = "123456789"
                };

                var result = await _userHelper.AddUserAsync(user, "123456");

                if (result != IdentityResult.Success)
                {
                    throw new InvalidOperationException("Could not create the user in seeder");
                }
            }



            // Se não existirem produtos criados
            if (!_context.Products.Any())
            {
                AddProduct("iPhone X", user);
                AddProduct("Magic Mouse", user);
                AddProduct("iWatch Series 4", user);
                AddProduct("iPad Mini", user);
                await _context.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Cria um produto aleatório
        /// </summary>
        /// <param name="name">Nome do produto</param>
        private void AddProduct(string name, User user)
        {
            _context.Products.Add(new Product
            {
                Name = name,
                Price = _random.Next(1000),
                IsAvailable = true,
                Stock = _random.Next(100),
                User = user
            });
        }
    }
}

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

            // Verificar se as roles existem, senão cria-as
            await _userHelper.CheckRoleAsync("Admin");
            await _userHelper.CheckRoleAsync("Customer");

            // Vai buscar o user
            var user = await _userHelper.GetUserByEmailAsync("pedro.pereira.fonseca@formandos.cinel.pt");

            // Se o user não existir, cria-o
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

                // Adiciona o user com password (123456)
                var result = await _userHelper.AddUserAsync(user, "123456");

                // Caso algo corra mal, reporta mensagem de erro
                if (result != IdentityResult.Success)
                {
                    throw new InvalidOperationException("Could not create the user in seeder");
                }

                // Caso corra tudo bem significa que o user foi criado, então atribuimos-lhe uma das roles acima criadas, neste caso "Admin"
                await _userHelper.AddUserToToleAsync(user, "Admin");
            }

            // Verifia novamente se o user criado já está nesta role, pois caso o user exista ele não chega a entrar no if anterior e não faz esta verificação
            var isInRole = await _userHelper.IsUserInRoleAsync(user, "Admin");

            if (!isInRole) // Se o user não estiver na role "Admin"
            {
                await _userHelper.AddUserToToleAsync(user, "Admin");
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

using SuperShop.Data.Entities;
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

        private Random _random;

        public SeedDb(DataContext context)
        {
            _context = context;
            _random = new Random();
        }

        public async Task SeedAsync()
        {
            await _context.Database.EnsureCreatedAsync(); // Se a BD não existir, cria-a

            // Se não existirem produtos criados
            if (!_context.Products.Any())
            {
                AddProduct("iPhone X");
                AddProduct("Magic Mouse");
                AddProduct("iWatch Series 4");
                AddProduct("iPad Mini");
                await _context.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Cria um produto aleatório
        /// </summary>
        /// <param name="name">Nome do produto</param>
        private void AddProduct(string name)
        {
            _context.Products.Add(new Product
            {
                Nome = name,
                Price = _random.Next(1000),
                IsAvailable = true,
                Stock = _random.Next(100)
            });
        }
    }
}

﻿using Microsoft.EntityFrameworkCore;
using SuperShop.Data.Entities;

namespace SuperShop.Data
{
    public class DataContext : DbContext  // Responsavel pela ligação à base de dados
    {
        // Propriedade responsavel/ligada à tabela após ela ser criada, através do DataContext
        public DbSet<Product> Products { get; set; }

        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }
    }
}

using Microsoft.EntityFrameworkCore;
using ShoppingStore.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingStore.Infrastructure.Data
{
    public class ShoppingStoreContext(DbContextOptions<ShoppingStoreContext> options) : DbContext(options)
    {
        public virtual DbSet<CartItem> Items { get; set; }
        public virtual DbSet<ShoppingCart> Carts { get; set; }
        public virtual DbSet<Article> Articles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Seed data
            modelBuilder.Entity<Article>().HasData(
                new Article { Id = Guid.NewGuid(), SKU = "SKU001", Name = "Red T-Shirt", Price = 9.99 },
                new Article { Id = Guid.NewGuid(), SKU = "SKU002", Name = "Blue T-Shirt", Price = 11.99 },
                new Article { Id = Guid.NewGuid(), SKU = "SKU003", Name = "Green Blouson", Price = 99.99 }
            );
        }
    }
}

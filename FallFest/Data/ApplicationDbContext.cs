using FallFest.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace FallFest.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Item> Items { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<OrderType> OrderTypes { get; set; }
        public DbSet<ItemType> ItemTypes { get; set; } 
    }
}

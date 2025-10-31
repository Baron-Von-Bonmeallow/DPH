using Microsoft.AspNetCore.Identity;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol.Plugins;
using AutoPago.Models;
namespace AutoPago.Data
{
    public class PayContext : DbContext
    {
        public DbSet<Products> Products { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
    }
}

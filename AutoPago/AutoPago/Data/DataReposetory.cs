using Microsoft.AspNetCore.Identity;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol.Plugins;
using AutoPago.Models;
namespace AutoPago.Data
{
    public class DataReposetory : IDataReposetory
    {
        private readonly PayContext _context;
        public DataReposetory(PayContext context)
        {
            _context = context;
        }
        public IEnumerable<Products> GetAllProducts()
        {
            return _context.Products.ToList();
        }
        public Products GetProductById(int id)
        {
            return _context.Products.Find(id);
        }
        public Products GetProductByName(string name)
        {
            return _context.Products.Find(name);
        }
        public void AddProduct(Products product)
        {
            _context.Products.Add(product);
            _context.SaveChanges();
        }
        public Products GetProductByCode(string code)
        {
            return _context.Products.FirstOrDefault(p => p.Code == code);        }
        public void AddTicket(Ticket ticket)
        {
            _context.Tickets.Add(ticket);
            _context.SaveChanges();
        }
        public void AddProductToCart(Products products)
        {
            _context.Carts.Add(new Cart { Products = new List<Products> { products }, Subtotal = products.Price });
        } 
        public void DeleteItem(int id)
        {
            var item = _context.Carts.Find(id);
            if (item == null) return;

            _context.Carts.Remove(item);
            _context.SaveChanges();
        }
    }
}

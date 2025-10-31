using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SelfCheckOutMarket.Models;
using SelfCheckOutMarket.Data;

namespace SelfCheckOutMarket.Controllers
{
    public class ProductsController : Controller
    {
        private readonly MarketDbContext _context;

        public ProductsController(MarketDbContext context)
        {
            _context = context;
        }

        //Lo que se hace en este metodo es obtener la lista de productos de la base de datos y enviarla a la vista para que se muestre al usuario
        public async Task<IActionResult> Index()
        {
            var products = await _context.Products.ToListAsync();
            return View(products);
        }

        //Aqui se muestra el formulario para crear un nuevo producto
        public IActionResult Create()
        {
            return View();
        }

        //Lo que sucede en esta funcion es que se recibe el producto que el usuario lleno en el formulario y se guarda en la base de datos
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product)
        {
            if (ModelState.IsValid)
            {
                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(product);
        }

        //Aqui lo que se hace es obtener el producto que el usuario quiere editar y mostrarlo en un formulario para que el usuario pueda modificarlo
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .FirstOrDefaultAsync(m => m.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }


        //Aqui se recibe el id del producto que el usuario quiere eliminar y se elimina de la base de datos
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);

            if (product != null)
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
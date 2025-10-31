using Microsoft.AspNetCore.Mvc;
using SelfCheckOutMarket.Data;
using SelfCheckOutMarket.Models;
using System.Text.Json;

namespace SelfCheckOutMarket.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly MarketDbContext _context;
        private const string CartSessionKey = "Cart";

        public CheckoutController(MarketDbContext context)
        {
            _context = context;
        }

        // Página principal del módulo (Welcome)
        public IActionResult Index()
        {
            return View(); // Muestra Index.cshtml
        }

        // Inicio del proceso de compra → va a Checkout
        public IActionResult Checkout()
        {
            HttpContext.Session.Remove(CartSessionKey);
            return RedirectToAction("Scan");
        }

        // Página del carrito/escaneo (antes Scan)
        public IActionResult Scan()
        {
            var cart = GetCart();
            return View("Index", cart);
        }

        [HttpPost]
        public IActionResult AddProduct(string barcode)
        {
            if (string.IsNullOrEmpty(barcode))
                return RedirectToAction("Scan");

            var product = _context.Products.FirstOrDefault(p => p.barcode == barcode);
            if (product == null)
            {
                TempData["Error"] = "Producto no encontrado.";
                return RedirectToAction("Scan");
            }

            var cart = GetCart();
            var existing = cart.FirstOrDefault(c => c.Barcode == barcode);

            if (existing != null)
            {
                existing.Quantity++;
            }
            else
            {
                cart.Add(new CartItem
                {
                    Barcode = product.barcode,
                    Name = product.name,
                    Price = product.price,
                    Quantity = 1,
                });
            }

            SaveCart(cart);
            return RedirectToAction("Scan");
        }

        [HttpPost]
        public IActionResult RemoveProduct(string barcode)
        {
            var cart = GetCart();
            var item = cart.FirstOrDefault(c => c.Barcode == barcode);
            if (item != null)
                cart.Remove(item);

            SaveCart(cart);
            return RedirectToAction("Scan");
        }

        [HttpPost]
        public IActionResult Cancel()
        {
            HttpContext.Session.Remove(CartSessionKey);
            return RedirectToAction("Index");
        }

        private List<CartItem> GetCart()
        {
            var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>(CartSessionKey);
            return cart ?? new List<CartItem>();
        }

        private void SaveCart(List<CartItem> cart)
        {
            HttpContext.Session.SetObjectAsJson(CartSessionKey, cart);
        }

        [HttpGet]
        public IActionResult Payment()
        {
            var cart = GetCart();
            if (cart == null || !cart.Any())
            {
                TempData["Error"] = "The cart is empty.";
                return RedirectToAction("Scan");
            }

            ViewBag.Total = cart.Sum(i => i.Subtotal);
            return View();
        }

        [HttpPost]
        public IActionResult Payment(string method, decimal amount = 0, string cardNumber = "")
        {
            var cart = GetCart();
            if (cart == null || !cart.Any())
                return RedirectToAction("Index");

            decimal total = cart.Sum(i => i.Price * i.Quantity);
            decimal change = 0;

            if (method == "Cash")
            {
                if (amount < total)
                {
                    TempData["Error"] = "Monto insuficiente.";
                    ViewBag.Total = total;
                    return View();
                }
                change = amount - total;
            }
            else if (method == "Card")
            {
                decimal cardBalance = 500; // saldo ficticio
                if (string.IsNullOrEmpty(cardNumber))
                {
                    TempData["Error"] = "Debe ingresar un número de tarjeta.";
                    ViewBag.Total = total;
                    return View();
                }

                if (cardBalance < total)
                {
                    TempData["Error"] = "Fondos insuficientes en la tarjeta.";
                    ViewBag.Total = total;
                    return View();
                }

                amount = total; // pago exitoso
            }
            else
            {
                TempData["Error"] = "Método de pago inválido.";
                ViewBag.Total = total;
                return View();
            }

            // Guardar datos para el recibo
            TempData["Total"] = total.ToString();
            TempData["Change"] = change.ToString();
            TempData["PaymentMethod"] = method;

            // ✅ Guardar también el carrito antes de borrarlo
            TempData["Cart"] = System.Text.Json.JsonSerializer.Serialize(cart);

            // Ahora sí limpiamos la sesión
            HttpContext.Session.Remove(CartSessionKey);

            return RedirectToAction("Receipt");
        }


        public IActionResult Receipt()
        {
            List<CartItem> cart = new List<CartItem>();

            if (TempData["Cart"] != null)
            {
                cart = System.Text.Json.JsonSerializer.Deserialize<List<CartItem>>(TempData["Cart"].ToString());
            }

            ViewBag.Total = TempData["Total"];
            ViewBag.Change = TempData["Change"];
            ViewBag.PaymentMethod = TempData["PaymentMethod"];
            ViewBag.Date = DateTime.Now;

            return View(cart);
        }

    }
}


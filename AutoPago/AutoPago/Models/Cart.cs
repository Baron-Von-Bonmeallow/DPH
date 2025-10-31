namespace AutoPago.Models
{
    public class Cart
    {
        public ICollection<Products> Products { get; set; }
        public decimal Subtotal { get; set; }
    }
}

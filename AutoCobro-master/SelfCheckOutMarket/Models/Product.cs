namespace SelfCheckOutMarket.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string brand { get; set; } = string.Empty;
        public string name { get; set; } = string.Empty;
        public decimal price { get; set; }
        public string category { get; set; } = string.Empty; 
        public string barcode { get; set; } = string.Empty;
    }
}

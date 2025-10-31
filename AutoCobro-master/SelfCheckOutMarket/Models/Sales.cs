namespace SelfCheckOutMarket.Models
{
    public class Sales
    {
        public int Id { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;
        public string PaymentMethod { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public decimal Change { get; set; }
        public List<SalesItem> Items { get; set; } = new();
    }

}

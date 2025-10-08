namespace AutoPago.Models
{
    public class Ticket
    {
        public int Id { get; set; }
        public DateTimeOffset Date { get; set; }
        public string PaymentMethod { get; set; }
        public ICollection<Cart> Items { get; set; }
        public decimal Total { get; set; }
    }
}

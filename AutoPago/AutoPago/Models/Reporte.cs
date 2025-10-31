namespace AutoPago.Models
{
    public class Reporte
    {
        public int Id { get; set; }
        public DateTimeOffset Date { get; set; }
        public decimal TotalSales { get; set; }
        public int TotalTransactions { get; set; }
    }
}

namespace NumericSeries.Models
{
    public class SeriesViewModel
    {
        public string Series { get; set; } = string.Empty;
        public int N { get; set; }
        public ModelResults Result { get; set; } = new();
    }
}

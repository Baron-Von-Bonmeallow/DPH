using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace NumericSeries.Models
{
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public class ErrorViewModel
    {
        public string? RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
        public string? Message { get; set; }

    }
}

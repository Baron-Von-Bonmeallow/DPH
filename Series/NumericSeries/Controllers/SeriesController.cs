using Humanizer;
using Microsoft.AspNetCore.Mvc;
using NumericSeries.Models;

namespace NumericSeries.Controllers
{
    public class SeriesController : Controller
    {
        [HttpGet("/series/{series}/{n:min(0)}")]
        public IActionResult Index(string series, int n = 0)
        {
            Calculator? calculator = series.ToLowerInvariant() switch
            {
                "natural" => new NaturalCalculator(),
                "odd" => new OddCalculator(),
                "even" => new EvenCalculator(),
                "fibonacci" => new FibonacciCalculator(),
                "quadratic" => new QuadraticCalculator(),
                _ => null
            };

            var result = calculator?.Calculate(n) ?? new ModelResults
            {
                Series = "Unknown",
                N = n,
                Result = 0
            };

            return View(new SeriesViewModel
            {
                Series = series.ApplyCase(LetterCasing.Sentence),
                N = n,
                Result = result
            });

        }

    }
}

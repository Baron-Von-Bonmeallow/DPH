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
            if (n < 0)
            {
                return BadRequest("Negative numbers are not allowed for this series.");
            }

            Calculator? calculator = series.ToLowerInvariant() switch
            {
                "natural" => new NaturalCalculator(),
                "odd" => new OddCalculator(),
                "even" => new EvenCalculator(),
                "fibonacci" => new FibonacciCalculator(),
                "quadratic" => new QuadraticCalculator(),
                _ => null
            };

            if (calculator == null)
            {
                return NotFound($"Series '{series}' is not recognized.");
            }

            var result = calculator.Calculate(n);

            return View(new SeriesViewModel
            {
                Series = series.ApplyCase(LetterCasing.Sentence),
                N = n,
                Result = result
            });

        }

    }
}

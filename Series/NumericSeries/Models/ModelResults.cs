namespace NumericSeries.Models
{
    public class ModelResults
    {
        public string Series { get; set; } = string.Empty;
        public int N { get; set; }
        public long Result { get; set; }
    }
    public interface Calculator
    {
        ModelResults Calculate(int n);
    }
    public class NaturalCalculator : Calculator
    {
        public ModelResults Calculate(int n)
        {
            return new ModelResults
            {
            Series = "Natural",
            N = n,
            Result = n 
            };
        }
    }
    public class OddCalculator : Calculator
    {
        public ModelResults Calculate(int n)
        {
            return new ModelResults 
            {
            Series = "Odd",
            N = n,
            Result = n * 2 - 1
            };
        }
    }
    public class EvenCalculator : Calculator
    {
        public ModelResults Calculate(int n)
        {
            return new ModelResults
            {
                Series = "Even",
                N = n,
                Result = n * 2
            };
        }
    }
    public class FibonacciCalculator : Calculator
    {
        public ModelResults Calculate(int n)
        {
            long result;
            long a = 0, b = 1, c = 0;
            for (int i = 2; i <= n; i++)
            {
                c = a + b;
                a = b;
                b = c;
            }
            result = c;
            return new ModelResults
            {
                Series = "Fibonacci",
                N = n,
                Result = result
            };
        }
    }
    public class QuadraticCalculator : Calculator
    {
        public ModelResults Calculate(int n)
        {
            return new ModelResults
            {
                Series = "Quadratic",
                N = n,
                Result = n * n
            };
        }
    }
}

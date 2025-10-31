namespace SelfCheckOutMarket.Data
{
    public class SeedData
    {
        public static void Initialize(MarketDbContext context)
        {
            context.Database.EnsureCreated();

            if(context.Products.Any())
            {
                return; 
            }

            var lines = File.ReadAllLines("DataFiles/catalogoProductosMx.csv");
            
            // Add a counter to track the line number
            int lineNumber = 0;

            foreach (var line in lines)
            {
                // Skip the first line (header row)
                if (lineNumber == 0)
                {
                    lineNumber++;
                    continue;
                }

                var parts = line.Split(',');

                if(parts.Length >= 5)
                {
                    var priceString = parts[2].Replace("$", "").Replace(" MXN", "").Trim();
                    if (decimal.TryParse(priceString, out decimal price))
                    {
                        context.Products.Add(new Models.Product
                        {
                            brand = parts[0],
                            name = parts[1],
                            price = price,
                            category = parts[3],
                            barcode = parts[4]
                        });
                    }
                    else
                    {
                        // Handle parsing error, maybe log the error
                        Console.WriteLine($"Error parsing price: {parts[2]}");
                    }           
                }
                lineNumber++;
            }

            context.SaveChanges();
        }
    }
}

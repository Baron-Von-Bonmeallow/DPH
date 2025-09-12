namespace DiscountCodes.Models
{
    public class ShoppingCart
    {
        private IDiscount discount = new NoDiscount();
        private List<Product> items = new List<Product>();
        public IReadOnlyList<Product> Items => items.AsReadOnly();

        public void AddItem(Product product)
        {
            items.Add(product);
        }

        /// <summary>
        /// Returns the total price of all items in the cart, after applying any discounts.
        /// </summary>
        /// <returns></returns>
        public decimal GetTotal()
        {
            return items.Sum(item => item.Price)-discount.GetDiscountAmount(Items);
        }

        /// <summary>
        /// Applies a discount code to the shopping cart.
        /// If the code is invalid or empty, no discount is applied.
        /// Only one discount code can be applied at a time, applying a new code replaces the previous one.
        /// </summary>
        /// <param name="code"></param>
        public void ApplyDiscount(string code)
        {
            discount= code.ToUpper() switch
                {
                "BRAND2DISCOUNT" => new DPBP(0.10m,"Brand2"),
                "10PERCENTOFF" => new DPRC(0.10m),
                "BRAND1MANIA"=>new DPBP(0.50m,"Brand1"),
                "BOGOFREE"=> new MIDS(2),
                "5USDOFF"=>new Minus(5m),
                _ => new NoDiscount()
                };
            // TODO: Implement discount codes
        }
    }
}

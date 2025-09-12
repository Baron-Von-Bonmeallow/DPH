using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscountCodes.Models
{
    public interface IDiscount
    {
        decimal GetDiscountAmount(IEnumerable<Product> items);
    }
    public class NoDiscount:IDiscount 
    {
    public decimal GetDiscountAmount(IEnumerable<Product> items)
        {
            return 0;
        }
    }
    public class DPRC : IDiscount 
    {
        private readonly decimal porcentaje;
        public DPRC(decimal porcentaje) 
        {
            this.porcentaje = porcentaje;
        }
        public decimal GetDiscountAmount(IEnumerable<Product> items) 
        {
            decimal sbtotal = items.Sum(item => item.Price);
            return sbtotal;
        }
    }
}

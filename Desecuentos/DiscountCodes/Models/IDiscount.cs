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
            decimal sbtotal = items.Sum(item => item.Price*porcentaje);
            return sbtotal;
        }
    }
    public class DPBP:IDiscount 
        {
            private readonly decimal porcentaje;
            private readonly string marca;
            public DPBP(decimal porcentaje, string marca)
            {
                this.porcentaje = porcentaje;
                this.marca = marca;
            }

            public decimal GetDiscountAmount(IEnumerable<Product> items) 
            {
                return items.Where(item=>item.Brand==marca).Sum(item=>item.Price*porcentaje);
            }
        }
    public class Minus:IDiscount 
    {
        private decimal amount;
        public Minus(decimal amount)
        {
            this.amount = amount;
        }

        public decimal GetDiscountAmount(IEnumerable<Product> items) 
        {
            decimal value=items.Sum(item=>item.Price);
            if (value-amount>0) { return amount; }
            else { return 0; }
        }
    }
    public class MIDS:IDiscount 
    {
        private int amount;
        public MIDS(int amount) 
        {
        this.amount=amount;
        }
        public decimal GetDiscountAmount(IEnumerable<Product> items) 
        {
            decimal discount=0;
            var gitems = items.GroupBy(item => new { item.Brand, item.Price, });
            foreach ( var group in gitems) 
            {
            int fitem=group.Count()/amount;
            discount+= fitem*group.Key.Price;
            }
            return discount;
        }
    }
}

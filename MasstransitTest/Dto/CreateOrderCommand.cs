using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasstransitTest
{
    public class CreateOrderCommand
    {
        public string ProductId { get; set; }
        public string CustomerId { get; set; }
        public int Price { get; set; }
    }
}

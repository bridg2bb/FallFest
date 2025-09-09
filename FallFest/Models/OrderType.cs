// Models/OrderType.cs
using FallFest.Models;
using System.Collections.Generic;

namespace FallFest.Models
{
    public class OrderType
    {
        public int OrderTypeID { get; set; }
        public string TypeName { get; set; }

        // Navigation property
        public ICollection<Order> Orders { get; set; }
    }
}

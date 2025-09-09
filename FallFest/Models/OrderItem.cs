using System;

namespace FallFest.Models
{
    public class OrderItem
    {
        public int OrderItemID { get; set; }
        public Guid OrderID { get; set; }
        public int ItemID { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }

        // Navigation properties
        public Order Order { get; set; }
        public Item Item { get; set; }
    }
}

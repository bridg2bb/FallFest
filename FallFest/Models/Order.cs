﻿// Models/Order.cs
using System;
using System.Collections.Generic;

namespace FallFest.Models
{
    public class Order
    {
        public Guid OrderID { get; set; }
        public decimal AmountPaid { get; set; }
        public decimal AmountReturned { get; set; }
        public int OrderTypeID { get; set; }
        public DateTime TransactionDateTime { get; set; }

        // Navigation properties for relationships
        public OrderType OrderType { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; }
    }

    public class OrderStats
    {
        public int TotalOrders { get; set; }
        public double TotalAmount { get; set; }
        public Guid OrderId { get; set; }
    }

    // ViewModel to handle incoming data from the client-side JavaScript
    public class OrderRequestViewModel
    {
        public List<OrderItemViewModel> OrderItems { get; set; }
        public decimal AmountPaid { get; set; }
        public decimal AmountReturned { get; set; }
        public System.DateTime TransactionDateTime { get; set; }
    }

    // ViewModel for a single item in the order
    public class OrderItemViewModel
    {
        public string Name { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}

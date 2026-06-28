using System;
using System.Collections.Generic;

namespace ECommerceApp.Models
{
    public class Order
    {
        public int OrderId { get; set; }
        public int UserId { get; set; }
        public string CustomerName { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; }         // Pending, Confirmed, Shipped, Delivered
        public string PaymentMethod { get; set; }  // COD, Online, UPI, Card
        public string PaymentStatus { get; set; }  // Pending, Paid
        public string DeliveryAddress { get; set; }
        public DateTime OrderDate { get; set; }
        public List<OrderItem> Items { get; set; }
    }
}

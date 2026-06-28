using System;

namespace ECommerceApp.Models
{
    public class Payment
    {
        public int PaymentId { get; set; }
        public int OrderId { get; set; }
        public decimal Amount { get; set; }
        public string Method { get; set; }        // COD, Card, UPI, NetBanking
        public string Status { get; set; }        // Pending, Success, Failed
        public string TransactionId { get; set; }
        public DateTime PaymentDate { get; set; }
    }
}

﻿using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Domain.Entities
{
    public class OrderDetails
    {
        [Key]
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public ApplicationUser User { get; set; }
        public decimal Total { get; set; }
        public Guid PaymentId { get; set; }
        public Payment Payment { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public OrderPayment OrderPayment { get; set; }
    }
}

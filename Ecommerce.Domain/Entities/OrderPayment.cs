using System.ComponentModel.DataAnnotations;


namespace Ecommerce.Domain.Entities
{
    public class OrderPayment
    {
        [Key]
        public Guid Id { get; set; }
        public Guid Orderid { get; set; }
        public OrderDetails OrderDetails { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public string Provider { get; set; }
        public string Status { get; set; }
    }
}

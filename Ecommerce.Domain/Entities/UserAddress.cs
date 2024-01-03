namespace Ecommerce.Domain.Entities
{
    public class UserAddress
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public ApplicationUser User { get; set; }
        public AddressType AddressType { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string PostalCode { get; set; }
        public string CountryCode { get; set; }
        public string TelePhone { get; set; }
        public string MobilePhone { get; set; }
        public string Email { get; set; }
    }

    public enum AddressType
    {
        Home,
        Work,
        Shipping,
        Billing,
        Other
    }
}

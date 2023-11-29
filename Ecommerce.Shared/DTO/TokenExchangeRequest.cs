namespace Ecommerce.Shared.DTO;

public class TokenExchangeRequest
{
    public string GrantType { get; set; }
    public string ClientId { get; set; }
    public string ClientSecret { get; set; }
}
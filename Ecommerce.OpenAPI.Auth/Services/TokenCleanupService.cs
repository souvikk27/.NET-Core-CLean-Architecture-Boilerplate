namespace Ecommerce.OpenAPI.Auth.Services;

public interface ITokenCleanupService
{
    Task<IEnumerable<Object>> CleanupTokensAsync();
}

public class TokenCleanupService : ITokenCleanupService
{
    private readonly IOpenIddictTokenManager _tokenManager;

    public TokenCleanupService(IOpenIddictTokenManager tokenManager)
    {
        _tokenManager = tokenManager ?? throw new ArgumentNullException(nameof(tokenManager));  
    }

    public async Task<IEnumerable<Object>> CleanupTokensAsync()
    {
        var tokens = _tokenManager.ListAsync();
        return (IEnumerable<object>)await Task.FromResult(tokens);
    }
}
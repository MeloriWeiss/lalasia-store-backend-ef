namespace Lalasia_store.Controllers.Contracts.Auth;

public class AuthTokensResponse
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
}
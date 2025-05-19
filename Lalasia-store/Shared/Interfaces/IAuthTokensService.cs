namespace Lalasia_store.Shared.Interfaces;

public interface IAuthTokensService
{
    public string GenerateAccessToken(Guid userId, IList<string> roles);
    public string GenerateRefreshToken(Guid userId, IList<string> roles);
}
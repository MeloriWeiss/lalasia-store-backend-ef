namespace Lalasia_store.Shared.Interfaces;

public interface IAuthTokensService
{
    public string GenerateAccessToken(Guid userId);
    public string GenerateRefreshToken(Guid userId);
}
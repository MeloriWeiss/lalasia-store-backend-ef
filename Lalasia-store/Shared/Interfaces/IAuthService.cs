using System.Security.Claims;
using Lalasia_store.Controllers.Contracts.Auth;
using Lalasia_store.Controllers.Contracts.Common;

namespace Lalasia_store.Shared.Interfaces;

public interface IAuthService
{
    public Task<AuthTokensResponse> Signup(SignupRequest request);
    public Task<AuthTokensResponse> Login(LoginRequest request);
    public Task<AuthTokensResponse> Refresh(ClaimsPrincipal claimsPrincipal);
    public Task<DefaultResponse> Logout();
}
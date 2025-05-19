using System.Security.Claims;
using Lalasia_store.Controllers.Contracts.Auth;
using Lalasia_store.Controllers.Contracts.Common;

namespace Lalasia_store.Shared.Interfaces;

public interface IAuthService
{
    public Task<AuthResponse> Signup(SignupRequest request);
    public Task<AuthResponse> Login(LoginRequest request);
    public Task<AuthResponse> Refresh(ClaimsPrincipal claimsPrincipal);
    public Task<DefaultResponse> Logout();
    public Task<List<string>> GetUserRoles(ClaimsPrincipal claimsPrincipal);
}
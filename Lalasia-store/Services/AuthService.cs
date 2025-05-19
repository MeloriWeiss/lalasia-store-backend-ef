using System.Security.Claims;
using Lalasia_store.Controllers.Contracts.Auth;
using Lalasia_store.Controllers.Contracts.Common;
using Lalasia_store.Models.Data;
using Lalasia_store.Models.Types;
using Lalasia_store.Shared.Exceptions;
using Lalasia_store.Shared.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace Lalasia_store.Services;

public class AuthService : IAuthService
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;

    public AuthService(
        IServiceScopeFactory serviceScopeFactory,
        UserManager<User> userManager,
        SignInManager<User> signInManager)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _userManager = userManager;
        _signInManager = signInManager;
    }

    public async Task<AuthResponse> Signup(SignupRequest request)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var authTokensService = scope.ServiceProvider.GetRequiredService<IAuthTokensService>();

        var existingEmail = await _userManager.FindByEmailAsync(request.Email);
        if (existingEmail is not null)
            throw new BadRequestException("The email is already in use");

        var user = new User
        {
            UserName = request.Name, Email = request.Email, PasswordHash = request.Password,
            SecurityStamp = Guid.NewGuid().ToString()
        };

        var result = await _userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
            throw new BadRequestException("Failed to create an account");

        await _userManager.AddToRoleAsync(user, UserRoles.User.ToString());

        var roles = await _userManager.GetRolesAsync(user);
        
        var newAccessToken = authTokensService.GenerateAccessToken(user.Id, roles);
        var newRefreshToken = authTokensService.GenerateRefreshToken(user.Id, roles);

        return new AuthResponse()
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken
        };
    }

    public async Task<AuthResponse> Login(LoginRequest request)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var authTokensService = scope.ServiceProvider.GetRequiredService<IAuthTokensService>();

        var user = await _userManager.FindByEmailAsync(request.Email);

        if (user == null)
            throw new NotFoundException("The user was not found");

        var result = await _signInManager.PasswordSignInAsync(user, request.Password, true, false);

        if (!result.Succeeded)
            throw new BadRequestException("Incorrect email or password");

        var roles = await _userManager.GetRolesAsync(user);

        var newAccessToken = authTokensService.GenerateAccessToken(user.Id, roles);
        var newRefreshToken = authTokensService.GenerateRefreshToken(user.Id, roles);

        return new AuthResponse()
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken
        };
    }

    public async Task<AuthResponse> Refresh(ClaimsPrincipal claimsPrincipal)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var authTokensService = scope.ServiceProvider.GetRequiredService<IAuthTokensService>();

        var user = await _userManager.GetUserAsync(claimsPrincipal);

        if (user is null)
            throw new NotFoundException("The user was not found");

        var roles = await _userManager.GetRolesAsync(user);

        var newAccessToken = authTokensService.GenerateAccessToken(user.Id, roles);
        var newRefreshToken = authTokensService.GenerateRefreshToken(user.Id, roles);

        return new AuthResponse()
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken
        };
    }

    public async Task<DefaultResponse> Logout()
    {
        await _signInManager.SignOutAsync();

        return new DefaultResponse { Error = false, Message = "Successful logout" };
    }
    
    public async Task<List<string>> GetUserRoles(ClaimsPrincipal claimsPrincipal)
    {
        var user = await _userManager.GetUserAsync(claimsPrincipal);

        if (user is null)
            throw new NotFoundException("The user was not found");

        var roles = await _userManager.GetRolesAsync(user);

        return roles.ToList();
    }
}
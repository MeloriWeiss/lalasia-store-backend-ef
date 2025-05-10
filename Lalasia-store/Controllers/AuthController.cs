using Lalasia_store.Controllers.Contracts.Auth;
using Lalasia_store.Controllers.Contracts.Common;
using Lalasia_store.Models.Data;
using Lalasia_store.Shared.Exceptions;
using Lalasia_store.Shared.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Lalasia_store.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        IAuthService authService,
        ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> Signup([FromBody] SignupRequest request)
    {
        try
        {
            var authTokens = await _authService.Signup(request);

            return Ok(authTokens);
        }
        catch (BadRequestException badRequestException)
        {
            _logger.LogError(badRequestException, "[Signup] server error");
            return BadRequest(new DefaultResponse() { Error = true, Message = badRequestException.Message });
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "[Signup] server error");
            return BadRequest(new DefaultResponse() { Error = true, Message = "Failed to signup" });
        }
    }

    [HttpPost]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        try
        {
            var authTokens = await _authService.Login(request);

            return Ok(authTokens);
        }
        catch (NotFoundException notFoundException)
        {
            _logger.LogError(notFoundException, "[Login] server error");
            return NotFound(new DefaultResponse() { Error = true, Message = notFoundException.Message });
        }
        catch (BadRequestException badRequestException)
        {
            _logger.LogError(badRequestException, "[Login] server error");
            return BadRequest(new DefaultResponse() { Error = true, Message = badRequestException.Message });
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "[Login] server error");
            return BadRequest(new DefaultResponse() { Error = true, Message = "Failed to login" });
        }
    }

    [HttpPost]
    [Authorize(AuthenticationSchemes = "RefreshToken")]
    public async Task<IActionResult> Refresh()
    {
        try
        {
            var authTokens = await _authService.Refresh(User);

            return Ok(authTokens);
        }
        catch (NotFoundException notFoundException)
        {
            _logger.LogError(notFoundException, "[Refresh] server error");
            return NotFound(new DefaultResponse() { Error = true, Message = notFoundException.Message });
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "[Refresh] server error");
            return BadRequest(new DefaultResponse { Error = true, Message = "Failed to refresh tokens" });
        }
    }

    [HttpPost]
    [Authorize(AuthenticationSchemes = "AccessToken")]
    public async Task<IActionResult> Logout()
    {
        try
        {
            var result = await _authService.Logout();

            return Ok(result);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "[Logout] server error");
            return BadRequest(new DefaultResponse { Error = true, Message = "Failed to logout" });
        }
    }
}
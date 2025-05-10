using Lalasia_store.Controllers.Contracts.Common;
using Lalasia_store.Controllers.Contracts.User;
using Lalasia_store.Models;
using Lalasia_store.Models.Data;
using Lalasia_store.Shared.Exceptions;
using Lalasia_store.Shared.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Lalasia_store.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class UserController : ControllerBase
{
    private readonly ILogger<UserController> _logger;
    private readonly IUserService _userService;

    public UserController(
        IUserService userService,
        ILogger<UserController> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    [HttpGet]
    [Authorize(AuthenticationSchemes = "AccessToken")]
    public async Task<IActionResult> GetInfo()
    {
        try
        {
            var user = await _userService.GetInfo(User);

            return Ok(user);
        }
        catch (NotFoundException notFoundException)
        {
            _logger.LogError(notFoundException, "[UserGetInfo] server error");
            return NotFound(new DefaultResponse() { Error = true, Message = notFoundException.Message });
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "[UserGetInfo] server error");
            return BadRequest(new DefaultResponse() { Error = true, Message = "Couldn't get user information" });
        }
    }

    [HttpPatch]
    [Authorize(AuthenticationSchemes = "AccessToken")]
    public async Task<IActionResult> ChangeInfo([FromBody] ChangeUserInfoRequest request)
    {
        try
        {
            var result = await _userService.ChangeInfo(request, User);

            return Ok(result);
        }
        catch (NotFoundException notFoundException)
        {
            _logger.LogError(notFoundException, "[UserChangeInfo] server error");
            return NotFound(new DefaultResponse() { Error = true, Message = notFoundException.Message });
        }
        catch (BadRequestException badRequestException)
        {
            _logger.LogError(badRequestException, "[UserChangeInfo] server error");
            return BadRequest(new DefaultResponse() { Error = true, Message = badRequestException.Message });
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "[UserChangeInfo] server error");
            return BadRequest(new DefaultResponse() { Error = true, Message = "Не удалось изменить данные" });
        }
    }

    [HttpPatch]
    [Authorize(AuthenticationSchemes = "AccessToken")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
    {
        try
        {
            var result = await _userService.ChangePassword(request, User);
            return Ok(result);
        }
        catch (NotFoundException notFoundException)
        {
            _logger.LogError(notFoundException, "[UserChangePassword] server error");
            return NotFound(new DefaultResponse() { Error = true, Message = notFoundException.Message });
        }
        catch (BadRequestException badRequestException)
        {
            _logger.LogError(badRequestException, "[UserChangePassword] server error");
            return BadRequest(new DefaultResponse() { Error = true, Message = badRequestException.Message });
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "[UserChangePassword] server error");
            return BadRequest(new DefaultResponse()
                { Error = true, Message = "Не удалось изменить данные" });
        }
    }
}
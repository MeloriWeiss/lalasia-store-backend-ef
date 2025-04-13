using Lalasia_store.Contracts.Auth;
using Lalasia_store.Core.Services.Auth;
using Lalasia_store.Models;
using Lalasia_store.Models.Data;
using Lalasia_store.Models.Types;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Lalasia_store.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _dbContext;
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly RoleManager<Role> _roleManager;
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        AppDbContext dbContext,
        UserManager<User> userManager,
        SignInManager<User> signInManager,
        RoleManager<Role> roleManager,
        IAuthService authService,
        ILogger<AuthController> logger)
    {
        _dbContext = dbContext;
        _userManager = userManager;
        _signInManager = signInManager;
        _roleManager = roleManager;
        _authService = authService;
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        try
        {
            var user = new User { UserName = request.Name, Email = request.Email, PasswordHash = request.Password };
            // пытаемся создать пользователя с автоматическим хэшированием пароля
            var result = await _userManager.CreateAsync(user, request.Password);

            // возвращаем ошибку, если не удалось создать пользователя
            if (!result.Succeeded)
                return BadRequest(new { error = true, message = "Не удалось создать аккаунт" });

            await _userManager.AddToRoleAsync(user, UserRoles.User.ToString());

            // геренируем новые токены
            var newAccessToken = _authService.GenerateAccessToken(user.Id);
            var newRefreshToken = _authService.GenerateRefreshToken();

            // создаём рефреш-токен и сохраняем в базу данных
            var token = new RefreshToken
            {
                Token = newRefreshToken,
                UserId = user.Id,
                // устанавливаем время истечения
                ExpiresAt = DateTime.UtcNow.AddDays(7),
            };

            await _dbContext.RefreshTokens.AddAsync(token);
            await _dbContext.SaveChangesAsync();

            // возвращаем на клиент актуальные токены
            return Ok(new
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken
            });
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "[Register] server error");
            return BadRequest(new { error = true, message = "Не удалось зарегистрироваться" });
        }
    }

    [HttpPost]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        try
        {
            var user = await _userManager.FindByEmailAsync(request.Email);

            if (user == null)
                return NotFound(new { error = true, message = "Пользователь не найден" });

            var result = await _signInManager.PasswordSignInAsync(user, request.Password, true, false);

            if (!result.Succeeded)
                return Unauthorized(new { error = true, message = "Неправильный email или пароль" });

            // ищем рефреш-токен в базе данных по старому токену
            var existingRefreshToken =
                await _dbContext.RefreshTokens.FirstOrDefaultAsync(token => token.UserId == user.Id);

            var newAccessToken = _authService.GenerateAccessToken(user.Id);
            var newRefreshToken = _authService.GenerateRefreshToken();

            if (existingRefreshToken == null)
            {
                var token = new RefreshToken
                {
                    Token = newRefreshToken,
                    UserId = user.Id,
                    // устанавливаем время истечения
                    ExpiresAt = DateTime.UtcNow.AddDays(7),
                };
                await _dbContext.RefreshTokens.AddAsync(token);
            }
            else
            {
                existingRefreshToken.Token = newRefreshToken;
                existingRefreshToken.ExpiresAt = DateTime.UtcNow.AddDays(7);
            }

            await _dbContext.SaveChangesAsync();

            return Ok(new
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken
            });
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "[Login] server error");
            return BadRequest(new { error = true, message = "Не удалось войти" });
        }
    }

    [HttpPost]
    public async Task<IActionResult> Refresh([FromBody] RefreshRequest request)
    {
        try
        {
            // ищем рефреш-токен в базе данных по старому токену
            var existingRefreshToken =
                await _dbContext.RefreshTokens.FirstOrDefaultAsync(token => token.Token == request.RefreshToken);

            // проверяем токен на существование и валидность
            if (existingRefreshToken == null || existingRefreshToken.ExpiresAt <= DateTime.UtcNow)
                return Unauthorized(new { error = true, message = "Войдите в систему заново" });

            // получаем id пользователя
            var userId = existingRefreshToken.UserId;

            // геренируем новые токены
            var newAccessToken = _authService.GenerateAccessToken(userId);
            var newRefreshToken = _authService.GenerateRefreshToken();

            // обновляем рефреш-токен
            existingRefreshToken.Token = newRefreshToken;
            existingRefreshToken.ExpiresAt = DateTime.UtcNow.AddDays(7);

            await _dbContext.SaveChangesAsync();

            // возвращаем новые токены
            return Ok(new
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken
            });
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "[Refresh] server error");
            return BadRequest(new { error = true, message = "Не удалось проверить права доступа" });
        }
    }

    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        try
        {
            await _signInManager.SignOutAsync();

            return Ok(new { error = false, message = "Вы успешно вышли из системы" });
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "[Logout] server error");
            return BadRequest(new { error = true, message = "Ошибка выхода из системы" });
        }
    }
}
using System.Text;
using Lalasia_store.Core.Services.Auth;
using Lalasia_store.Models;
using Lalasia_store.Models.Data;
using Lalasia_store.Models.Types;
using Lalasia_store.Settings;
using Lalasia_store.Utils;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));

builder.Services.AddDbContext<AppDbContext>();
builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IAuthService, AuthService>();

builder.Services.AddHttpContextAccessor();
// настроим Identity и параметры пароля
builder.Services.AddIdentity<User, Role>(options =>
    {
        // настриваем разрешённые символы в имени пользователя
        options.User.AllowedUserNameCharacters =
            "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZабвгдезийклмнопрстуфхцчшщъыьэюяАБВГДЕЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯ";
        // настриваем пароль
        // обязательное наличие цифры
        options.Password.RequireDigit = false;
        // минимальное количество символов
        options.Password.RequiredLength = 8;
        // обязательное наличие строчных букв
        options.Password.RequireLowercase = false;
        // обязательное наличие заглавных букв
        options.Password.RequireUppercase = false;
        // минимальное количество уникальных символов
        options.Password.RequiredUniqueChars = 0;
        // обязательное наличие специальных символов
        options.Password.RequireNonAlphanumeric = false;
    })
    // метод для сохранения данных о пользователе в базе данных
    .AddEntityFrameworkStores<AppDbContext>()
    // метод для использования генерации токенов для подтверждения почты или пароля (например, при смене)
    .AddDefaultTokenProviders();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:3000");
        policy.AllowAnyHeader();
        policy.AllowAnyMethod();
    });
});

var key = Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"] ?? string.Empty);
builder.Services
    .AddAuthentication(options =>
    {
        // добавляем схемы (методы, инструкции, по которым будет проходить авторизация)
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        // обязательное использование https для получания метаданных об аутентификации
        options.RequireHttpsMetadata = false;
        // сохраняем токен, чтобы каждый раз не запрашивать новый и иметь возможность получить его из HttpContext
        options.SaveToken = true;
        // добавляем параметры токену
        options.TokenValidationParameters = new TokenValidationParameters
        {
            // валидация по секретному ключу токена
            ValidateIssuerSigningKey = true,
            // указываем на секретный ключ, передавая его в байтах
            IssuerSigningKey = new SymmetricSecurityKey(key),
            // проверка издателя токена (например, сервера)
            ValidateIssuer = false,
            // проверка аудитории, сервисов, для которых предназначен токен
            ValidateAudience = false,
            // проверка на срок годности токена
            ValidateLifetime = true
        };
    });

builder.Services.AddAuthorizationBuilder()
    // добавляем роль, которую можно будет использовать в атрибуте [Authorize] для разрешения получения ответов только ползователям с данной ролью
    .AddPolicy("AdminRole", policy => policy.RequireRole(UserRoles.Admin.ToString()));

var app = builder.Build();

// выполнение сидинга (заполнение базы данных для удобной работы)
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var dbContext = services.GetRequiredService<AppDbContext>();
        var roleManager = services.GetRequiredService<RoleManager<Role>>();

        await DbSeed.Seed(dbContext, roleManager);
    }
    catch (Exception exception)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogWarning(exception, "Ошибка сидинга");
    }
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();
app.UseHttpsRedirection();

// добавляем middleware для авторизации и аутентификации
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();
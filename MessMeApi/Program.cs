using Microsoft.EntityFrameworkCore;
using AutoMapper;
using MessMeApi.DbContexts;
using MessMeApi.Repositories.Implementations;
using MessMeApi.Repositories.Interfaces;
using MessMeApi.Services;
using MessMeApi.Mappers;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]);

// ✅ JWT Authentication Setup
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(key)
        };
    });

// ✅ Database Connection
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

// ✅ Dependency Injection
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddAutoMapper(typeof(MappingProfile));
builder.Services.AddControllers();
builder.Services.AddAuthorization();

// ✅ CORS Configuration (Fixing AllowedOrigins)
var allowedOrigins = builder.Configuration["AllowedOrigins"]?.Split(",") ?? Array.Empty<string>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy => policy.WithOrigins(allowedOrigins)
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials()
    );
});

var app = builder.Build();

// ✅ Enable CORS
app.UseCors("AllowFrontend");

// ✅ Configure Middleware
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();

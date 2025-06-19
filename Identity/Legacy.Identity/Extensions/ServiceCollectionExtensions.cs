using System.Text;
using Legacy.Identity.Database;
using Legacy.Identity.Interfaces;
using Legacy.Identity.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace Legacy.Identity.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddSwaggerAndJwtHandling(this IServiceCollection services, IConfiguration configuration)
    {
        // Identity uzywa wlasnej bazy danych (IdentityDatabaseContext)
        services.AddIdentity<IdentityUser, IdentityRole>()
            .AddEntityFrameworkStores<IdentityDatabaseContext>()
            .AddDefaultTokenProviders();

        services.AddSwagger(configuration);
        services.AddJwtHandling(configuration);
        services.AddScoped<ITokenService, TokenService>();
    }

    private static void AddJwtHandling(this IServiceCollection services, IConfiguration configuration)
    {
        // Walidacja konfiguracji JWT
        var jwtSettings = configuration.GetSection("JwtSettings");

        if (string.IsNullOrEmpty(jwtSettings["Key"]))
        {
            throw new ArgumentException("JWT Key must be configured in JwtSettings");
        }

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwtSettings["Issuer"],
                    ValidateAudience = true,
                    ValidAudience = jwtSettings["Audience"],
                    ValidateLifetime = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtSettings["Key"] ?? throw new InvalidOperationException("JWT Key is missing."))
                    ),

                    ValidateIssuerSigningKey = true,
                };
            });
    }

    private static void AddSwagger(this IServiceCollection services, IConfiguration configuration)
    {
        var swaggerSettings = configuration.GetSection("SwaggerSettings");
        var apiName = swaggerSettings["ApiName"] ?? throw new Exception("ApiName must be configured in SwaggerSettings");
        var version = swaggerSettings["Version"] ?? throw new Exception("Version must be configured in SwaggerSettings");
        
        services.AddSwaggerGen(s =>
        {
            s.SwaggerDoc($"v1", new OpenApiInfo { Title = $"{apiName} v{version}", Version = $"{version}" });
            s.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Wprowadź JWT token w formacie: Bearer {{token}}"
            });


            s.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });

            s.EnableAnnotations();
        });
    }
}
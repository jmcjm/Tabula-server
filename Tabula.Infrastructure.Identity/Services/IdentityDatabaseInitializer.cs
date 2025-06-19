using System.Diagnostics;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Tabula.Infrastructure.Identity.Database;
using Tabula.Infrastructure.Identity.Interfaces;
using Tabula.Infrastructure.Identity.Models;

namespace Tabula.Infrastructure.Identity.Services;

public class IdentityDatabaseInitializer(
    IdentityDatabaseContext context,
    ILogger<IdentityDatabaseInitializer> logger,
    ITokenService tokenService,
    IConfiguration configuration) : IIdentityDatabaseInitializer
{
    public async Task InitializeAsync()
    {
        try
        {
            logger.LogInformation("Applying migrations...");
            await context.Database.MigrateAsync();
            
            if (!tokenService.DoesUserExistAsync("admin").Result)
            {
                tokenService.CreateRoleAsync("admin").Wait();
                tokenService.CreateRoleAsync("user").Wait();

                // Get admin credentials from configuration (environment variables or user secrets)
                var adminPassword = configuration["ADMIN_PASSWORD"] ?? 
                                   configuration["AdminSettings:Password"];
                
                if (string.IsNullOrEmpty(adminPassword))
                {
                    logger.LogCritical("ADMIN_PASSWORD environment variable is not set. Admin user will not be created.");
                    logger.LogCritical("Please set ADMIN_PASSWORD environment variable or AdminSettings:Password in user secrets.");
                    throw new InvalidOperationException("Admin password must be configured via environment variables or user secrets. Never hardcode or auto-generate admin passwords.");
                }

                // NEW: Validate admin password strength
                if (!IsPasswordSecure(adminPassword))
                {
                    logger.LogCritical("Admin password does not meet security requirements. Password must be at least 12 characters long and contain uppercase, lowercase, numbers, and special characters.");
                    throw new InvalidOperationException("Admin password does not meet security requirements.");
                }

                var adminUsername = configuration["ADMIN_USERNAME"] ?? "admin";
                var adminEmail = configuration["ADMIN_EMAIL"] ?? "admin@example.com";
                
                var adminUser = new RegisterModel(adminUsername, adminEmail, adminPassword,
                    new List<string> { "admin" });
                var result = await tokenService.CreateUserAsync(adminUser);
                
                if (result.Succeeded)
                {
                    logger.LogInformation("Admin user '{AdminUsername}' created successfully", adminUsername);
                }
                else
                {
                    logger.LogError("Failed to create admin user: {Errors}", 
                        string.Join(", ", result.Errors.Select(e => e.Description)));
                    throw new InvalidOperationException("Failed to create admin user. Check password policy requirements.");
                }
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while initializing the database");
            throw;
        }
    }

    private string GenerateSecureRandomString(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789@#$%^&*()_+=-!";
        using var rng = RandomNumberGenerator.Create();
        return new string(Enumerable.Range(0, length)
            .Select(_ =>
            {
                var buffer = new byte[1];
                rng.GetBytes(buffer);
                return chars[buffer[0] % chars.Length];
            }).ToArray());
    }

    private bool IsPasswordSecure(string password)
    {
        // Password security requirements:
        // - At least 12 characters
        // - At least one uppercase letter
        // - At least one lowercase letter
        // - At least one digit
        // - At least one special character
        return password.Length >= 12 &&
               password.Any(char.IsUpper) &&
               password.Any(char.IsLower) &&
               password.Any(char.IsDigit) &&
               password.Any(c => "!@#$%^&*()_+-=[]{}|;:,.<>?".Contains(c));
    }
}
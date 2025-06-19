namespace Tabula.Infrastructure.Identity.Interfaces;

public interface IIdentityDatabaseChecker
{
    Task<bool> CheckConnectionAsync();
}
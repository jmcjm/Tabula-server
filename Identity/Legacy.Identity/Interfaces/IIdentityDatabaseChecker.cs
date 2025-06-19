namespace Legacy.Identity.Interfaces;

public interface IIdentityDatabaseChecker
{
    Task<bool> CheckConnectionAsync();
}
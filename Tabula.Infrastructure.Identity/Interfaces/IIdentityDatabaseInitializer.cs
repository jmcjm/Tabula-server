namespace Tabula.Infrastructure.Identity.Interfaces;

public interface IIdentityDatabaseInitializer
{
    Task InitializeAsync();
}
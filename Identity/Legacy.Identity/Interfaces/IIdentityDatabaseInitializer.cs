namespace Legacy.Identity.Interfaces;

public interface IIdentityDatabaseInitializer
{
    Task InitializeAsync();
}
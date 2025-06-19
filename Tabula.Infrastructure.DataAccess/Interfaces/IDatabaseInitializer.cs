namespace Tabula.Infrastructure.DataAccess.Interfaces;

public interface IDatabaseInitializer
{
    Task InitializeAsync();
}
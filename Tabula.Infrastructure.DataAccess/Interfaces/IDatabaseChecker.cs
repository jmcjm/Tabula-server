namespace Tabula.Infrastructure.DataAccess.Interfaces;

public interface IDatabaseChecker
{
    Task<bool> CheckConnectionAsync();
}
namespace Domain.Interfaces;

public interface IDatabaseService
{
    /// <summary>
    /// Checks if the database connection is successful.
    /// </summary>
    Task<bool> CheckConnectionAsync();
    
    /// <summary>
    /// Checks migrations and applies them if necessary.
    /// </summary>
    Task InitializeAsync();
}
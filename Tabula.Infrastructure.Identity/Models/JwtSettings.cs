namespace Tabula.Infrastructure.Identity.Models;

/// <summary>
/// Przechowuje konfigurację ustawień JWT.
/// </summary>
public class JwtSettings
{
    /// <summary>
    /// Klucz używany do podpisywania tokenów JWT.
    /// </summary>
    public string Key { get; set; } = string.Empty;

    /// <summary>
    /// Wydawca (issuer) tokenów JWT.
    /// </summary>
    public string Issuer { get; set; } = string.Empty;

    /// <summary>
    /// Odbiorca (audience) tokenów JWT.
    /// </summary>
    public string Audience { get; set; } = string.Empty;
}
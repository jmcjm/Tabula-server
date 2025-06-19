using System.Text.Json.Serialization;

namespace Legacy.Identity.Models;

/// <summary>
/// Reprezentuje odpowiedź zawierającą token JWT.
/// </summary>
public class TokenResponse(string token)
{
    /// <summary>
    /// Token JWT zwrócony po udanym uwierzytelnieniu.
    /// </summary>
    [JsonPropertyName("token")]
    public string Token { get; set; } = token;
    
    // [JsonPropertyName("id_token")]
    // public string IdToken { get; set; } = string.Empty;
    //
    // [JsonPropertyName("expires_in")]
    // public int ExpiresIn { get; set; }
    //
    // [JsonPropertyName("token_type")]
    // public string TokenType { get; set; } = "Bearer";
}
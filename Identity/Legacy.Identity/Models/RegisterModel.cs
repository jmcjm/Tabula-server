namespace Legacy.Identity.Models;

public record RegisterModel(string Username, string Email, string Password, IList<string> Roles);
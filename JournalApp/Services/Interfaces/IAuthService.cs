namespace JournalApp.Services.Interfaces;

/// <summary>
/// Service interface for authentication and security
/// </summary>
public interface IAuthService
{
    Task<bool> LoginAsync(string username, string password);
    Task LogoutAsync();
    bool IsAuthenticated { get; }
    int? CurrentUserId { get; }
    string? CurrentUsername { get; }
}

using JournalApp.Services.Interfaces;
using Microsoft.Extensions.Logging;
using BCrypt.Net;
using Microsoft.Maui.Storage;

namespace JournalApp.Services;

/// <summary>
/// Service implementation for authentication and security
/// </summary>
public class AuthService : IAuthService
{
    private readonly IUserService _userService;
    private readonly ILogger<AuthService> _logger;
    private const string UserIdKey = "JournalApp_UserId";
    private const string UsernameKey = "JournalApp_Username";

    private int? _currentUserId;
    private string? _currentUsername;

    public AuthService(IUserService userService, ILogger<AuthService> logger)
    {
        _userService = userService;
        _logger = logger;
        LoadStoredAuth();
    }

    public bool IsAuthenticated => _currentUserId.HasValue;

    public int? CurrentUserId => _currentUserId;

    public string? CurrentUsername => _currentUsername;

    public async Task<bool> LoginAsync(string username, string password)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                return false;

            var isValid = await _userService.ValidatePasswordAsync(username, password);
            if (!isValid)
                return false;

            var user = await _userService.GetUserByUsernameAsync(username);
            if (user == null)
                return false;

            _currentUserId = user.UserId;
            _currentUsername = user.Username;

            // Store authentication
            Preferences.Set(UserIdKey, user.UserId.ToString());
            Preferences.Set(UsernameKey, user.Username);

            _logger.LogInformation("User logged in: {Username}", username);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login");
            return false;
        }
    }

    public Task LogoutAsync()
    {
        try
        {
            _currentUserId = null;
            _currentUsername = null;

            Preferences.Remove(UserIdKey);
            Preferences.Remove(UsernameKey);

            _logger.LogInformation("User logged out");
            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during logout");
            return Task.CompletedTask;
        }
    }

    private void LoadStoredAuth()
    {
        try
        {
            var userIdStr = Preferences.Get(UserIdKey, string.Empty);
            if (!string.IsNullOrEmpty(userIdStr) && int.TryParse(userIdStr, out var userId))
            {
                _currentUserId = userId;
                _currentUsername = Preferences.Get(UsernameKey, string.Empty);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading stored authentication");
        }
    }
}

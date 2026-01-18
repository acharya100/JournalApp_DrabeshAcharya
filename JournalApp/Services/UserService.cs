using JournalApp.Data;
using JournalApp.Models;
using JournalApp.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using BCrypt.Net;

namespace JournalApp.Services;

/// <summary>
/// Service implementation for user management
/// </summary>
public class UserService : IUserService
{
    private readonly JournalDbContext _context;
    private readonly ILogger<UserService> _logger;

    public UserService(JournalDbContext context, ILogger<UserService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<User?> GetUserByIdAsync(int userId)
    {
        try
        {
            return await _context.Users.FindAsync(userId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user by ID: {UserId}", userId);
            throw;
        }
    }

    public async Task<User?> GetUserByUsernameAsync(string username)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentException("Username cannot be null or empty", nameof(username));

            return await _context.Users
                .FirstOrDefaultAsync(u => u.Username == username);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user by username: {Username}", username);
            throw;
        }
    }

    public async Task<User> CreateUserAsync(string username, string password)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentException("Username cannot be null or empty", nameof(username));
            
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Password cannot be null or empty", nameof(password));

            if (await UserExistsAsync(username))
                throw new InvalidOperationException($"User with username '{username}' already exists");

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password, workFactor: 12);

            var user = new User
            {
                Username = username,
                Password = hashedPassword,
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            _logger.LogInformation("User created successfully: {Username}", username);
            return user;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating user: {Username}", username);
            throw;
        }
    }

    public async Task<bool> ValidatePasswordAsync(string username, string password)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                return false;

            var user = await GetUserByUsernameAsync(username);
            if (user == null)
                return false;

            return BCrypt.Net.BCrypt.Verify(password, user.Password);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating password for user: {Username}", username);
            return false;
        }
    }

    public async Task<bool> ChangePasswordAsync(int userId, string oldPassword, string newPassword)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(newPassword))
                throw new ArgumentException("New password cannot be null or empty", nameof(newPassword));

            var user = await GetUserByIdAsync(userId);
            if (user == null)
                throw new InvalidOperationException("User not found");

            if (!BCrypt.Net.BCrypt.Verify(oldPassword, user.Password))
                return false;

            user.Password = BCrypt.Net.BCrypt.HashPassword(newPassword, workFactor: 12);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Password changed successfully for user: {UserId}", userId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error changing password for user: {UserId}", userId);
            throw;
        }
    }

    public async Task<bool> UserExistsAsync(string username)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(username))
                return false;

            return await _context.Users.AnyAsync(u => u.Username == username);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if user exists: {Username}", username);
            throw;
        }
    }
}

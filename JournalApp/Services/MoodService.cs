using JournalApp.Data;
using JournalApp.Models;
using JournalApp.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace JournalApp.Services;

/// <summary>
/// Service implementation for mood management
/// </summary>
public class MoodService : IMoodService
{
    private readonly JournalDbContext _context;
    private readonly ILogger<MoodService> _logger;

    public MoodService(JournalDbContext context, ILogger<MoodService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<List<Mood>> GetAllMoodsAsync()
    {
        try
        {
            return await _context.Moods
                .OrderBy(m => m.Category)
                .ThenBy(m => m.MoodName)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all moods");
            throw;
        }
    }

    public async Task<List<Mood>> GetMoodsByCategoryAsync(string category)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(category))
                return await GetAllMoodsAsync();

            return await _context.Moods
                .Where(m => m.Category == category)
                .OrderBy(m => m.MoodName)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting moods by category: {Category}", category);
            throw;
        }
    }

    public async Task<Mood?> GetMoodByIdAsync(int moodId)
    {
        try
        {
            return await _context.Moods.FindAsync(moodId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting mood by ID: {MoodId}", moodId);
            throw;
        }
    }
}

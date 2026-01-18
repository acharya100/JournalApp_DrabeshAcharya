using JournalApp.Models;

namespace JournalApp.Services.Interfaces;

/// <summary>
/// Service interface for mood management
/// </summary>
public interface IMoodService
{
    Task<List<Mood>> GetAllMoodsAsync();
    Task<List<Mood>> GetMoodsByCategoryAsync(string category);
    Task<Mood?> GetMoodByIdAsync(int moodId);
}

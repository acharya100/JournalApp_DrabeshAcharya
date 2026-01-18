using JournalApp.Models;

namespace JournalApp.Services.Interfaces;

/// <summary>
/// Service interface for tag management
/// </summary>
public interface ITagService
{
    Task<List<Tag>> GetAllTagsAsync();
    Task<List<Tag>> GetPredefinedTagsAsync();
    Task<Tag> CreateTagAsync(string tagName);
    Task<Tag?> GetTagByIdAsync(int tagId);
    Task<Tag?> GetTagByNameAsync(string tagName);
}

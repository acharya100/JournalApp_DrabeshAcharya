using JournalApp.Models;

namespace JournalApp.Services.Interfaces;

/// <summary>
/// Service interface for journal entry management
/// </summary>
public interface IJournalService
{
    Task<JournalEntry?> GetEntryByIdAsync(int entryId);
    Task<JournalEntry?> GetEntryByDateAsync(int userId, DateTime date);
    Task<List<JournalEntry>> GetEntriesAsync(int userId, int pageNumber = 1, int pageSize = 10);
    Task<List<JournalEntry>> SearchEntriesAsync(int userId, string searchTerm);
    Task<List<JournalEntry>> FilterEntriesAsync(int userId, DateTime? startDate, DateTime? endDate, 
        List<int>? moodIds, List<int>? tagIds);
    Task<JournalEntry> CreateEntryAsync(int userId, DateTime entryDate, string title, 
        string content, int primaryMoodId, List<int>? secondaryMoodIds = null, 
        List<int>? tagIds = null);
    Task<JournalEntry> UpdateEntryAsync(int entryId, string title, string content, 
        int primaryMoodId, List<int>? secondaryMoodIds = null, List<int>? tagIds = null);
    Task<bool> DeleteEntryAsync(int entryId);
    Task<bool> EntryExistsForDateAsync(int userId, DateTime date);
    Task<int> GetTotalEntriesCountAsync(int userId);
}

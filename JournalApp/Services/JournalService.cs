using JournalApp.Data;
using JournalApp.Models;
using JournalApp.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace JournalApp.Services;

/// <summary>
/// Service implementation for journal entry management
/// </summary>
public class JournalService : IJournalService
{
    private readonly JournalDbContext _context;
    private readonly ILogger<JournalService> _logger;

    public JournalService(JournalDbContext context, ILogger<JournalService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<JournalEntry?> GetEntryByIdAsync(int entryId)
    {
        try
        {
            return await _context.JournalEntries
                .Include(e => e.PrimaryMood)
                .Include(e => e.SecondaryMoods)
                    .ThenInclude(sm => sm.Mood)
                .Include(e => e.Tags)
                    .ThenInclude(t => t.Tag)
                .FirstOrDefaultAsync(e => e.EntryId == entryId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting entry by ID: {EntryId}", entryId);
            throw;
        }
    }

    public async Task<JournalEntry?> GetEntryByDateAsync(int userId, DateTime date)
    {
        try
        {
            var dateOnly = date.Date;
            return await _context.JournalEntries
                .Include(e => e.PrimaryMood)
                .Include(e => e.SecondaryMoods)
                    .ThenInclude(sm => sm.Mood)
                .Include(e => e.Tags)
                    .ThenInclude(t => t.Tag)
                .FirstOrDefaultAsync(e => e.UserId == userId && e.EntryDate.Date == dateOnly);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting entry by date: {Date} for user: {UserId}", date, userId);
            throw;
        }
    }

    public async Task<List<JournalEntry>> GetEntriesAsync(int userId, int pageNumber = 1, int pageSize = 10)
    {
        try
        {
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 10;

            return await _context.JournalEntries
                .Where(e => e.UserId == userId)
                .Include(e => e.PrimaryMood)
                .Include(e => e.SecondaryMoods)
                    .ThenInclude(sm => sm.Mood)
                .Include(e => e.Tags)
                    .ThenInclude(t => t.Tag)
                .OrderByDescending(e => e.EntryDate)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting entries for user: {UserId}", userId);
            throw;
        }
    }

    public async Task<List<JournalEntry>> SearchEntriesAsync(int userId, string searchTerm)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return await GetEntriesAsync(userId, 1, 100);

            var term = searchTerm.ToLower();
            return await _context.JournalEntries
                .Where(e => e.UserId == userId &&
                    (e.JournalTitle.ToLower().Contains(term) || e.Content.ToLower().Contains(term)))
                .Include(e => e.PrimaryMood)
                .Include(e => e.SecondaryMoods)
                    .ThenInclude(sm => sm.Mood)
                .Include(e => e.Tags)
                    .ThenInclude(t => t.Tag)
                .OrderByDescending(e => e.EntryDate)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching entries for user: {UserId}", userId);
            throw;
        }
    }

    public async Task<List<JournalEntry>> FilterEntriesAsync(int userId, DateTime? startDate, DateTime? endDate,
        List<int>? moodIds, List<int>? tagIds)
    {
        try
        {
            var query = _context.JournalEntries
                .Where(e => e.UserId == userId)
                .AsQueryable();

            if (startDate.HasValue)
                query = query.Where(e => e.EntryDate >= startDate.Value.Date);

            if (endDate.HasValue)
                query = query.Where(e => e.EntryDate <= endDate.Value.Date);

            if (moodIds != null && moodIds.Any())
            {
                query = query.Where(e => 
                    moodIds.Contains(e.PrimaryMoodId) ||
                    e.SecondaryMoods.Any(sm => moodIds.Contains(sm.MoodId)));
            }

            if (tagIds != null && tagIds.Any())
            {
                query = query.Where(e => e.Tags.Any(t => tagIds.Contains(t.TagId)));
            }

            return await query
                .Include(e => e.PrimaryMood)
                .Include(e => e.SecondaryMoods)
                    .ThenInclude(sm => sm.Mood)
                .Include(e => e.Tags)
                    .ThenInclude(t => t.Tag)
                .OrderByDescending(e => e.EntryDate)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error filtering entries for user: {UserId}", userId);
            throw;
        }
    }

    public async Task<JournalEntry> CreateEntryAsync(int userId, DateTime entryDate, string title,
        string content, int primaryMoodId, List<int>? secondaryMoodIds = null,
        List<int>? tagIds = null)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Title cannot be null or empty", nameof(title));

            if (await EntryExistsForDateAsync(userId, entryDate.Date))
                throw new InvalidOperationException($"Entry already exists for date: {entryDate.Date:yyyy-MM-dd}");

            // Validate secondary moods (max 2)
            if (secondaryMoodIds != null && secondaryMoodIds.Count > 2)
                throw new ArgumentException("Maximum 2 secondary moods allowed", nameof(secondaryMoodIds));

            var entry = new JournalEntry
            {
                UserId = userId,
                EntryDate = entryDate.Date,
                JournalTitle = title,
                Content = content ?? string.Empty,
                PrimaryMoodId = primaryMoodId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.JournalEntries.Add(entry);
            await _context.SaveChangesAsync();

            // Add secondary moods
            if (secondaryMoodIds != null && secondaryMoodIds.Any())
            {
                foreach (var moodId in secondaryMoodIds)
                {
                    entry.SecondaryMoods.Add(new JournalEntryMood
                    {
                        EntryId = entry.EntryId,
                        MoodId = moodId
                    });
                }
            }

            // Add tags
            if (tagIds != null && tagIds.Any())
            {
                foreach (var tagId in tagIds)
                {
                    entry.Tags.Add(new JournalEntryTag
                    {
                        EntryId = entry.EntryId,
                        TagId = tagId
                    });
                }
            }

            await _context.SaveChangesAsync();

            _logger.LogInformation("Entry created successfully: {EntryId} for user: {UserId}", entry.EntryId, userId);
            return await GetEntryByIdAsync(entry.EntryId) ?? entry;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating entry for user: {UserId}", userId);
            throw;
        }
    }

    public async Task<JournalEntry> UpdateEntryAsync(int entryId, string title, string content,
        int primaryMoodId, List<int>? secondaryMoodIds = null, List<int>? tagIds = null)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Title cannot be null or empty", nameof(title));

            var entry = await GetEntryByIdAsync(entryId);
            if (entry == null)
                throw new InvalidOperationException($"Entry not found: {entryId}");

            // Validate secondary moods (max 2)
            if (secondaryMoodIds != null && secondaryMoodIds.Count > 2)
                throw new ArgumentException("Maximum 2 secondary moods allowed", nameof(secondaryMoodIds));

            entry.JournalTitle = title;
            entry.Content = content ?? string.Empty;
            entry.PrimaryMoodId = primaryMoodId;
            entry.UpdatedAt = DateTime.UtcNow;

            // Remove existing secondary moods
            var existingSecondaryMoods = _context.JournalEntryMoods
                .Where(sm => sm.EntryId == entryId)
                .ToList();
            _context.JournalEntryMoods.RemoveRange(existingSecondaryMoods);

            // Add new secondary moods
            if (secondaryMoodIds != null && secondaryMoodIds.Any())
            {
                foreach (var moodId in secondaryMoodIds)
                {
                    entry.SecondaryMoods.Add(new JournalEntryMood
                    {
                        EntryId = entryId,
                        MoodId = moodId
                    });
                }
            }

            // Remove existing tags
            var existingTags = _context.JournalEntryTags
                .Where(t => t.EntryId == entryId)
                .ToList();
            _context.JournalEntryTags.RemoveRange(existingTags);

            // Add new tags
            if (tagIds != null && tagIds.Any())
            {
                foreach (var tagId in tagIds)
                {
                    entry.Tags.Add(new JournalEntryTag
                    {
                        EntryId = entryId,
                        TagId = tagId
                    });
                }
            }

            await _context.SaveChangesAsync();

            _logger.LogInformation("Entry updated successfully: {EntryId}", entryId);
            return await GetEntryByIdAsync(entryId) ?? entry;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating entry: {EntryId}", entryId);
            throw;
        }
    }

    public async Task<bool> DeleteEntryAsync(int entryId)
    {
        try
        {
            var entry = await GetEntryByIdAsync(entryId);
            if (entry == null)
                return false;

            _context.JournalEntries.Remove(entry);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Entry deleted successfully: {EntryId}", entryId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting entry: {EntryId}", entryId);
            throw;
        }
    }

    public async Task<bool> EntryExistsForDateAsync(int userId, DateTime date)
    {
        try
        {
            var dateOnly = date.Date;
            return await _context.JournalEntries
                .AnyAsync(e => e.UserId == userId && e.EntryDate.Date == dateOnly);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if entry exists for date: {Date}", date);
            throw;
        }
    }

    public async Task<int> GetTotalEntriesCountAsync(int userId)
    {
        try
        {
            return await _context.JournalEntries
                .CountAsync(e => e.UserId == userId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting total entries count for user: {UserId}", userId);
            throw;
        }
    }
}

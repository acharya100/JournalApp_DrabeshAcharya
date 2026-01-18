using JournalApp.Data;
using JournalApp.Models;
using JournalApp.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace JournalApp.Services;

/// <summary>
/// Service implementation for analytics and dashboard data
/// </summary>
public class AnalyticsService : IAnalyticsService
{
    private readonly JournalDbContext _context;
    private readonly ILogger<AnalyticsService> _logger;

    public AnalyticsService(JournalDbContext context, ILogger<AnalyticsService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<DashboardAnalytics> GetDashboardAnalyticsAsync(int userId, DateTime? startDate = null, DateTime? endDate = null)
    {
        try
        {
            var analytics = new DashboardAnalytics
            {
                MoodDistributionByCategory = await GetMoodDistributionAsync(userId, startDate, endDate),
                MostUsedTags = await GetMostUsedTagsAsync(userId, 10, startDate, endDate),
                WordCountTrends = await GetWordCountTrendsAsync(userId, startDate, endDate),
                StreakInfo = await GetStreakInfoAsync(userId)
            };

            // Get most frequent mood
            var moodCounts = await _context.JournalEntries
                .Where(e => e.UserId == userId)
                .Where(e => !startDate.HasValue || e.EntryDate >= startDate.Value)
                .Where(e => !endDate.HasValue || e.EntryDate <= endDate.Value)
                .Where(e => e.PrimaryMood != null)
                .GroupBy(e => e.PrimaryMood!.MoodName)
                .Select(g => new { MoodName = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .FirstOrDefaultAsync();

            analytics.MostFrequentMood = moodCounts?.MoodName;

            // Get total entries
            analytics.TotalEntries = await _context.JournalEntries
                .Where(e => e.UserId == userId)
                .Where(e => !startDate.HasValue || e.EntryDate >= startDate.Value)
                .Where(e => !endDate.HasValue || e.EntryDate <= endDate.Value)
                .CountAsync();

            // Get first and last entry dates
            var firstEntry = await _context.JournalEntries
                .Where(e => e.UserId == userId)
                .OrderBy(e => e.EntryDate)
                .FirstOrDefaultAsync();

            var lastEntry = await _context.JournalEntries
                .Where(e => e.UserId == userId)
                .OrderByDescending(e => e.EntryDate)
                .FirstOrDefaultAsync();

            analytics.FirstEntryDate = firstEntry?.EntryDate;
            analytics.LastEntryDate = lastEntry?.EntryDate;

            // Tag breakdown
            analytics.TagBreakdown = await GetMostUsedTagsAsync(userId, 20, startDate, endDate);

            return analytics;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting dashboard analytics for user: {UserId}", userId);
            throw;
        }
    }

    public async Task<StreakInfo> GetStreakInfoAsync(int userId)
    {
        try
        {
            var entries = await _context.JournalEntries
                .Where(e => e.UserId == userId)
                .OrderByDescending(e => e.EntryDate)
                .Select(e => e.EntryDate.Date)
                .Distinct()
                .ToListAsync();

            if (!entries.Any())
                return new StreakInfo { CurrentStreak = 0, LongestStreak = 0 };

            var today = DateTime.UtcNow.Date;
            var currentStreak = 0;
            var longestStreak = 0;
            var tempStreak = 0;
            var missedDays = new List<DateTime>();

            // Calculate current streak
            var checkDate = today;
            while (entries.Contains(checkDate))
            {
                currentStreak++;
                checkDate = checkDate.AddDays(-1);
            }

            // Calculate longest streak and missed days
            if (entries.Any())
            {
                var sortedEntries = entries.OrderBy(e => e).ToList();
                var startDate = sortedEntries.First();
                var endDate = sortedEntries.Last();

                for (var date = startDate; date <= today; date = date.AddDays(1))
                {
                    if (sortedEntries.Contains(date))
                    {
                        tempStreak++;
                        longestStreak = Math.Max(longestStreak, tempStreak);
                    }
                    else
                    {
                        if (date < today) // Don't count today if no entry yet
                            missedDays.Add(date);
                        tempStreak = 0;
                    }
                }
            }

            return new StreakInfo
            {
                CurrentStreak = currentStreak,
                LongestStreak = longestStreak,
                MissedDays = missedDays
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting streak info for user: {UserId}", userId);
            throw;
        }
    }

    public async Task<List<MoodDistribution>> GetMoodDistributionAsync(int userId, DateTime? startDate = null, DateTime? endDate = null)
    {
        try
        {
            var query = _context.JournalEntries
                .Where(e => e.UserId == userId)
                .AsQueryable();

            if (startDate.HasValue)
                query = query.Where(e => e.EntryDate >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(e => e.EntryDate <= endDate.Value);

            var totalEntries = await query.CountAsync();

            if (totalEntries == 0)
                return new List<MoodDistribution>();

            var distribution = await query
                .Where(e => e.PrimaryMood != null)
                .GroupBy(e => e.PrimaryMood!.Category)
                .Select(g => new MoodDistribution
                {
                    Category = g.Key,
                    Count = g.Count(),
                    Percentage = (double)g.Count() / totalEntries * 100
                })
                .OrderByDescending(d => d.Count)
                .ToListAsync();

            return distribution;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting mood distribution for user: {UserId}", userId);
            throw;
        }
    }

    public async Task<List<TagUsage>> GetMostUsedTagsAsync(int userId, int count = 10, DateTime? startDate = null, DateTime? endDate = null)
    {
        try
        {
            var query = _context.JournalEntryTags
                .Where(t => t.JournalEntry != null && t.JournalEntry.UserId == userId)
                .AsQueryable();

            if (startDate.HasValue)
                query = query.Where(t => t.JournalEntry != null && t.JournalEntry.EntryDate >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(t => t.JournalEntry != null && t.JournalEntry.EntryDate <= endDate.Value);

            var totalTagUsages = await query.CountAsync();

            if (totalTagUsages == 0)
                return new List<TagUsage>();

            var tagUsages = await query
                .Where(t => t.Tag != null)
                .GroupBy(t => t.Tag!.TagName)
                .Select(g => new TagUsage
                {
                    TagName = g.Key,
                    Count = g.Count(),
                    Percentage = (double)g.Count() / totalTagUsages * 100
                })
                .OrderByDescending(t => t.Count)
                .Take(count)
                .ToListAsync();

            return tagUsages;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting most used tags for user: {UserId}", userId);
            throw;
        }
    }

    public async Task<List<WordCountTrend>> GetWordCountTrendsAsync(int userId, DateTime? startDate = null, DateTime? endDate = null)
    {
        try
        {
            var query = _context.JournalEntries
                .Where(e => e.UserId == userId)
                .AsQueryable();

            if (startDate.HasValue)
                query = query.Where(e => e.EntryDate >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(e => e.EntryDate <= endDate.Value);

            var entries = await query
                .OrderBy(e => e.EntryDate)
                .Select(e => new { e.EntryDate, e.Content })
                .ToListAsync();

            if (!entries.Any())
                return new List<WordCountTrend>();

            var trends = entries
                .GroupBy(e => e.EntryDate.Date)
                .Select(g => new WordCountTrend
                {
                    Date = g.Key,
                    WordCount = g.Sum(e => string.IsNullOrWhiteSpace(e.Content)
                        ? 0
                        : e.Content.Split(new[] { ' ', '\t', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries).Length),
                    AverageWords = g.Average(e => string.IsNullOrWhiteSpace(e.Content)
                        ? 0
                        : e.Content.Split(new[] { ' ', '\t', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries).Length)
                })
                .ToList();

            return trends;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting word count trends for user: {UserId}", userId);
            throw;
        }
    }
}

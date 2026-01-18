using JournalApp.Models;

namespace JournalApp.Services.Interfaces;

/// <summary>
/// Service interface for analytics and dashboard data
/// </summary>
public interface IAnalyticsService
{
    Task<DashboardAnalytics> GetDashboardAnalyticsAsync(int userId, DateTime? startDate = null, DateTime? endDate = null);
    Task<StreakInfo> GetStreakInfoAsync(int userId);
    Task<List<MoodDistribution>> GetMoodDistributionAsync(int userId, DateTime? startDate = null, DateTime? endDate = null);
    Task<List<TagUsage>> GetMostUsedTagsAsync(int userId, int count = 10, DateTime? startDate = null, DateTime? endDate = null);
    Task<List<WordCountTrend>> GetWordCountTrendsAsync(int userId, DateTime? startDate = null, DateTime? endDate = null);
}

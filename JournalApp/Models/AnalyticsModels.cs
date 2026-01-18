namespace JournalApp.Models;

/// <summary>
/// View models for analytics and dashboard data
/// </summary>
public class MoodDistribution
{
    public string Category { get; set; } = string.Empty;
    public int Count { get; set; }
    public double Percentage { get; set; }
}

public class TagUsage
{
    public string TagName { get; set; } = string.Empty;
    public int Count { get; set; }
    public double Percentage { get; set; }
}

public class WordCountTrend
{
    public DateTime Date { get; set; }
    public int WordCount { get; set; }
    public double AverageWords { get; set; }
}

public class StreakInfo
{
    public int CurrentStreak { get; set; }
    public int LongestStreak { get; set; }
    public List<DateTime> MissedDays { get; set; } = new List<DateTime>();
}

public class DashboardAnalytics
{
    public MoodDistribution MoodDistribution { get; set; } = new MoodDistribution();
    public List<MoodDistribution> MoodDistributionByCategory { get; set; } = new List<MoodDistribution>();
    public string? MostFrequentMood { get; set; }
    public StreakInfo StreakInfo { get; set; } = new StreakInfo();
    public List<TagUsage> MostUsedTags { get; set; } = new List<TagUsage>();
    public List<TagUsage> TagBreakdown { get; set; } = new List<TagUsage>();
    public List<WordCountTrend> WordCountTrends { get; set; } = new List<WordCountTrend>();
    public int TotalEntries { get; set; }
    public DateTime? FirstEntryDate { get; set; }
    public DateTime? LastEntryDate { get; set; }
}

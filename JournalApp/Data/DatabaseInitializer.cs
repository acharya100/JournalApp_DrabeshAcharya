using JournalApp.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace JournalApp.Data;

/// <summary>
/// Handles database initialization and seeding
/// </summary>
public class DatabaseInitializer
{
    private readonly JournalDbContext _context;
    private readonly ILogger<DatabaseInitializer> _logger;

    public DatabaseInitializer(JournalDbContext context, ILogger<DatabaseInitializer> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task InitializeAsync()
    {
        try
        {
            // Ensure database is created
            await _context.Database.EnsureCreatedAsync();
            
            // Check if data already exists
            if (await _context.Moods.AnyAsync())
            {
                _logger.LogInformation("Database already initialized");
                return;
            }

            await SeedMoodsAsync();
            await SeedTagsAsync();
            
            _logger.LogInformation("Database initialized successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error initializing database");
            throw;
        }
    }

    private async Task SeedMoodsAsync()
    {
        var moods = new List<Mood>
        {
            // Positive moods
            new Mood { MoodName = "Happy", Category = "Positive", Emoji = "😊" },
            new Mood { MoodName = "Excited", Category = "Positive", Emoji = "🤩" },
            new Mood { MoodName = "Relaxed", Category = "Positive", Emoji = "😌" },
            new Mood { MoodName = "Grateful", Category = "Positive", Emoji = "🙏" },
            new Mood { MoodName = "Confident", Category = "Positive", Emoji = "💪" },
            
            // Neutral moods
            new Mood { MoodName = "Calm", Category = "Neutral", Emoji = "😐" },
            new Mood { MoodName = "Thoughtful", Category = "Neutral", Emoji = "🤔" },
            new Mood { MoodName = "Curious", Category = "Neutral", Emoji = "🧐" },
            new Mood { MoodName = "Nostalgic", Category = "Neutral", Emoji = "😊" },
            new Mood { MoodName = "Bored", Category = "Neutral", Emoji = "😑" },
            
            // Negative moods
            new Mood { MoodName = "Sad", Category = "Negative", Emoji = "😔" },
            new Mood { MoodName = "Angry", Category = "Negative", Emoji = "😠" },
            new Mood { MoodName = "Stressed", Category = "Negative", Emoji = "😰" },
            new Mood { MoodName = "Lonely", Category = "Negative", Emoji = "😞" },
            new Mood { MoodName = "Anxious", Category = "Negative", Emoji = "😟" }
        };

        await _context.Moods.AddRangeAsync(moods);
        await _context.SaveChangesAsync();
    }

    private async Task SeedTagsAsync()
    {
        var predefinedTags = new[]
        {
            "Work", "Career", "Studies", "Family", "Friends", "Relationships",
            "Health", "Fitness", "Personal Growth", "Self-care", "Hobbies",
            "Travel", "Nature", "Finance", "Spirituality", "Birthday",
            "Holiday", "Vacation", "Celebration", "Exercise", "Reading",
            "Writing", "Cooking", "Meditation", "Yoga", "Music", "Shopping",
            "Parenting", "Projects", "Planning", "Reflection"
        };

        var tags = predefinedTags.Select(tagName => new Tag
        {
            TagName = tagName,
            IsPredefined = true
        }).ToList();

        await _context.Tags.AddRangeAsync(tags);
        await _context.SaveChangesAsync();
    }
}

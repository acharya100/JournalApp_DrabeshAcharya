namespace JournalApp.Models;

/// <summary>
/// Represents a mood that can be associated with journal entries
/// </summary>
public class Mood
{
    public int MoodId { get; set; }
    public string MoodName { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty; // Positive, Neutral, Negative
    public string? Emoji { get; set; } // Optional emoji representation
    
    // Navigation properties
    public ICollection<JournalEntry> PrimaryMoodEntries { get; set; } = new List<JournalEntry>();
    public ICollection<JournalEntryMood> SecondaryMoodEntries { get; set; } = new List<JournalEntryMood>();
}

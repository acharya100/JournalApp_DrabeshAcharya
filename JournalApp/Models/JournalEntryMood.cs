namespace JournalApp.Models;

/// <summary>
/// Junction table for journal entries and secondary moods
/// </summary>
public class JournalEntryMood
{
    public int JournalEntryMoodId { get; set; }
    public int EntryId { get; set; }
    public int MoodId { get; set; }
    
    // Navigation properties
    public JournalEntry? JournalEntry { get; set; }
    public Mood? Mood { get; set; }
}

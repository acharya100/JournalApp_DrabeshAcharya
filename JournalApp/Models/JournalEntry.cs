namespace JournalApp.Models;

/// <summary>
/// Represents a journal entry with rich text content, mood tracking, and tags
/// </summary>
public class JournalEntry
{
    public int EntryId { get; set; }
    public int UserId { get; set; }
    public DateTime EntryDate { get; set; } // One entry per day
    public string JournalTitle { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty; // Markdown/rich text content
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    // Mood relationships
    public int PrimaryMoodId { get; set; } // Required primary mood
    public Mood? PrimaryMood { get; set; }
    
    // Secondary moods (up to 2)
    public ICollection<JournalEntryMood> SecondaryMoods { get; set; } = new List<JournalEntryMood>();
    
    // Tags
    public ICollection<JournalEntryTag> Tags { get; set; } = new List<JournalEntryTag>();
    
    // User relationship
    public User? User { get; set; }
    
    // Helper property to get word count
    public int WordCount => string.IsNullOrWhiteSpace(Content) 
        ? 0 
        : Content.Split(new[] { ' ', '\t', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries).Length;
}

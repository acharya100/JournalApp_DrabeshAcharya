namespace JournalApp.Models;

/// <summary>
/// Junction table for journal entries and tags
/// </summary>
public class JournalEntryTag
{
    public int JournalEntryTagId { get; set; }
    public int EntryId { get; set; }
    public int TagId { get; set; }
    
    // Navigation properties
    public JournalEntry? JournalEntry { get; set; }
    public Tag? Tag { get; set; }
}

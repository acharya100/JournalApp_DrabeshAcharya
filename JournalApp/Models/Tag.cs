namespace JournalApp.Models;

/// <summary>
/// Represents a tag that can be associated with journal entries
/// </summary>
public class Tag
{
    public int TagId { get; set; }
    public string TagName { get; set; } = string.Empty;
    public bool IsPredefined { get; set; } = false; // True for pre-built tags, false for custom tags
    
    // Navigation properties
    public ICollection<JournalEntryTag> JournalEntryTags { get; set; } = new List<JournalEntryTag>();
}

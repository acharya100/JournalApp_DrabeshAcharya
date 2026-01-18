namespace JournalApp.Models;

/// <summary>
/// Represents a user in the journaling application
/// </summary>
public class User
{
    public int UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty; // Encrypted password
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public ICollection<JournalEntry> JournalEntries { get; set; } = new List<JournalEntry>();
}

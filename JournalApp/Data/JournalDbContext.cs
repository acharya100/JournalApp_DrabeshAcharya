using Microsoft.EntityFrameworkCore;
using JournalApp.Models;

namespace JournalApp.Data;

/// <summary>
/// Database context for the Journal application
/// </summary>
public class JournalDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<JournalEntry> JournalEntries { get; set; }
    public DbSet<Mood> Moods { get; set; }
    public DbSet<Tag> Tags { get; set; }
    public DbSet<JournalEntryMood> JournalEntryMoods { get; set; }
    public DbSet<JournalEntryTag> JournalEntryTags { get; set; }

    public JournalDbContext(DbContextOptions<JournalDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User configuration
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId);
            entity.Property(e => e.Username).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Password).IsRequired().HasMaxLength(255);
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.HasIndex(e => e.Username).IsUnique();
        });

        // Journal Entry configuration
        modelBuilder.Entity<JournalEntry>(entity =>
        {
            entity.HasKey(e => e.EntryId);
            entity.Property(e => e.JournalTitle).IsRequired().HasMaxLength(500);
            entity.Property(e => e.Content).HasColumnType("TEXT");
            entity.Property(e => e.EntryDate).IsRequired();
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.UpdatedAt).IsRequired();
            
            // One entry per day per user
            entity.HasIndex(e => new { e.UserId, e.EntryDate }).IsUnique();
            
            // Relationships
            entity.HasOne(e => e.User)
                .WithMany(u => u.JournalEntries)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            
            entity.HasOne(e => e.PrimaryMood)
                .WithMany(m => m.PrimaryMoodEntries)
                .HasForeignKey(e => e.PrimaryMoodId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Mood configuration
        modelBuilder.Entity<Mood>(entity =>
        {
            entity.HasKey(e => e.MoodId);
            entity.Property(e => e.MoodName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Category).IsRequired().HasMaxLength(50);
            entity.HasIndex(e => e.MoodName).IsUnique();
        });

        // Tag configuration
        modelBuilder.Entity<Tag>(entity =>
        {
            entity.HasKey(e => e.TagId);
            entity.Property(e => e.TagName).IsRequired().HasMaxLength(100);
            entity.HasIndex(e => e.TagName).IsUnique();
        });

        // Journal Entry Mood (Secondary moods) configuration
        modelBuilder.Entity<JournalEntryMood>(entity =>
        {
            entity.HasKey(e => e.JournalEntryMoodId);
            entity.HasIndex(e => new { e.EntryId, e.MoodId }).IsUnique();
            
            entity.HasOne(e => e.JournalEntry)
                .WithMany(j => j.SecondaryMoods)
                .HasForeignKey(e => e.EntryId)
                .OnDelete(DeleteBehavior.Cascade);
            
            entity.HasOne(e => e.Mood)
                .WithMany(m => m.SecondaryMoodEntries)
                .HasForeignKey(e => e.MoodId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Journal Entry Tag configuration
        modelBuilder.Entity<JournalEntryTag>(entity =>
        {
            entity.HasKey(e => e.JournalEntryTagId);
            entity.HasIndex(e => new { e.EntryId, e.TagId }).IsUnique();
            
            entity.HasOne(e => e.JournalEntry)
                .WithMany(j => j.Tags)
                .HasForeignKey(e => e.EntryId)
                .OnDelete(DeleteBehavior.Cascade);
            
            entity.HasOne(e => e.Tag)
                .WithMany(t => t.JournalEntryTags)
                .HasForeignKey(e => e.TagId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}

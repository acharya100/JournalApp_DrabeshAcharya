using JournalApp.Data;
using JournalApp.Models;
using JournalApp.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace JournalApp.Services;

/// <summary>
/// Service implementation for tag management
/// </summary>
public class TagService : ITagService
{
    private readonly JournalDbContext _context;
    private readonly ILogger<TagService> _logger;

    public TagService(JournalDbContext context, ILogger<TagService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<List<Tag>> GetAllTagsAsync()
    {
        try
        {
            return await _context.Tags
                .OrderBy(t => t.IsPredefined ? 0 : 1)
                .ThenBy(t => t.TagName)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all tags");
            throw;
        }
    }

    public async Task<List<Tag>> GetPredefinedTagsAsync()
    {
        try
        {
            return await _context.Tags
                .Where(t => t.IsPredefined)
                .OrderBy(t => t.TagName)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting predefined tags");
            throw;
        }
    }

    public async Task<Tag> CreateTagAsync(string tagName)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(tagName))
                throw new ArgumentException("Tag name cannot be null or empty", nameof(tagName));

            // Check if tag already exists
            var existingTag = await GetTagByNameAsync(tagName);
            if (existingTag != null)
                return existingTag;

            var tag = new Tag
            {
                TagName = tagName.Trim(),
                IsPredefined = false
            };

            _context.Tags.Add(tag);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Tag created successfully: {TagName}", tagName);
            return tag;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating tag: {TagName}", tagName);
            throw;
        }
    }

    public async Task<Tag?> GetTagByIdAsync(int tagId)
    {
        try
        {
            return await _context.Tags.FindAsync(tagId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting tag by ID: {TagId}", tagId);
            throw;
        }
    }

    public async Task<Tag?> GetTagByNameAsync(string tagName)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(tagName))
                return null;

            return await _context.Tags
                .FirstOrDefaultAsync(t => t.TagName.ToLower() == tagName.ToLower().Trim());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting tag by name: {TagName}", tagName);
            throw;
        }
    }
}

using JournalApp.Models;
using JournalApp.Services.Interfaces;
using Microsoft.Extensions.Logging;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Text;
using QuestColors = QuestPDF.Helpers.Colors;

namespace JournalApp.Services;

/// <summary>
/// Service implementation for PDF export functionality
/// </summary>
public class PdfExportService : IPdfExportService
{
    private readonly ILogger<PdfExportService> _logger;

    public PdfExportService(ILogger<PdfExportService> logger)
    {
        _logger = logger;
        QuestPDF.Settings.License = LicenseType.Community;
    }

    public async Task<byte[]> ExportEntriesToPdfAsync(int userId, List<JournalEntry> entries, DateTime? startDate = null, DateTime? endDate = null)
    {
        try
        {
            if (entries == null || !entries.Any())
                throw new ArgumentException("No entries to export", nameof(entries));

            var pdfBytes = await Task.Run(() =>
            {
                var document = Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        page.Size(PageSizes.A4);
                        page.Margin(2, Unit.Centimetre);
                        page.PageColor(QuestColors.White);
                        page.DefaultTextStyle(x => x.FontSize(11).FontFamily("Arial"));

                        page.Header()
                            .Column(column =>
                            {
                                column.Item()
                                    .Text("Journal Entries")
                                    .FontSize(20)
                                    .Bold()
                                    .AlignCenter();

                                if (startDate.HasValue || endDate.HasValue)
                                {
                                    column.Item()
                                        .PaddingTop(5)
                                        .Text($"Date Range: {startDate?.ToString("yyyy-MM-dd") ?? "Start"} to {endDate?.ToString("yyyy-MM-dd") ?? "End"}")
                                        .FontSize(10)
                                        .AlignCenter();
                                }
                            });

                        page.Content()
                            .PaddingVertical(1, Unit.Centimetre)
                            .Column(column =>
                            {
                                foreach (var entry in entries.OrderBy(e => e.EntryDate))
                                {
                                    column.Item().Element(container =>
                                    {
                                        container
                                            .Border(1)
                                            .BorderColor(QuestColors.Grey.Lighten2)
                                            .Padding(15)
                                            .Column(col =>
                                            {
                                                // Entry Date and Title
                                                col.Item().Row(row =>
                                                {
                                                    row.RelativeItem().Text(entry.EntryDate.ToString("MMMM dd, yyyy"))
                                                        .FontSize(14)
                                                        .Bold()
                                                        .FontColor(QuestColors.Blue.Darken2);
                                                    
                                                    row.ConstantItem(100).Text(entry.PrimaryMood?.MoodName ?? "N/A")
                                                        .FontSize(10)
                                                        .AlignRight();
                                                });

                                                col.Item().PaddingTop(5).Text(entry.JournalTitle)
                                                    .FontSize(16)
                                                    .Bold()
                                                    .FontColor(QuestColors.Black);

                                                // Secondary Moods
                                                if (entry.SecondaryMoods != null && entry.SecondaryMoods.Any())
                                                {
                                                    var secondaryMoods = string.Join(", ", 
                                                        entry.SecondaryMoods.Select(sm => sm.Mood?.MoodName ?? ""));
                                                    col.Item().PaddingTop(3).Text($"Secondary Moods: {secondaryMoods}")
                                                        .FontSize(10)
                                                        .FontColor(QuestColors.Grey.Darken1);
                                                }

                                                // Tags
                                                if (entry.Tags != null && entry.Tags.Any())
                                                {
                                                    var tags = string.Join(", ", 
                                                        entry.Tags.Select(t => t.Tag?.TagName ?? ""));
                                                    col.Item().PaddingTop(2).Text($"Tags: {tags}")
                                                        .FontSize(10)
                                                        .FontColor(QuestColors.Grey.Darken1);
                                                }

                                                // Content
                                                col.Item().PaddingTop(10).Text(entry.Content)
                                                    .FontSize(11)
                                                    .AlignLeft()
                                                    .LineHeight(1.5f);

                                                // Timestamps
                                                col.Item().PaddingTop(10).Row(row =>
                                                {
                                                    row.RelativeItem().Text($"Created: {entry.CreatedAt:yyyy-MM-dd HH:mm}")
                                                        .FontSize(8)
                                                        .FontColor(QuestColors.Grey.Medium);
                                                    
                                                    if (entry.UpdatedAt != entry.CreatedAt)
                                                    {
                                                        row.ConstantItem(150).Text($"Updated: {entry.UpdatedAt:yyyy-MM-dd HH:mm}")
                                                            .FontSize(8)
                                                            .FontColor(QuestColors.Grey.Medium)
                                                            .AlignRight();
                                                    }
                                                });
                                            });
                                    });

                                    column.Item().PaddingBottom(10);
                                }
                            });

                        page.Footer()
                            .AlignCenter()
                            .DefaultTextStyle(style => style.FontSize(9).FontColor(QuestColors.Grey.Medium))
                            .Text(x =>
                            {
                                x.Span("Page ");
                                x.CurrentPageNumber();
                                x.Span(" of ");
                                x.TotalPages();
                            });
                    });
                });

                return document.GeneratePdf();
            });

            _logger.LogInformation("PDF exported successfully with {Count} entries", entries.Count);
            return pdfBytes;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error exporting entries to PDF");
            throw;
        }
    }

    public async Task<string> SavePdfToFileAsync(byte[] pdfBytes, string fileName)
    {
        try
        {
            if (pdfBytes == null || pdfBytes.Length == 0)
                throw new ArgumentException("PDF bytes cannot be null or empty", nameof(pdfBytes));

            if (string.IsNullOrWhiteSpace(fileName))
                fileName = $"JournalExport_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";

            if (!fileName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
                fileName += ".pdf";

            // For MAUI, we'll use the file system
            var filePath = Path.Combine(FileSystem.AppDataDirectory, fileName);
            await File.WriteAllBytesAsync(filePath, pdfBytes);

            _logger.LogInformation("PDF saved to file: {FilePath}", filePath);
            return filePath;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving PDF to file");
            throw;
        }
    }
}

using JournalApp.Models;

namespace JournalApp.Services.Interfaces;

/// <summary>
/// Service interface for PDF export functionality
/// </summary>
public interface IPdfExportService
{
    Task<byte[]> ExportEntriesToPdfAsync(int userId, List<JournalEntry> entries, DateTime? startDate = null, DateTime? endDate = null);
    Task<string> SavePdfToFileAsync(byte[] pdfBytes, string fileName);
}

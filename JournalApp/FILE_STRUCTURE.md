# JournalApp - Complete File Structure

This document lists all files created for the JournalApp project.

## Project Files

### Configuration
- `JournalApp.csproj` - Project file with NuGet packages
- `MauiProgram.cs` - Application configuration and service registration
- `README.md` - Comprehensive documentation

### Models (Data Entities)
- `Models/User.cs` - User entity
- `Models/JournalEntry.cs` - Journal entry entity
- `Models/Mood.cs` - Mood entity
- `Models/Tag.cs` - Tag entity
- `Models/JournalEntryMood.cs` - Junction table for secondary moods
- `Models/JournalEntryTag.cs` - Junction table for tags
- `Models/AnalyticsModels.cs` - View models for analytics

### Data Layer
- `Data/JournalDbContext.cs` - Entity Framework DbContext
- `Data/DatabaseInitializer.cs` - Database initialization and seeding

### Services - Interfaces
- `Services/Interfaces/IUserService.cs`
- `Services/Interfaces/IJournalService.cs`
- `Services/Interfaces/IMoodService.cs`
- `Services/Interfaces/ITagService.cs`
- `Services/Interfaces/IAnalyticsService.cs`
- `Services/Interfaces/IAuthService.cs`
- `Services/Interfaces/IPdfExportService.cs`

### Services - Implementations
- `Services/UserService.cs`
- `Services/JournalService.cs`
- `Services/MoodService.cs`
- `Services/TagService.cs`
- `Services/AnalyticsService.cs`
- `Services/AuthService.cs`
- `Services/PdfExportService.cs`

### Blazor Components - Layout
- `Components/Layout/MainLayout.razor`
- `Components/Layout/MainLayout.razor.css`

### Blazor Components - Pages
- `Components/Pages/Home.razor` - Home page (redirects)
- `Components/Pages/Login.razor` - Login/Register page
- `Components/Pages/Login.razor.css` - Login page styles
- `Components/Pages/Dashboard.razor` - Analytics dashboard
- `Components/Pages/JournalEditor.razor` - Create/Edit journal entries
- `Components/Pages/JournalList.razor` - Paginated list view
- `Components/Pages/JournalCalendar.razor` - Calendar view
- `Components/Pages/JournalCalendar.razor.css` - Calendar styles
- `Components/Pages/JournalSearch.razor` - Search and filter
- `Components/Pages/Export.razor` - PDF export page
- `Components/Pages/Settings.razor` - User settings

### Blazor Components - Shared
- `Components/Shared/NavMenu.razor` - Navigation menu
- `Components/Shared/NavMenu.razor.css` - Navigation styles
- `Components/Shared/AuthorizeAttribute.cs` - Authorization attribute

### Blazor Configuration
- `Components/Routes.razor` - Route configuration
- `Components/_Imports.razor` - Global imports

### Web Assets
- `wwwroot/index.html` - HTML entry point
- `wwwroot/css/app.css` - Base application styles
- `wwwroot/css/dashboard.css` - Dashboard and component styles

## NuGet Packages Required

All packages are configured in `JournalApp.csproj`:

1. **Microsoft.EntityFrameworkCore.Sqlite.Core** (9.0.10)
2. **Microsoft.EntityFrameworkCore.Design** (9.0.10)
3. **BCrypt.Net-Next** (4.0.3)
4. **QuestPDF** (2024.10.2)
5. **Markdig** (0.37.0)
6. **Blazorise** (1.7.2)
7. **Blazorise.Bootstrap5** (1.7.2)
8. **Blazorise.Icons.FontAwesome** (1.7.2)
9. **Blazorise.RichTextEdit** (1.7.2)
10. **Chart.Blazor** (1.0.0)
11. **Microsoft.Extensions.Logging** (9.0.0)

## Features Implemented

✅ User authentication (password and PIN)
✅ User registration
✅ Journal entry CRUD operations
✅ One entry per day constraint
✅ Rich text/Markdown content support
✅ Mood tracking (primary + up to 2 secondary)
✅ Tag system (predefined and custom)
✅ Calendar navigation
✅ Paginated list view
✅ Search and filter functionality
✅ Streak tracking
✅ Dashboard analytics
✅ PDF export
✅ Settings page
✅ Error handling and logging
✅ Input validation
✅ Professional UI with Bootstrap

## Database Schema

- **Users**: UserId, Username, Password (encrypted), CreatedAt
- **JournalEntries**: EntryId, UserId, EntryDate, JournalTitle, Content, CreatedAt, UpdatedAt, PrimaryMoodId
- **Moods**: MoodId, MoodName, Category, Emoji
- **Tags**: TagId, TagName, IsPredefined
- **JournalEntryMoods**: Junction table for secondary moods
- **JournalEntryTags**: Junction table for tags

## Next Steps

1. Restore NuGet packages: `dotnet restore`
2. Build the project: `dotnet build`
3. Run the application: `dotnet run` or F5 in Visual Studio
4. Register a new user account
5. Start creating journal entries!

# JournalApp - .NET MAUI Hybrid Blazor Application

A modern, feature-rich desktop journaling application built with .NET MAUI and Blazor, featuring secure local data storage, mood tracking, analytics, and more.

## Features

### Core Features
- **Journal Entry Management**: Create, update, and delete daily journal entries (one per day)
- **Rich Text/Markdown Support**: Write entries with formatting support
- **Mood Tracking**: 
  - Primary mood (required)
  - Up to 2 secondary moods (optional)
  - Categories: Positive, Neutral, Negative
- **Tagging System**: Custom and pre-built tags for organizing entries
- **Calendar Navigation**: Visual calendar view to navigate entries
- **Paginated List View**: Browse all entries with pagination
- **Search & Filter**: Search by title/content, filter by date range, moods, or tags
- **Streak Tracking**: Track daily streaks, longest streak, and missed days
- **Theme Customization**: Light/dark theme support
- **Dashboard Analytics**: 
  - Mood distribution charts
  - Most frequent moods
  - Most used tags
  - Word count trends
  - Streak information
- **Security**: Password and PIN protection
- **PDF Export**: Export entries as PDF by date range

## Technology Stack

- **.NET 9.0** (Standard Term Support)
- **.NET MAUI** (Multi-platform App UI)
- **Blazor Hybrid** (Web UI in native app)
- **Entity Framework Core 9.0.10** with SQLite
- **BCrypt.Net** for password hashing
- **QuestPDF** for PDF generation
- **Markdig** for Markdown support

## Prerequisites

- Visual Studio 2022 (17.8 or later) with .NET MAUI workload
- .NET 9.0 SDK
- Windows 10/11 (for Windows development)

## Installation

### 1. Clone or Extract the Project

Extract the project to your desired location.

### 2. Install NuGet Packages

The following packages are already configured in `JournalApp.csproj`:

- `Microsoft.EntityFrameworkCore.Sqlite.Core` (9.0.10)
- `Microsoft.EntityFrameworkCore.Design` (9.0.10)
- `BCrypt.Net-Next` (4.0.3)
- `QuestPDF` (2024.10.2)
- `Markdig` (0.37.0)
- `Blazorise` (1.7.2)
- `Blazorise.Bootstrap5` (1.7.2)
- `Blazorise.Icons.FontAwesome` (1.7.2)
- `Blazorise.RichTextEdit` (1.7.2)
- `Chart.Blazor` (1.0.0)

To restore packages:
```bash
dotnet restore
```

### 3. Build the Project

```bash
dotnet build
```

### 4. Run the Application

In Visual Studio:
- Select your target platform (Windows, Android, iOS, etc.)
- Press F5 to run

Or via command line:
```bash
dotnet run
```

## Project Structure

```
JournalApp/
├── Components/
│   ├── Layout/
│   │   ├── MainLayout.razor          # Main application layout
│   │   └── MainLayout.razor.css
│   ├── Pages/
│   │   ├── Dashboard.razor            # Analytics dashboard
│   │   ├── Login.razor                # Login/Register page
│   │   ├── JournalEditor.razor        # Create/Edit journal entries
│   │   ├── JournalList.razor          # Paginated list view
│   │   ├── JournalCalendar.razor      # Calendar view
│   │   ├── JournalSearch.razor        # Search and filter
│   │   └── Settings.razor             # User settings
│   ├── Shared/
│   │   ├── NavMenu.razor              # Navigation menu
│   │   └── AuthorizeAttribute.cs      # Authorization attribute
│   ├── Routes.razor                   # Route configuration
│   └── _Imports.razor                 # Global imports
├── Data/
│   ├── JournalDbContext.cs            # EF Core DbContext
│   └── DatabaseInitializer.cs         # Database seeding
├── Models/
│   ├── User.cs                        # User entity
│   ├── JournalEntry.cs                # Journal entry entity
│   ├── Mood.cs                        # Mood entity
│   ├── Tag.cs                         # Tag entity
│   ├── JournalEntryMood.cs            # Junction table
│   ├── JournalEntryTag.cs             # Junction table
│   └── AnalyticsModels.cs             # Analytics view models
├── Services/
│   ├── Interfaces/                    # Service interfaces
│   └── [Service implementations]      # Service implementations
├── wwwroot/
│   ├── css/
│   │   ├── app.css                    # Base styles
│   │   └── dashboard.css             # Dashboard styles
│   └── index.html                     # HTML entry point
├── MauiProgram.cs                     # App configuration
└── JournalApp.csproj                  # Project file
```

## Database

The application uses SQLite for local data storage. The database file is created automatically at:
- **Windows**: `%LOCALAPPDATA%\JournalApp\journal.db`
- **Android**: App's data directory
- **iOS**: App's documents directory

### Database Schema

- **Users**: User accounts with encrypted passwords
- **JournalEntries**: Journal entries with content, dates, and relationships
- **Moods**: Predefined moods (Happy, Sad, etc.) with categories
- **Tags**: Predefined and custom tags
- **JournalEntryMoods**: Many-to-many relationship for secondary moods
- **JournalEntryTags**: Many-to-many relationship for tags

### Initial Data

The database is automatically seeded with:
- **15 Moods** across 3 categories (Positive, Neutral, Negative)
- **31 Predefined Tags** (Work, Health, Travel, etc.)

## Usage Guide

### First Time Setup

1. **Launch the Application**
2. **Register a New Account**:
   - Click "Register" on the login page
   - Enter username and password
   - Click "Register"

3. **Login**:
   - Enter your username and password
   - Or set up a PIN for quick access

### Creating Journal Entries

1. Navigate to **"New Entry"** from the sidebar
2. Select the entry date (defaults to today)
3. Enter a title
4. Write your content (supports Markdown)
5. Select a **primary mood** (required)
6. Optionally select up to 2 **secondary moods**
7. Add **tags** (predefined or custom)
8. Click **"Save Entry"**

### Viewing Entries

- **Dashboard**: Overview with analytics and quick actions
- **All Entries**: Paginated list of all entries
- **Calendar**: Visual calendar showing entries by date
- **Search**: Search and filter entries by various criteria

### Analytics Dashboard

The dashboard provides:
- Total entries count
- Current and longest streaks
- Most frequent mood
- Mood distribution charts
- Most used tags
- Word count trends
- Missed days list

### Settings

- **Change Password**: Update your account password
- **PIN Protection**: Set a PIN for quick login
- **Theme**: Switch between light and dark themes

### PDF Export

1. Navigate to Search page
2. Apply filters to select entries
3. Export functionality (to be implemented in UI)

## Security Features

- **Password Encryption**: Passwords are hashed using BCrypt
- **PIN Protection**: Optional PIN for quick access
- **Local Storage**: All data stored locally on device
- **Authentication**: Session-based authentication

## Development Notes

### Error Handling

- All services include comprehensive error handling
- Logging is implemented throughout the application
- User-friendly error messages displayed in UI

### Code Quality

- **Separation of Concerns**: Clear separation between data, services, and UI
- **Dependency Injection**: All services registered via DI
- **Repository Pattern**: Service layer abstracts data access
- **Validation**: Input validation using Data Annotations
- **Async/Await**: All database operations are asynchronous

### Extending the Application

To add new features:

1. **Add Entity Models** in `Models/` folder
2. **Update DbContext** in `Data/JournalDbContext.cs`
3. **Create Service Interface** in `Services/Interfaces/`
4. **Implement Service** in `Services/`
5. **Register Service** in `MauiProgram.cs`
6. **Create Blazor Component** in `Components/Pages/`
7. **Add Route** in `Components/Routes.razor` (if needed)

## Troubleshooting

### Database Issues

If the database doesn't initialize:
1. Check file permissions in the app data directory
2. Verify SQLite package is installed correctly
3. Check application logs for errors

### Build Errors

1. Ensure all NuGet packages are restored: `dotnet restore`
2. Verify .NET 9.0 SDK is installed
3. Check that MAUI workload is installed: `dotnet workload install maui`

### Runtime Errors

1. Check application logs in the output window
2. Verify database file location and permissions
3. Ensure all services are properly registered in `MauiProgram.cs`

## Future Enhancements

Potential features to add:
- Rich text editor with WYSIWYG support
- Image attachments
- Cloud sync
- Export to other formats (JSON, CSV)
- Advanced charting with interactive graphs
- Reminders and notifications
- Entry templates
- Multi-language support

## License

This project is provided as-is for educational and development purposes.

## Support

For issues or questions, please refer to the project documentation or contact the development team.

---

**Built with ❤️ using .NET MAUI and Blazor**

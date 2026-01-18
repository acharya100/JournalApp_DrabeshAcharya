using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using JournalApp.Data;
using JournalApp.Services;
using JournalApp.Services.Interfaces;
using Microsoft.Maui.Storage;
using SQLitePCL;

namespace JournalApp
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            // Initializing SQLite provider
            Batteries.Init();
            
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            builder.Services.AddMauiBlazorWebView();

#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
            builder.Logging.AddDebug();
#endif

            // Configure SQLite database
            var dbPath = Path.Combine(FileSystem.AppDataDirectory, "journal.db");
            builder.Services.AddDbContext<JournalDbContext>(options =>
                options.UseSqlite($"Data Source={dbPath}"));

            // Register services
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IJournalService, JournalService>();
            builder.Services.AddScoped<IMoodService, MoodService>();
            builder.Services.AddScoped<ITagService, TagService>();
            builder.Services.AddScoped<IAnalyticsService, AnalyticsService>();
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IPdfExportService, PdfExportService>();

            // Initialize database
            var app = builder.Build();
            InitializeDatabase(app);

            return app;
        }

        private static void InitializeDatabase(MauiApp app)
        {
            try
            {
                using var scope = app.Services.CreateScope();
                var initializer = new DatabaseInitializer(
                    scope.ServiceProvider.GetRequiredService<JournalDbContext>(),
                    scope.ServiceProvider.GetRequiredService<ILogger<DatabaseInitializer>>()
                );
                initializer.InitializeAsync().Wait();
            }
            catch (Exception ex)
            {
                var logger = app.Services.GetRequiredService<ILogger<DatabaseInitializer>>();
                logger.LogError(ex, "Error initializing database");
            }
        }
    }
}

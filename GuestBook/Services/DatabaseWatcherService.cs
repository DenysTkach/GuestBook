using Microsoft.AspNetCore.SignalR;
using GuestBook.Hubs;

namespace GuestBook.Services;

public class DatabaseWatcherService : IHostedService, IDisposable
{
    private readonly IHubContext<DatabaseHub> _hubContext;
    private readonly ILogger<DatabaseWatcherService> _logger;
    private readonly string _databasePath;
    private FileSystemWatcher? _watcher;
    private long _lastNotificationTicks = 0;
    private readonly long _debounceIntervalTicks = TimeSpan.FromMilliseconds(500).Ticks;

    public DatabaseWatcherService(
        IHubContext<DatabaseHub> hubContext,
        ILogger<DatabaseWatcherService> logger,
        IConfiguration configuration)
    {
        _hubContext = hubContext;
        _logger = logger;
        
        // Extract database path from connection string
        var connectionString = configuration.GetConnectionString("DefaultConnection") ?? "Data Source=guestbook.db";
        _databasePath = ExtractDatabasePath(connectionString);
    }

    private static string ExtractDatabasePath(string connectionString)
    {
        // Parse "Data Source=guestbook.db" format
        var parts = connectionString.Split(';');
        foreach (var part in parts)
        {
            var keyValue = part.Split('=');
            if (keyValue.Length == 2 && keyValue[0].Trim().Equals("Data Source", StringComparison.OrdinalIgnoreCase))
            {
                return keyValue[1].Trim();
            }
        }
        return "guestbook.db";
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        var fullPath = Path.GetFullPath(_databasePath);
        var directory = Path.GetDirectoryName(fullPath) ?? ".";
        var fileName = Path.GetFileName(fullPath);

        _logger.LogInformation("Starting database watcher for: {DatabasePath}", fullPath);

        _watcher = new FileSystemWatcher(directory)
        {
            Filter = fileName,
            NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.Size,
            EnableRaisingEvents = true
        };

        _watcher.Changed += OnDatabaseChanged;

        return Task.CompletedTask;
    }

    private async void OnDatabaseChanged(object sender, FileSystemEventArgs e)
    {
        try
        {
            // Thread-safe debounce using Interlocked
            var nowTicks = DateTime.UtcNow.Ticks;
            var previousTicks = Interlocked.Exchange(ref _lastNotificationTicks, nowTicks);
            
            if (nowTicks - previousTicks < _debounceIntervalTicks)
            {
                return;
            }

            _logger.LogInformation("Database file changed, notifying clients");
            await _hubContext.Clients.All.SendAsync("DatabaseChanged");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error notifying clients of database change");
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Stopping database watcher");
        _watcher?.Dispose();
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _watcher?.Dispose();
    }
}

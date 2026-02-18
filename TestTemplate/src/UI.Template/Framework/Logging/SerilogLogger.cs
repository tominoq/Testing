namespace UI.Template.Framework.Logging;

/// <summary>
/// The <see cref="Serilog.Core.Logger" /> wrapper.
/// </summary>
/// <param name="logger">Instance of the <see cref="Serilog.Core.Logger" /> class used for actual logging.</param>
/// <param name="logPath">The path to the directory where logs should be stored.</param>
internal sealed class SerilogLogger(Serilog.Core.Logger logger, string logPath) : ILogger
{
    private bool _isDisposed;

    /// <inheritdoc />
    public IReadOnlyCollection<string> AllLogs
    {
        get
        {
            if (Directory.Exists(LogPath))
            {
                return Array.AsReadOnly(Directory.GetFiles(LogPath));
            }

            return Array.AsReadOnly(Array.Empty<string>());
        }
    }

    /// <inheritdoc />
    public string LogPath { get; } = logPath;

    /// <inheritdoc />
    public void LogVerbose(string message) => logger.Verbose(message);

    /// <inheritdoc />
    public void LogVerbose(Exception? exception, string message) => logger.Verbose(exception, message);

    /// <inheritdoc />
    public void LogDebug(string message) => logger.Debug(message);

    /// <inheritdoc />
    public void LogDebug(Exception? exception, string message) => logger.Debug(exception, message);

    /// <inheritdoc />
    public void LogInformation(string message) => logger.Information(message);

    /// <inheritdoc />
    public void LogInformation(Exception? exception, string message) => logger.Information(exception, message);

    /// <inheritdoc />
    public void LogWarning(string message) => logger.Warning(message);

    /// <inheritdoc />
    public void LogWarning(Exception? exception, string message) => logger.Warning(exception, message);

    /// <inheritdoc />
    public void LogError(string message) => logger.Error(message);

    /// <inheritdoc />
    public void LogError(Exception? exception, string message) => logger.Error(exception, message);

    /// <inheritdoc />
    public void LogFatal(string message) => logger.Fatal(message);

    /// <inheritdoc />
    public void LogFatal(Exception? exception, string message) => logger.Fatal(exception, message);

    /// <summary>
    /// Releases the resources used by the <see cref="SerilogLogger" />.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Releases the unmanaged resources used by the <see cref="SerilogLogger" />
    /// and optionally releases the managed resources.
    /// </summary>
    /// <param name="disposing">A value indicating whether to release managed resources.</param>
    private void Dispose(bool disposing)
    {
        if (!_isDisposed)
        {
            if (disposing)
            {
                logger.Dispose();
            }

            _isDisposed = true;
        }
    }
}

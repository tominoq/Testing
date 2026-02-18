namespace UI.Template.Framework.Logging;

/// <summary>
/// Provides a logging functionality.
/// </summary>
public interface ILogger : IDisposable
{
    /// <summary>
    /// Gets all files from the <see cref="LogPath" /> directory.
    /// </summary>
    IReadOnlyCollection<string> AllLogs { get; }

    /// <summary>
    /// Gets the path to the directory where logs should be stored.
    /// </summary>
    string LogPath { get; }

    /// <summary>
    /// Writes a log event with the Verbose level.
    /// </summary>
    /// <param name="message">The message describing the event.</param>
    void LogVerbose(string message);

    /// <summary>
    /// Writes a log event with the Verbose level and associated exception.
    /// </summary>
    /// <param name="exception">The exception related to the event.</param>
    /// <param name="message">The message describing the event.</param>
    void LogVerbose(Exception? exception, string message);

    /// <summary>
    /// Writes a log event with the Debug level.
    /// </summary>
    /// <param name="message">The message describing the event.</param>
    void LogDebug(string message);

    /// <summary>
    /// Writes a log event with the Debug level and associated exception.
    /// </summary>
    /// <param name="exception">The exception related to the event.</param>
    /// <param name="message">The message describing the event.</param>
    void LogDebug(Exception? exception, string message);

    /// <summary>
    /// Writes a log event with the Information level.
    /// </summary>
    /// <param name="message">The message describing the event.</param>
    void LogInformation(string message);

    /// <summary>
    /// Writes a log event with the Information level and associated exception.
    /// </summary>
    /// <param name="exception">The exception related to the event.</param>
    /// <param name="message">The message describing the event.</param>
    void LogInformation(Exception? exception, string message);

    /// <summary>
    /// Writes a log event with the Warning level.
    /// </summary>
    /// <param name="message">The message describing the event.</param>
    void LogWarning(string message);

    /// <summary>
    /// Writes a log event with the Warning level and associated exception.
    /// </summary>
    /// <param name="exception">The exception related to the event.</param>
    /// <param name="message">The message describing the event.</param>
    void LogWarning(Exception? exception, string message);

    /// <summary>
    /// Writes a log event with the Error level.
    /// </summary>
    /// <param name="message">The message describing the event.</param>
    void LogError(string message);

    /// <summary>
    /// Writes a log event with the Error level and associated exception.
    /// </summary>
    /// <param name="exception">The exception related to the event.</param>
    /// <param name="message">The message describing the event.</param>
    void LogError(Exception? exception, string message);

    /// <summary>
    /// Writes a log event with the Fatal level.
    /// </summary>
    /// <param name="message">The message describing the event.</param>
    void LogFatal(string message);

    /// <summary>
    /// Writes a log event with the Fatal level and associated exception.
    /// </summary>
    /// <param name="exception">The exception related to the event.</param>
    /// <param name="message">The message describing the event.</param>
    void LogFatal(Exception? exception, string message);
}

using System.Globalization;
using Microsoft.Extensions.Configuration;
using Serilog;
using UI.Template.Framework.Configurations;
using UI.Template.NUnit;

namespace UI.Template.Framework.Logging;

/// <summary>
/// Factory for creating instances of <see cref="ILogger" />.
/// </summary>
public static class LogFactory
{
    private const string TestClassLoggerName = "TestClassLogger";
    private const string TestMethodLoggerName = "TestMethodLogger";
    private const string TestProjectLoggerName = "TestProjectLogger";

    private static readonly CallerLogEnricher _callerEnricher = new();
    private static readonly TestClassLogEnricher _testClassEnricher = new();
    private static readonly TestMethodLogEnricher _testMethodEnricher = new();

    private static readonly string _separator = new('#', 100);

    /// <summary>
    /// Used for safely working with static properties in multi-threading environment.
    /// </summary>
    private static readonly object _lock = new();

    static LogFactory()
    {
        DateTime currentLocalDateTime = DateTime.Now;
        string date = currentLocalDateTime.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
        string time = currentLocalDateTime.ToString("HH-mm-ss", CultureInfo.InvariantCulture);
        LogDirectory = Path.Combine($"{TestConfiguration.LogsPath}", date, $"run_at_{time}");
    }

    /// <summary>
    /// Gets the root directory path where logs should be stored.
    /// </summary>
    public static string LogDirectory { get; }

    /// <summary>
    /// Gets the current <see cref="ILogger" /> instance.
    /// </summary>
    public static ILogger Logger => InitializedLoggers[LoggerKey];

    /// <summary>
    /// Gets the logger key of the current scope (test project/class/method).
    /// </summary>
    public static string LoggerKey => TestInfo.FullName + (TestInfo.Name is null ? null : "_" + TestInfo.Id);

    /// <summary>
    /// Gets the initialized <see cref="ILogger" /> instances.
    /// </summary>
    public static Dictionary<string, ILogger> InitializedLoggers { get; } = [];

    /// <summary>
    /// Closes the logger.
    /// </summary>
    /// <param name="loggerName">The name of the logger to close.</param>
    public static void CloseLogger(string? loggerName = null)
    {
        lock (_lock)
        {
            var loggerKey = loggerName ?? LoggerKey;
            var logger = InitializedLoggers[loggerKey];
            logger.LogInformation("Closing the logger...");
            logger.LogInformation(_separator);
            logger.Dispose();
            InitializedLoggers.Remove(loggerKey);
        }
    }

    /// <summary>
    /// Initializes a logger.
    /// </summary>
    /// <param name="loggerConfiguration">The configuration for the logger.</param>
    /// <param name="logPath">The path where the log files will be stored.</param>
    /// <param name="loggerName">The name of the logger.</param>
    public static void InitializeLogger(IConfiguration loggerConfiguration, string logPath, string? loggerName = null)
    {
        lock (_lock)
        {
            var loggerKey = loggerName ?? LoggerKey;
            if (!InitializedLoggers.ContainsKey(loggerKey))
            {
                var logger = new SerilogLogger(
                    new LoggerConfiguration().ReadFrom.Configuration(loggerConfiguration)
                                             .Enrich.With(_callerEnricher)
                                             .Enrich.With(_testClassEnricher)
                                             .Enrich.With(_testMethodEnricher)
                                             .CreateLogger(),
                    logPath
                );
                InitializedLoggers.Add(loggerKey, logger);
                logger.LogInformation(_separator);
            }
        }
    }

    /// <summary>
    /// Initializes a logger for the current test class.
    /// </summary>
    public static void InitializeTestClassLogger()
    {
        var configuration = Globals.GetConfiguration();
        configuration.RemoveLogger(TestMethodLoggerName);
        configuration.RemoveLogger(TestProjectLoggerName);

        var logPath = Path.Combine(LogDirectory, $"{TestInfo.ClassName}");
        configuration.SetLogPaths(TestClassLoggerName, logPath);

        InitializeLogger(configuration, logPath);
    }

    /// <summary>
    /// Initializes a logger for the current test method.
    /// </summary>
    public static void InitializeTestMethodLogger()
    {
        var configuration = Globals.GetConfiguration();
        configuration.RemoveLogger(TestClassLoggerName);
        configuration.RemoveLogger(TestProjectLoggerName);

        var logPath = Path.Combine(LogDirectory, $"{TestInfo.ClassName}", $"{TestInfo.Name}_{TestInfo.Id}");
        configuration.SetLogPaths(TestMethodLoggerName, logPath);

        InitializeLogger(configuration, logPath);
    }

    /// <summary>
    /// Initializes a logger for the current test project.
    /// </summary>
    public static void InitializeTestProjectLogger()
    {
        var configuration = Globals.GetConfiguration();
        configuration.RemoveLogger(TestClassLoggerName);
        configuration.RemoveLogger(TestMethodLoggerName);

        var logPath = LogDirectory;
        configuration.SetLogPaths(TestProjectLoggerName, logPath);

        InitializeLogger(configuration, logPath);
    }
}

using Microsoft.Extensions.Configuration;

namespace UI.Template.Framework.Configurations;

/// <summary>
/// Provides a set of extension methods for the <see cref="IConfiguration" /> interface.
/// </summary>
public static class IConfigurationExtensions
{
    /// <summary>
    /// Removes the specified logger configuration from the Serilog configuration.
    /// </summary>
    /// <param name="configuration">The configuration to use.</param>
    /// <param name="loggerName">The name of the logger to remove.</param>
    public static void RemoveLogger(this IConfiguration configuration, string loggerName)
    {
        ArgumentNullException.ThrowIfNull(configuration);
        ArgumentNullException.ThrowIfNull(loggerName);

        var section = configuration.GetSection($"Serilog:WriteTo:{loggerName}");

        if (section.Exists())
        {
            section.Value = string.Empty; // "null" does not work here
        }
    }

    /// <summary>
    /// Sets file paths for the File sinks of the specified logger.
    /// </summary>
    /// <param name="configuration">The configuration to use.</param>
    /// <param name="loggerName">The name of the logger.</param>
    /// <param name="path">The base path for the log files.</param>
    public static void SetLogPaths(this IConfiguration configuration, string loggerName, string path)
    {
        ArgumentNullException.ThrowIfNull(configuration);
        ArgumentNullException.ThrowIfNull(loggerName);

        var section = configuration.GetSection($"Serilog:WriteTo:{loggerName}:Args:configureLogger:WriteTo");

        if (section.Exists())
        {
            foreach (var child in section.GetChildren())
            {
                if (child.GetValue<string>("Name") != "File")
                {
                    continue;
                }

                var arguments = child.GetSection("Args");

                if (arguments.Exists())
                {
                    var logLevel = arguments.GetValue<string>("restrictedToMinimumLevel")?.ToLowerInvariant() ?? "verbose";
                    var logPath = arguments.GetSection("path");
                    var logScope = loggerName.Replace("Test", string.Empty, StringComparison.InvariantCulture)
                                             .Replace("Logger", string.Empty, StringComparison.InvariantCulture)
                                             .ToLowerInvariant();

                    logPath.Value = Path.Combine(path, $"log-{logScope}-{logLevel}.log");
                }
            }
        }
    }
}

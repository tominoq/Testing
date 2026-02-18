using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using Serilog.Core;
using Serilog.Events;

namespace UI.Template.Framework.Logging;

/// <summary>
/// Enriches the logger with the caller method name.
/// </summary>
public sealed class CallerLogEnricher : ILogEventEnricher
{
    /// <inheritdoc />
    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        ArgumentNullException.ThrowIfNull(logEvent);

        var loggerClassFound = false;
        var stackTrace = new StackTrace();

        foreach (var frame in stackTrace.GetFrames())
        {
            var method = frame.GetMethod();
            var declaringType = method?.DeclaringType;

            if (typeof(ILogger).IsAssignableFrom(declaringType))
            {
                loggerClassFound = true;
            }
            else if (loggerClassFound &&
                     declaringType is not null &&
                     !IsCompilerGenerated(declaringType) &&
                     !IsSystemType(declaringType))
            {
                var caller = $"{declaringType.Name}.{method!.Name}";
                logEvent.AddPropertyIfAbsent(new LogEventProperty("Caller", new ScalarValue(caller)));
                return;
            }
        }

        logEvent.AddPropertyIfAbsent(new LogEventProperty("Caller", new ScalarValue("<unknown caller>")));
    }

    private static bool IsCompilerGenerated(Type type)
    {
        return type.GetCustomAttribute<CompilerGeneratedAttribute>(false) is not null;
    }

    private static bool IsSystemType(Type type)
    {
        var namespaceRoot = type.Namespace?.Split(".")[0];
        var systemNamespaces = new[] { "System", "Microsoft" };

        return namespaceRoot is not null && systemNamespaces.Contains(namespaceRoot);
    }
}

using Serilog.Core;
using Serilog.Events;
using UI.Template.NUnit;

namespace UI.Template.Framework.Logging;

/// <summary>
/// Enriches the logger with the test method name. If the name is not defined
/// (e.g. in the scope of a test class), the name of the test class is used instead.
/// </summary>
public sealed class TestMethodLogEnricher : ILogEventEnricher
{
    /// <inheritdoc />
    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        ArgumentNullException.ThrowIfNull(logEvent);

        logEvent.AddPropertyIfAbsent(new LogEventProperty("TestMethod", new ScalarValue(TestInfo.Name ?? TestInfo.ClassName)));
    }
}

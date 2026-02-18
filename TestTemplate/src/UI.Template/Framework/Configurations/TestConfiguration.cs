using System.Drawing;
using System.Globalization;
using UI.Template.Framework.Helpers;

namespace UI.Template.Framework.Configurations;

/// <summary>
/// The configuration for the tests.
/// </summary>
public static class TestConfiguration
{
    /// <summary>
    /// Window size that is applied when <see cref="IsHeadless"/> is true and <see cref="WindowSize"/> is empty/not set.
    /// </summary>
    private const string DefaultWindowSize = "1920,1080";

    static TestConfiguration()
    {
        var section = Globals.Configuration.GetSection(nameof(TestConfiguration));
        LogsPath = section[nameof(LogsPath)];
        StoreLogsAlways = bool.Parse(section[nameof(StoreLogsAlways)] ?? "false");
        PageElementTimeout = Convert.ToUInt32(section[nameof(PageElementTimeout)], CultureInfo.InvariantCulture);
        PageLoadTimeout = Convert.ToUInt32(section[nameof(PageLoadTimeout)], CultureInfo.InvariantCulture);
        IsRemote = Convert.ToBoolean(section[nameof(IsRemote)], CultureInfo.InvariantCulture);
        WindowSize = Utils.ParseWindowSize(section[nameof(WindowSize)] ?? string.Empty);
        IsHeadless = Convert.ToBoolean(section[nameof(IsHeadless)], CultureInfo.InvariantCulture);
        SeleniumHubUrl = section[nameof(SeleniumHubUrl)] ?? string.Empty;

        if (IsHeadless && WindowSize.IsEmpty)
        {
            WindowSize = Utils.ParseWindowSize(DefaultWindowSize);
        }
    }

    /// <summary>
    /// Gets the path to the root directory for the logs.
    /// </summary>
    public static string? LogsPath { get; }

    /// <summary>
    /// Gets a value indicating whether to store logs always (true)
    /// or only when test fails (false).
    /// </summary>
    public static bool StoreLogsAlways { get; }

    /// <summary>
    /// Time (in seconds) telling us how long the program waits for a single element to be loaded.
    /// </summary>
    public static uint PageElementTimeout { get; private set; }

    /// <summary>
    /// Time (in seconds) telling us how long the program waits for a single page to be loaded.
    /// </summary>
    public static uint PageLoadTimeout { get; private set; }

    /// <summary>
    /// Dimensions of the browser window for testing.
    /// </summary>
    public static Size WindowSize { get; private set; }

    /// <summary>
    /// URL of the selenium hub on which the tests should run.
    /// </summary>
    public static string SeleniumHubUrl { get; private set; }

    /// <summary>
    /// Should the browser run in headless mode? True for yes, False for no.
    /// </summary>
    public static bool IsHeadless { get; private set; }

    /// <summary>
    /// Use local or remote driver. True for remote, False for local.
    /// </summary>
    public static bool IsRemote { get; private set; }
}

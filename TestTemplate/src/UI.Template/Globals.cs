using System.Reflection;
using Microsoft.Extensions.Configuration;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using UI.Template.Framework.Configurations;
using UI.Template.Framework.Extensions;
using UI.Template.Framework.Helpers;
using UI.Template.Framework.Logging;
using UI.Template.NUnit;

namespace UI.Template;

/// <summary>
/// The global properties and methods.
/// </summary>
public static class Globals
{
    /// <summary>
    /// Format of the current time to use in file attachments.
    /// </summary>
    public const string CurrentTimeFileNameFormat = "HH-mm-ss-fff";

    // Disabled CA2211 because of the nature of the class and the need for thread-specific values.
#pragma warning disable CA2211

    /// <summary>
    /// Identifies windows name that is opened when new <see cref="IWebDriver"/> instance is created.
    /// </summary>
    [ThreadStatic]
    private static string? _mainWindowName;

    /// <summary>
    /// Identifies windows name that is opened when new <see cref="IWebDriver"/> instance is created.
    /// </summary>
    [ThreadStatic]
    private static bool _isLogged;

#pragma warning restore CA2211

    static Globals()
    {
        Random = new Random();
    }

    /// <summary>
    /// Gets the application configuration.
    /// </summary>
    public static IConfiguration Configuration { get; } = GetConfiguration();

    /// <summary>
    /// Retrieves the application configuration.
    /// </summary>
    /// <returns>An <see cref="IConfiguration" /> instance containing the application settings.</returns>
    internal static IConfiguration GetConfiguration()
    {
        var buildConfiguration = typeof(Globals).Assembly.GetCustomAttribute<AssemblyConfigurationAttribute>()?.Configuration;

        return new ConfigurationBuilder().SetBasePath(AppContext.BaseDirectory)
                                         .AddJsonFile("appsettings.json", false)
                                         .AddJsonFile("appsettings.local.json", true)
                                         .AddJsonFile($"appsettings.{buildConfiguration}.json", true)
                                         .Build();
    }

    #region UI
    /// <summary>
    /// Gets the current <see cref="ILogger" /> instance.
    /// </summary>
    public static ILogger Logger => LogFactory.Logger;

    /// <summary>
    /// Global handel of the currently used <see cref="IWebDriver"/> object.
    /// </summary>
    public static IWebDriver WebDriver => ListOfInitializedWebdrivers[WebDriverKey];

    /// <summary>
    /// Holds the active sessions of <see cref="IWebDriver"/> objects, together with their identification key.
    /// </summary>
    public static Dictionary<string, IWebDriver> ListOfInitializedWebdrivers { get; } = [];

    /// <summary>
    /// Gets or sets the name of the main window that is opened when a new <see cref="IWebDriver"/> instance is created.
    /// This property is thread-specific; each thread has its own value due to the underlying <c>[ThreadStatic]</c> field.
    /// </summary>
    public static string MainWindowName
    {
        get => _mainWindowName ?? string.Empty;
        set => _mainWindowName = value;
    }

    /// <summary>
    /// Indicates whether the user is logged in.
    /// </summary>
    public static bool IsLogged
    {
        get => _isLogged;
        set => _isLogged = value;
    }

    /// <summary>
    /// Global handel of currently used <see cref="Actions"/> object.
    /// </summary>
    public static Actions Action => new(WebDriver);

    /// <summary>
    /// Global handel of currently used <see cref="IJavaScriptExecutor"/> object.
    /// </summary>
    public static IJavaScriptExecutor JavaScriptExecutor => (IJavaScriptExecutor)WebDriver;

    /// <summary>
    /// Combination of the current test class name and worker ID to create a unique key for webdriver reference belonging to the test.
    /// </summary>
    public static string WebDriverKey => TestInfo.ClassName + TestInfo.WorkerName;

    /// <summary>
    /// Id of the webdriver session.
    /// </summary>
    public static string? WebDriverSessionId => WebDriver.GetSessionId();
    #endregion UI

    #region Others
    /// <summary>
    /// Global handel of currently used <see cref="WebDriverWait"/> object.
    /// </summary>
    public static WebDriverWait Wait => new(WebDriver, TimeSpan.FromSeconds(TestConfiguration.PageElementTimeout));

    /// <summary>
    /// Global handel of currently used <see cref="Random"/> object.
    /// </summary>
    public static Random Random { get; }

    /// <summary>
    /// Gets the solution directory information.
    /// This is typically the root directory of the solution containing the solution file.
    /// </summary>
    public static DirectoryInfo SolutionDir => Utils.GetSolutionDirectoryInfo();
    #endregion Others

    /// <summary>
    /// Gets comma separated list of string properties and their values.
    /// </summary>
    public static string GetStringPropertyValuesForLogging()
    {
        return $"\n\t{nameof(TestInfo.Id)}: {TestInfo.Id}," +
               $"\n\t{nameof(TestInfo.FullName)}: {TestInfo.FullName}," +
               $"\n\t{nameof(TestInfo.Name)}: {TestInfo.Name}," +
               $"\n\t{nameof(TestInfo.WorkerName)}: {TestInfo.WorkerName}," +
               $"\n\t{nameof(TestInfo.ClassName)}: {TestInfo.ClassName}," +
               $"\n\t{nameof(WebDriverKey)}: {WebDriverKey}";
    }

    public static bool IsReady() => Globals.WebDriver.IsPageReady();

    public static void WaitForReady()
    {
        const string jScriptCommand = "return !window.ngBusy";
        var wait = new WebDriverWait(Globals.WebDriver, TimeSpan.FromSeconds(TestConfiguration.PageLoadTimeout));
        wait.SetTimeoutMessage($"Page '{Globals.WebDriver.Url}' wasn't completely ready during the timeout, because javascript command '{jScriptCommand}' was still returning false value.")
            .Until(_ => IsReady());
    }

    /// <summary>
    /// Refreshes the page
    /// </summary>
    public static void Refresh()
    {
        Globals.WebDriver.Navigate().Refresh();
    }

    /// <summary>
    /// Method returns unique WebDriver session ID
    /// </summary>
    /// <param name="webDriver"><see cref="IWebDriver"/></param>
    /// <returns>Session ID</returns>
    public static string? GetSessionId(IWebDriver webDriver)
    {
        return ((WebDriver)webDriver)?.SessionId.ToString();
    }

    /// <summary>
    /// Method sends request to the node to refresh connection timeout.
    /// If no request would be sent to node, hub can close the session before the wait (usually) cycle is finished.
    /// </summary>
    /// <param name="webDriver"><see cref="WebDriver"/></param>
    public static void Ping(IWebDriver? webDriver = null)
    {
        _ = (webDriver ?? Globals.WebDriver).CurrentWindowHandle;
    }

    /// <summary>
    /// Creates Selenium By locator based on project specific Id
    /// </summary>
    /// <param name="id">Target element id</param>
    /// <returns>By locator</returns>
    public static By GetCustomLocator(string id)
    {
        return By.CssSelector($"[data-testid='{id}']");
    }

    /// <summary>
    /// Method checks if there is any error on the page source, browser logs or any other logs.
    /// Depends on the implementation in specific project.
    /// </summary>
    /// <param name="webDriver"><see cref="IWebDriver"/></param>
    public static void CheckForErrors(IWebDriver webDriver)
    {
        if (webDriver.IsWebInErrorState())
            throw new InvalidOperationException($"Fatal HTTP error has been detected on the page '{webDriver.Url}'! Please check attachments for details.");
        else if (webDriver.IsPageNotFound())
            throw new InvalidOperationException($"HTTP 404 error has been detected on the page '{webDriver.Url}'! Please check attachments for details.");
    }
}

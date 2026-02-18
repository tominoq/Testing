using System.Drawing;
using System.Reflection;
using OpenQA.Selenium;
using OpenQA.Selenium.DevTools;
using OpenQA.Selenium.DevTools.V136.Network;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.UI;
using UI.Template.Framework.Configurations;
using UI.Template.Framework.Enums;
using UI.Template.Framework.Exceptions;
using UI.Template.Framework.Helpers;
using UI.Template.Framework.Interfaces;
using SeleniumLogEntry = OpenQA.Selenium.LogEntry; // ambiguous with OpenQA.Selenium.DevTools

namespace UI.Template.Framework.Extensions;

/// <summary>
/// Commonly used extension methods for working with <see cref="IWebDriver"/> object.
/// </summary>
public static class IWebDriverExtensions
{
    /// <summary>
    /// Creates screenshot from page.
    /// </summary>
    /// <param name="webDriver"><see cref="IWebDriver"/></param>
    /// <param name="fileName">Screenshot name</param>
    /// <returns>Path to screenshot file</returns>
    public static string PrintScreen(this IWebDriver webDriver, string fileName)
    {
        try
        {
            var screenshot = ((ITakesScreenshot)webDriver).GetScreenshot();
            screenshot.SaveAsFile(fileName);
        }
        catch (WebDriverException)
        {
            return string.Empty;
        }
        return fileName;
    }

    /// <summary>
    /// Method checks if the application is ready and not active.
    /// </summary>
    /// <param name="_"><see cref="IWebDriver"/></param>
    /// <returns>True if the condition in the <see cref="IReady.IsReady"/> is fulfilled, otherwise false</returns>
    public static bool IsReady(this IWebDriver _)
    {
        return Globals.IsReady();
    }

    /// <summary>
    /// Waits until application is ready and not active.
    /// </summary>
    /// <param name="_"><see cref="IWebDriver"/></param>
    public static void WaitForReady(this IWebDriver _)
    {
        Globals.WaitForReady();
    }

    /// <summary>
    /// Waits until URL has changed.
    /// </summary>
    /// <param name="webDriver"><see cref="IWebDriver"/></param>
    /// <param name="url">Original url to be waiting for change</param>
    public static void WaitForUrlChanged(this IWebDriver webDriver, string url)
    {
        Globals.Logger.LogVerbose($"Waiting for url is changed from '{url}'");

        var wait = new WebDriverWait(webDriver, TimeSpan.FromSeconds(TestConfiguration.PageLoadTimeout));
        wait.SetTimeoutMessage($"Url wasn't changed from '{url}' during the timeout.");

        if (url.StartsWith("http", StringComparison.OrdinalIgnoreCase))
            wait.Until(driver => url != driver.Url);
        else
            wait.Until(driver => url != driver.UrlPathAndQuery());

        webDriver.WaitForReady();
        webDriver.CheckForErrors();

        Globals.Logger.LogVerbose($"Url was changed to '{webDriver.Url}'");
    }

    /// <summary>
    /// Waits until URL has changed after specified action.
    /// </summary>
    /// <param name="webDriver"><see cref="IWebDriver"/></param>
    /// <param name="action"><see cref="Action"/> to be invoked before url changing</param>
    public static void WaitForUrlChanged(this IWebDriver webDriver, Action action)
    {
        // store URL before action
        var url = webDriver.Url;
        action.Invoke();
        webDriver.WaitForUrlChanged(url);
    }

    /// <summary>
    /// Waits until URL contains exact string (case sensitive).
    /// </summary>
    /// <param name="webDriver"><see cref="IWebDriver"/></param>
    /// <param name="containedString">String that has to be part of the url</param>
    public static void WaitForUrlContains(this IWebDriver webDriver, string containedString)
    {
        var wait = new WebDriverWait(webDriver, TimeSpan.FromSeconds(TestConfiguration.PageLoadTimeout));
        wait.SetTimeoutMessage($"'{nameof(containedString)}' with value '{containedString}' wasn't contained in url during the timeout.")
            .Until(driver => driver.Url.Contains(containedString));
    }

    /// <summary>
    /// Waits until URL contains a string according to <paramref name="stringComparison"/>.
    /// </summary>
    /// <param name="webDriver"><see cref="IWebDriver"/></param>
    /// <param name="containedString">String that has to be part of the url</param>
    /// <param name="stringComparison"><see cref="StringComparison"/></param>
    public static void WaitForUrlContains(this IWebDriver webDriver, string containedString, StringComparison stringComparison)
    {
        var wait = new WebDriverWait(webDriver, TimeSpan.FromSeconds(TestConfiguration.PageLoadTimeout));
        wait.SetTimeoutMessage($"'{nameof(containedString)}' with value '{containedString}' wasn't contained in url during the timeout.")
            .Until(driver => driver.Url.Contains(containedString, stringComparison));
    }

    /// <summary>
    /// Scrolls to top of the HTML document.
    /// </summary>
    /// https://developer.mozilla.org/en-US/docs/Web/API/Window/scrollTo
    /// <param name="webDriver"><see cref="IWebDriver"/></param>
    public static void ScrollTop(this IWebDriver webDriver)
    {
        ((IJavaScriptExecutor)webDriver).ExecuteScript("window.scrollTo(0, 0);");
    }

    /// <summary>
    /// Returns Url path without query and fragment.
    /// </summary>
    /// <param name="webDriver"><see cref="IWebDriver"/></param>
    /// <returns>string</returns>
    public static string UrlPath(this IWebDriver webDriver)
    {
        Uri url = new(webDriver.Url);
        return url.AbsolutePath;
    }

    /// <summary>
    /// Returns Url path and query including fragment.
    /// </summary>
    /// <param name="webDriver"><see cref="IWebDriver"/></param>
    /// <returns>string</returns>
    public static string? UrlPathAndQuery(this IWebDriver webDriver)
    {
        Uri url = new(webDriver.Url);
        if (url.AbsolutePath == ",")
            return null;
        else
            return url.PathAndQuery + url.Fragment;
    }

    /// <summary>
    /// Method saves browser logs.
    /// </summary>
    /// <param name="webDriver"><see cref="IWebDriver"/></param>
    /// <param name="optionalFileNameSuffix">File name with the logs can contain suffix with additional information</param>
    public static void SaveBrowserLogs(this IWebDriver webDriver, string optionalFileNameSuffix = "")
        => webDriver.SaveLogs(WebDriverLog.Browser, optionalFileNameSuffix);

    /// <summary>
    /// Method saves logs to the file and adds them to the test context.
    /// </summary>
    /// <param name="webDriver"><see cref="IWebDriver"/></param>
    /// <param name="logType"><see cref="WebDriverLog"/></param>
    /// <param name="optionalFileNameSuffix">File name with the logs can contain suffix with additional information</param>
    public static void SaveLogs(this IWebDriver webDriver, WebDriverLog logType, string optionalFileNameSuffix = "")
    {
        try
        {
            optionalFileNameSuffix += "_" + Utils.GetCurrentDateTime(Globals.CurrentTimeFileNameFormat);
            string filePath = Utils.SaveAsFile(webDriver.GetLogs(logType),
                                                 Utils.GetFilePath(Globals.Logger.LogPath, $"{logType}", "log", optionalFileNameSuffix));

            if (string.IsNullOrEmpty(filePath))
                return;

            Globals.Logger.LogInformation($"Saving '{logType}' logs to '{filePath}'");
        }
        catch (Exception e)
        {
            Globals.Logger.LogFatal(e, $"Cannot save '{logType}' logs.");
        }
    }

    /// <summary>
    /// Method returns logs of the current session based on the log type.
    /// </summary>
    /// <param name="webDriver"><see cref="IWebDriver"/></param>
    /// <param name="logType"><see cref="WebDriverLog"/>e</param>
    /// <returns><see cref="IEnumerable{SeleniumLogEntry}"/></returns>
    public static string GetLogs(this IWebDriver webDriver, WebDriverLog logType)
        => string.Join(Environment.NewLine, webDriver.GetLogs(logType.GetName()) ?? Enumerable.Empty<SeleniumLogEntry>());

    /// <summary>
    /// Method returns logs of the current session based on the log type.
    /// </summary>
    /// <param name="webDriver"><see cref="IWebDriver"/></param>
    /// <param name="logType">Log type</param>
    /// <returns><see cref="IEnumerable{SeleniumLogEntry}"/></returns>
    public static IEnumerable<SeleniumLogEntry>? GetLogs(this IWebDriver webDriver, string logType)
    {
        if (webDriver is not ISupportsLogs)
            return [];

        // workaround for bug https://github.com/SeleniumHQ/selenium/issues/8229
        var commandExecutorProperty = typeof(RemoteWebDriver).GetProperty("CommandExecutor", BindingFlags.Public | BindingFlags.Instance);
        if (commandExecutorProperty == null)
            return null;

        if (commandExecutorProperty.GetValue(webDriver) is not ICommandExecutor commandExecutor)
            return null;

        commandExecutor.TryAddCommand(DriverCommand.GetAvailableLogTypes, new HttpCommandInfo(HttpCommandInfo.GetCommand, "/session/{sessionId}/se/log/types"));
        commandExecutor.TryAddCommand(DriverCommand.GetLog, new HttpCommandInfo(HttpCommandInfo.PostCommand, "/session/{sessionId}/se/log"));

        var response = commandExecutor.Execute(new Command(((IHasSessionId)webDriver).SessionId, DriverCommand.GetLog, new Dictionary<string, object?> { ["type"] = logType }));

        if (response == null || response.Status != WebDriverResult.Success || response.Value == null)
        {
            Globals.Logger.LogError($"Cannot get logs for the log type '{logType}'{Environment.NewLine}");
            if (response?.Value is Dictionary<string, object> logDict)
            {
                foreach (KeyValuePair<string, object> logRecord in logDict)
                    Globals.Logger.LogError($"Message type: {logRecord.Key}; message content: {logRecord.Value}");
            }
            return null;
        }

        var fromDictionaryMethod = typeof(SeleniumLogEntry).GetMethod("FromDictionary", BindingFlags.Static | BindingFlags.NonPublic);
        if (fromDictionaryMethod == null)
            return null;

        if (response.Value is not IEnumerable<object> responseEnumerable)
            return null;

        return responseEnumerable.Select(v => fromDictionaryMethod.Invoke(null, new object?[] { v }))
                                .OfType<SeleniumLogEntry>()
                                .ToList();
    }

    /// <summary>
    /// Save a web page source
    /// </summary>
    /// <param name="webDriver"><see cref="IWebDriver"/></param>
    /// <param name="optionalFileNameSuffix">File name of the page source can contain suffix with additional information</param>
    public static void SavePageSource(this IWebDriver webDriver, string optionalFileNameSuffix = "")
    {
        try
        {
            if (webDriver.PageSource == null)
                return;

            string pageSource = webDriver.PageSource;
            optionalFileNameSuffix += "_" + Utils.GetCurrentDateTime(Globals.CurrentTimeFileNameFormat);
            var filePath = Utils.SaveAsFile(pageSource, Utils.GetFilePath(Globals.Logger.LogPath, "page", "html", optionalFileNameSuffix));

            if (string.IsNullOrEmpty(filePath))
                return;

            Globals.Logger.LogInformation($"Saving page source to: '{filePath}'");
        }
        catch (Exception e)
        {
            Globals.Logger.LogFatal(e, "Cannot download the page source.");
        }
    }

    /// <summary>
    /// Take a screenshot of the current page
    /// </summary>
    /// <param name="webDriver"><see cref="IWebDriver"/></param>
    /// <param name="optionalFileNameSuffix">File name of the screenshot can contain suffix with additional information</param>
    public static void TakeScreenshot(this IWebDriver webDriver, string optionalFileNameSuffix = "")
    {
        try
        {
            optionalFileNameSuffix += "_" + Utils.GetCurrentDateTime(Globals.CurrentTimeFileNameFormat);
            var filePath = webDriver.PrintScreen(Utils.GetFilePath(Globals.Logger.LogPath, "screenshot", "png", optionalFileNameSuffix));

            if (string.IsNullOrEmpty(filePath))
                return;
            Globals.Logger.LogInformation($"Saving screenshot to: '{filePath}'");
        }
        catch (Exception e)
        {
            Globals.Logger.LogFatal(e, "Cannot make a screenshot of the page.");
        }
    }

    /// <summary>
    /// Method returns WebDriver session ID
    /// </summary>
    /// <param name="webDriver"><see cref="IWebDriver"/></param>
    /// <returns>Session ID</returns>
    public static string? GetSessionId(this IWebDriver webDriver)
    {
        return Globals.GetSessionId(webDriver);
    }

    /// <summary>
    /// Method sends request to the node to refresh connection timeout.
    /// If no request would be sent to node, hub can close the session before the wait (usually) cycle is finished.
    /// </summary>
    /// <param name="webDriver"><see cref="WebDriver"/></param>
    public static void Ping(this IWebDriver webDriver)
    {
        Globals.Ping(webDriver);
    }

    /// <summary>
    /// Method switches to the new opened tab/window.
    /// Method expects that there is one main tab/window before invocation method that opens new tab/window.
    /// </summary>
    /// <param name="webDriver"><see cref="WebDriver"/></param>
    /// <param name="wait"><see cref="WebDriverWait"/></param>
    /// <param name="action">Action/method that opens new tab/window</param>
    public static void OpenAndSwitchToNewTab(this IWebDriver webDriver, WebDriverWait wait, Action action)
    {
        if (webDriver.WindowHandles.Count != 1)
            throw new UnexpectedCountException("There is unexpected count of opened tabs/windows. There has to be only one tab/window.");

        string originalName = webDriver.CurrentWindowHandle;
        action.Invoke();

        wait.SetTimeoutMessage("Count of window handles was different from 2.")
            .Until(_ => webDriver.WindowHandles.Count == 2);
        foreach (var name in webDriver.WindowHandles)
        {
            if (name != originalName)
            {
                webDriver.SwitchToTab(name);
                return;
            }
        }
    }

    /// <summary>
    /// Switches focus to an opened tab by index.
    /// </summary>
    /// <param name="webDriver">current <see cref="WebDriver"/></param>
    /// <param name="tabIndex">index of the tab to be switched to</param>
    /// <returns>True if the switching to the tab was successful, false if there is no tab with <paramref name="tabIndex"/></returns>
    public static bool TrySwitchToTab(this IWebDriver webDriver, int tabIndex)
    {
        var tabCount = webDriver.WindowHandles.Count;
        if (tabIndex <= tabCount && tabIndex >= 0)
        {
            webDriver.SwitchTo().Window(webDriver.WindowHandles[tabIndex]);
            return true;
        }
        return false;
    }

    /// <summary>
    /// Method switches to the tab/window with <paramref name="name"/>.
    /// </summary>
    /// <param name="webDriver"><see cref="WebDriver"/></param>
    /// <param name="name">Name of the tab/window to be switched to</param>
    /// <exception cref="Exception">if there is no such tab/window</exception>
    public static void SwitchToTab(this IWebDriver webDriver, string name)
    {
        if (webDriver.WindowHandles.Contains(name))
            webDriver.SwitchTo().Window(name);
        else
            throw new NoSuchWindowException($"Cannot find window name '{name}'. Please check if the tab/window wasn't closed accidentally.");
    }

    /// <summary>
    /// Open and focus on a new tab.
    /// </summary>
    /// <param name="webDriver">current <see cref="WebDriver"/></param>
    public static void NewTab(this IWebDriver webDriver)
    {
        webDriver.SwitchTo().NewWindow(WindowType.Tab);
    }

    /// <summary>
    /// Open and focus on a new window.
    /// </summary>
    /// <param name="webDriver">current <see cref="WebDriver"/></param>
    public static void NewWindow(this IWebDriver webDriver)
    {
        webDriver.SwitchTo().NewWindow(WindowType.Window);
    }

    /// <summary>
    /// Open url in the new tab.
    /// </summary>
    /// <param name="webDriver">Current <see cref="WebDriver"/></param>
    /// <param name="url">Url to be opened in the new tab</param>
    public static void OpenUrlInNewTab(this IWebDriver webDriver, Uri url)
    {
        webDriver.NewTab();
        webDriver.Navigate().GoToUrl(url);
    }

    /// <summary>
    /// Open url in the new window.
    /// </summary>
    /// <param name="webDriver">Current <see cref="WebDriver"/></param>
    /// <param name="url">Url to be opened in the new window</param>
    public static void OpenUrlInNewWindow(this IWebDriver webDriver, Uri url)
    {
        webDriver.NewWindow();
        webDriver.Navigate().GoToUrl(url);
    }

    /// <summary>
    /// Closes an opened tab by index.
    /// </summary>
    /// <param name="webDriver">current <see cref="WebDriver"/></param>
    /// <param name="tabIndex">index of the tab to be closed</param>
    /// <param name="returnToTab">index of the tab to be returned to (0 = first tab by default)</param>
    /// <returns>True if the tab isn't present (closed or already close before), otherwise false</returns>
    public static bool CloseTab(this IWebDriver webDriver, int tabIndex, int returnToTab = 0)
    {
        if (webDriver.TrySwitchToTab(tabIndex))
        {
            webDriver.Close();
            webDriver.TrySwitchToTab(returnToTab);
            return true;
        }
        return false;
    }

    /// <summary>
    /// Closes currently focused tab.
    /// </summary>
    /// <param name="webDriver">current <see cref="WebDriver"/></param>
    /// <param name="returnToTab">index of the tab to be returned to (0 = first tab by default)</param>
    /// <returns>True if the tab isn't present (closed or already close before), otherwise false</returns>
    public static bool CloseCurrentTab(this IWebDriver webDriver, int returnToTab = 0)
    {
        webDriver.Close();
        return webDriver.TrySwitchToTab(returnToTab);
    }

    /// <summary>
    /// Method returns size of the opened window.
    /// </summary>
    /// <param name="webDriver"><see cref="WebDriver"/></param>
    /// <returns><see cref="Size"/></returns>
    public static Size GetWindowSize(this IWebDriver webDriver)
    {
        return webDriver.Manage().Window.Size;
    }

    /// <summary>
    /// Clears the Chrome browser cache using the dev tools
    /// more info: https://www.selenium.dev/documentation/webdriver/bidirectional/chrome_devtools/
    /// </summary>
    public static void ClearChromeCache(this IWebDriver webDriver)
    {
        IDevTools devTools = webDriver as IDevTools ?? throw new NotSupportedException("The current WebDriver does not support DevTools protocol.");
        DevToolsSession session = devTools.GetDevToolsSession();
        session.SendCommand(new ClearBrowserCacheCommandSettings());
    }

    /// <summary>
    /// Function checks if there is displayed page not found error 404
    /// </summary>
    /// <param name="webDriver"><see cref="IWebDriver"/></param>
    public static bool IsPageNotFound(this IWebDriver webDriver)
    {
        return webDriver.Title.Contains("Page Not Found", StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Method returns state of the page/window to be done loading
    /// </summary>
    /// <param name="webDriver"><see cref="IWebDriver"/></param>
    /// <returns>True if no page action are executed, false if there are actions that page executes (e.g. api calling, rendering)</returns>
    public static bool IsPageReady(this IWebDriver webDriver)
    {
        const string jScriptCommand = "return !window.ngBusy";
        bool isPageReady = (bool)(((IJavaScriptExecutor)webDriver)?.ExecuteScript(jScriptCommand) ?? false);
        Globals.Logger.LogVerbose($"IsPageReady result is '{isPageReady}'");
        return isPageReady;
    }

    /// <summary>
    /// Function checks if there is displayed error page 500 or there is something failed in browser logs.
    /// </summary>
    /// <param name="webDriver"><see cref="IWebDriver"/></param>
    /// <returns>True if any 5** error is detected, otherwise false</returns>
    public static bool IsWebInErrorState(this IWebDriver webDriver)
    {
        string pageTitle = webDriver.Title;
        bool isErrorInPageTitle = pageTitle.StartsWith("Error", StringComparison.OrdinalIgnoreCase);

        string pageSource = webDriver.PageSource;
        bool isErrorInPageSource = pageSource.Contains("HTTP ERROR") ||
                                   pageSource.Contains("This site can’t be reached", StringComparison.OrdinalIgnoreCase) ||
                                   pageSource.Contains("Bad gateway", StringComparison.OrdinalIgnoreCase);

        IEnumerable<OpenQA.Selenium.LogEntry> browserLogs = webDriver.GetLogs(WebDriverLog.Browser.GetName()) ?? new List<OpenQA.Selenium.LogEntry>();
        bool isErrorInBrowserLogs = browserLogs.Any(log => log.Level is LogLevel.Severe &&
                                                           (log.Message.Contains("Failed to load resource", StringComparison.OrdinalIgnoreCase) ||
                                                           log.Message.Contains("Request failed", StringComparison.OrdinalIgnoreCase)) &&
                                                           log.Message.Contains("status", StringComparison.OrdinalIgnoreCase));

        if (isErrorInPageTitle || isErrorInPageSource || isErrorInBrowserLogs)
        {
            Globals.Logger.LogError("Title of the page: " + pageTitle);
            Globals.Logger.LogError("Source of the page: " + Environment.NewLine + pageSource);
            Globals.Logger.LogError("Browser logs: " + Environment.NewLine + string.Join('\n', browserLogs));
            return true;
        }
        return false;
    }

    /// <summary>
    /// Method checks if there is any 5** error in the page source or in browser logs.
    /// </summary>
    /// <param name="webDriver"><see cref="IWebDriver"/></param>
    public static void CheckForErrors(this IWebDriver webDriver)
    {
        Globals.CheckForErrors(webDriver);
    }

    /// <summary>
    /// Method sets window size of the browser.
    /// </summary>
    /// <param name="webDriver"><see cref="IWebDriver"/></param>
    /// <param name="size"><see cref="Size"/> that can be also empty. In this case is window set maximized.</param>
    public static void SetWindowSize(this IWebDriver webDriver, Size size)
    {
        if (size.IsEmpty)
            webDriver.Manage().Window.Maximize();
        else
            webDriver.Manage().Window.Size = size;
    }
}

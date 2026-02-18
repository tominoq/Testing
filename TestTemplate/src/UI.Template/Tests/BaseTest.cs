using System.Text;
using NUnit.Framework.Interfaces;
using OpenQA.Selenium;
using UI.Template.Framework.Configurations;
using UI.Template.Framework.Extensions;
using UI.Template.Framework.Factories;
using UI.Template.Framework.Interfaces;
using UI.Template.Framework.Logging;
using UI.Template.NUnit;

namespace UI.Template.Tests;

/// <summary>
/// Common logic, including SetUp and TearDown methods, creating robust logging environment for the tests.
/// </summary>
public abstract class BaseTest : IBase
{
    /// <summary>
    /// Gets the current <see cref="ILogger" /> instance.
    /// </summary>
    protected static ILogger Logger => LogFactory.Logger;

    /// <summary>
    /// Used for safely working with static properties in multi-threading environment.
    /// </summary>
    protected static object Lock { get; } = new();

    /// <summary>
    /// Reference of the current WebDriver for easier usage.
    /// </summary>
    protected static IWebDriver WebDriver => Globals.WebDriver;

    /// <summary>
    /// Reference of failed test for easier usage.
    /// </summary>
    protected static bool IsTestFailed => TestInfo.Outcome == TestStatus.Failed;

    /// <summary>
    /// Logs all data necessary to error debug (like error message, stack trace, page source, screenshot, ....)
    /// </summary>
    protected virtual void LogErrorData()
    {
        try
        {
            // If the test FAILS take a screenshot to see what happened
            if (IsTestFailed)
            {
                WebDriver?.SavePageSource("error");
                WebDriver?.SaveBrowserLogs("error");
                WebDriver?.TakeScreenshot("error");
            }
        }
        catch (Exception e)
        {
            Logger.LogError(e, "Cannot store error data like screenshot or page source. Please check linked error.");
        }
    }

    /// <summary>
    /// Initializes the driver if not already initialized.
    /// </summary>
    protected virtual void InitializeDriver()
    {
        lock (Lock)
        {
            if (!Globals.ListOfInitializedWebdrivers.ContainsKey(Globals.WebDriverKey))
                Globals.ListOfInitializedWebdrivers.Add(Globals.WebDriverKey, WebDriverFactory.InitializeWebDriver());
        }
    }

    /// <summary>
    /// Closes the driver
    /// </summary>
    protected static void CloseDriver()
    {
        lock (Lock)
        {
            try
            {
                Globals.Logger.LogInformation($"Trying to quit WebDriver session '{WebDriver.GetSessionId()}'");
                Globals.WebDriver?.Quit();
                Globals.ListOfInitializedWebdrivers.Remove(Globals.WebDriverKey);
            }
            catch (Exception e)
            {
                Globals.Logger.LogFatal(e, "The driver can't be closed because an error appeared during its closing");
            }
        }
    }

    /// <summary>
    /// Initializes the test class.
    /// </summary>
    [OneTimeSetUp]
    public static void BaseOneTimeSetUp()
    {
        LogFactory.InitializeTestClassLogger();
        Logger.LogInformation($"TEST CLASS \"{TestInfo.FullName}\"");
    }

    /// <summary>
    /// Finalizes the test class.
    /// </summary>
    [OneTimeTearDown]
    public static void BaseOneTimeTearDown()
    {
        LogFactory.CloseLogger();
    }

    /// <summary>
    /// Initializes the test method.
    /// </summary>
    [SetUp]
    public void BaseSetUp()
    {
        LogFactory.InitializeTestMethodLogger();
        Logger.LogInformation($"EXECUTE TEST \"{TestInfo.FullName}\"");
        LogTestProperties(TestInfo.Properties);
        ExecuteBeforeTestStart();
        Logger.LogInformation("TEST START");
        InitializeDriver();
    }

    /// <summary>
    /// Finalizes the test method.
    /// </summary>
    [TearDown]
    public void BaseTearDown()
    {
        Logger.LogInformation("TEST END");
        Logger.LogInformation($"Test Result: {TestInfo.Outcome}");

        LogErrorData();

        ExecuteAfterTestEnd();
        CloseDriver();

        if (TestInfo.Outcome == TestStatus.Failed)
        {
            Logger.LogError("Error Message:" + Environment.NewLine + TestInfo.Result.Message.TrimEnd());
            Logger.LogError("Stack Trace:" + Environment.NewLine + TestInfo.Result.StackTrace?.TrimEnd());
        }

        AddAttachments();
        LogFactory.CloseLogger();
    }

    /// <summary>
    /// Executes logic before the test starts.
    /// </summary>
    /// <remarks>
    /// This method can be overridden in derived classes to provide custom logic, such as logging.
    /// The execution time of this method is not included in the measured test duration.
    /// </remarks>
    protected virtual void ExecuteBeforeTestStart()
    {
    }

    /// <summary>
    /// Executes logic after the test ends.
    /// </summary>
    /// <remarks>
    /// This method can be overridden in derived classes to provide custom logic, such as logging.
    /// The execution time of this method is not included in the measured test duration.
    /// </remarks>
    protected virtual void ExecuteAfterTestEnd()
    {
    }

    /// <summary>
    /// Adds all files to the <see cref="TestContext" /> to make them available
    /// as attachments in the Azure.
    /// </summary>
    private static void AddAttachments()
    {
        if (!TestConfiguration.StoreLogsAlways && TestInfo.Outcome != TestStatus.Failed)
        {
            return;
        }

        Logger.AllLogs.ToList().ForEach(file => TestContext.AddTestAttachment(file));

        LogFactory.InitializedLoggers[$"{TestInfo.Namespace}.{TestInfo.ClassName}"]
                  .AllLogs
                  .ToList()
                  .ForEach(file => TestContext.AddTestAttachment(file));

        if (LogFactory.InitializedLoggers.TryGetValue($"{TestInfo.Namespace}", out ILogger? projectLogger))
        {
            projectLogger.AllLogs.ToList().ForEach(file => TestContext.AddTestAttachment(file));
        }
    }

    private static void LogTestProperties(IDictionary<string, IReadOnlyCollection<object>> properties)
    {
        StringBuilder messageBuilder = new();
        messageBuilder.Append("Test Properties: ");

        // Filter out internal properties
        var propertiesToOutput = properties.Where(p => !p.Key.StartsWith('_')).ToList();

        if (propertiesToOutput.Count == 0)
        {
            messageBuilder.Append("(none)");
        }
        else
        {
            messageBuilder.AppendLine("{");

            foreach (var property in propertiesToOutput)
            {
                messageBuilder.Append("  ");
                messageBuilder.Append(property.Key);
                messageBuilder.Append(": ");
                messageBuilder.Append(string.Join(", ", property.Value));
                messageBuilder.AppendLine();
            }

            messageBuilder.Append('}');
        }

        Logger.LogInformation(messageBuilder.ToString());
    }
}

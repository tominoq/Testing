using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using UI.Template.Framework.Configurations;

namespace UI.Template.Framework.Extensions;

/// <summary>
/// Commonly used extension methods for working with <see cref="WebDriverWait"/> object.
/// </summary>
public static class WebDriverWaitExtensions
{
    /// <summary>
    /// Returns WebDriverWait object with defined timeout and interval (how long each iteration takes).
    /// </summary>
    /// <param name="webDriverWait">current <see cref="WebDriverWait"/></param>
    /// <param name="timeout">How long (seconds) to wait for the condition to be true</param>
    /// <param name="interval">How long (milliseconds) each iteration takes</param>
    /// <param name="exceptionsToBeIgnored">Exceptions that will be ignored if any of these raise</param>
    /// <returns>WebDriverWait</returns>
    public static WebDriverWait GetCustomWait(this WebDriverWait webDriverWait, uint timeout = 0, int interval = -1, params Type[] exceptionsToBeIgnored)
    {
        if (timeout == 0) timeout = TestConfiguration.PageElementTimeout;

        webDriverWait.Timeout = TimeSpan.FromSeconds(timeout);

        if (interval > -1)
            webDriverWait.PollingInterval = TimeSpan.FromMilliseconds(interval);

        Globals.Logger.LogVerbose($"Initialize custom wait with timeout '{webDriverWait.Timeout.TotalSeconds}(s)' and interval '{webDriverWait.PollingInterval.TotalMilliseconds}(ms)'");
        if (exceptionsToBeIgnored?.Length > 0)
            webDriverWait.IgnoreExceptionTypes(exceptionsToBeIgnored);
        return webDriverWait;
    }

    /// <summary>
    /// Routine used for waiting with refresh. Sometimes BO/signal doesn't return response correctly, so we can use this method.
    /// </summary>
    /// <param name="webDriverWait">current <see cref="WebDriverWait"/></param>
    /// <param name="condition">Condition used for wait. If condition is not fulfilled, page is refreshed.</param>
    /// <param name="refreshTimeout">How long custom wait waits for next refresh.</param>
    public static void WaitWithRefresh(this WebDriverWait webDriverWait, Func<bool> condition, uint refreshTimeout = 5)
    {
        int i = 0;
        webDriverWait.SetTimeoutMessage($"Condition '{condition.Method.Name}' wasn't fulfilled during the timeout.")
                     .Until(_ => Globals.Wait.TryWaitWithRefresh(condition, ++i, refreshTimeout));
    }

    /// <summary>
    /// Routine used for waiting with specified timeout. If timeout is reached, page is refreshed.
    /// </summary>
    /// <param name="webDriverWait">current <see cref="WebDriverWait"/></param>
    /// <param name="condition">Condition used for wait. If condition is not fulfilled, page is refreshed.</param>
    /// <param name="iteration">Iteration number.</param>
    /// <param name="refreshTimeout">How long custom wait waits for next refresh.</param>
    private static bool TryWaitWithRefresh(this WebDriverWait webDriverWait, Func<bool> condition, int iteration, uint refreshTimeout = 5)
    {
        Globals.Logger.LogInformation($"Starting wait with refresh and checking method '{condition.Method.Name}'");
        try
        {
            webDriverWait.GetCustomWait(timeout: refreshTimeout).Until(_ => condition());
            Globals.Logger.LogInformation($"Condition '{condition.Method.Name}' is fulfilled");
            return true;
        }
        catch (WebDriverTimeoutException)
        {
            // When condition is not fulfilled, exception is raised and page is refreshed
            Globals.Logger.LogDebug($"Refreshing the page...{iteration} (attempts)");
            Globals.Refresh();
            Globals.WebDriver.WaitForReady();
            return false;
        }
    }

    /// <summary>
    /// Routine used for waiting with specified timeout. If timeout is reached, condition wasn't fulfilled.
    /// </summary>
    /// <param name="webDriverWait">current <see cref="WebDriverWait"/></param>
    /// <param name="condition">Condition used for wait.</param>
    /// <param name="timeout">How long custom wait waits until exception raised.</param>
    /// <returns>True if condition is fulfilled, otherwise false</returns>
    public static bool TryWaitWithCondition(this WebDriverWait webDriverWait, Func<bool> condition, uint timeout = 5)
    {
        Globals.Logger.LogInformation($"Waiting for the method '{condition.Method.Name}' to be true.");
        try
        {
            webDriverWait.GetCustomWait(timeout: timeout).Until(_ => condition());
            Globals.Logger.LogInformation($"Condition '{condition.Method.Name}' is fulfilled");
            return true;
        }
        catch (WebDriverTimeoutException)
        {
            return false;
        }
    }

    /// <summary>
    /// Method is used for waiting if the action is executed as expected.
    /// </summary>
    /// <param name="webDriverWait">current <see cref="WebDriverWait"/></param>
    /// <param name="action"><see cref="Action"/> that is executed at least once. If the condition is not fulfilled after action is executed, it's executed repeatedly.</param>
    /// <param name="condition">Condition used for wait, if the action is executed as expected.</param>
    /// <param name="timeoutMessage">Message to be logged if the condition of the inner wait won't be fulfilled after <paramref name="conditionTimeout"/></param>
    /// <param name="timeout">How long (seconds) will the outer wait keep trying to trigger the action and meet the condition</param>
    /// <param name="interval">How long (milliseconds) each iteration takes</param>
    /// <param name="conditionTimeout">How long (seconds) will the inner wait try to evaluate the condition until it returns false to the outer wait</param>
    public static void WaitWithActionAndCondition(this WebDriverWait webDriverWait, Action action, Func<bool> condition, string? timeoutMessage = null, uint timeout = 0, int interval = -1, uint conditionTimeout = 5)
    {
        webDriverWait.GetCustomWait(timeout: timeout, interval: interval)
                     .SetTimeoutMessage($"Condition '{condition.Method.Name}' wasn't fulfilled during the timeout.")
                     .Until(_ =>
        {
            try
            {
                action();
                Globals.Logger.LogVerbose($"Waiting for the condition '{condition.Method.Name}' to be fulfilled.");
                webDriverWait.GetCustomWait(timeout: conditionTimeout).Until(_ => condition());
                return true;
            }
            catch (WebDriverTimeoutException)
            {
                // when condition is not fulfilled, exception is raised and action is repeated
                Globals.Logger.LogWarning(timeoutMessage ?? $"Condition is not fulfilled, it's necessary to try execute action '{action.Method.Name}' again.");
                return false;
            }
        });
    }

    /// <summary>
    /// Method sets the 'Message' property of current <see cref="WebDriverWait"/> object with custom value.
    /// </summary>
    /// <param name="webDriverWait"><see cref="WebDriverWait"/></param>
    /// <param name="message">Message to be displayed when timeout is reached</param>
    /// <returns><see cref="WebDriverWait"/></returns>
    public static WebDriverWait SetTimeoutMessage(this WebDriverWait webDriverWait, string message)
    {
        webDriverWait.Message = message;
        return webDriverWait;
    }

    /// <summary>
    /// Method is used for waiting if the action can be executed.
    /// </summary>
    /// <param name="webDriverWait"><see cref="WebDriverWait"/></param>
    /// <param name="condition">Condition used for wait, if the action can be executed.</param>
    /// <param name="action"><see cref="Action"/> that is executed only once if the condition is fulfilled.</param>
    /// <param name="timeoutMessage">Message to be logged if the condition of the inner wait won't be fulfilled after <paramref name="conditionTimeout"/></param>
    /// <param name="conditionTimeout">How long (seconds) will the inner wait try to evaluate the condition until it returns false to the outer wait</param>
    /// <returns>True if the condition is fulfilled and action invoked, otherwise false</returns>
    public static bool TryWaitWithConditionAndAction(this WebDriverWait webDriverWait, Func<bool> condition, Action action, string? timeoutMessage = null, uint conditionTimeout = 5)
    {
        try
        {
            webDriverWait.GetCustomWait(timeout: conditionTimeout).Until(_ => condition());
            Globals.Logger.LogVerbose($"Waiting for the condition '{condition.Method.Name}' to be fulfilled.");
            action();
            return true;
        }
        catch (WebDriverTimeoutException)
        {
            // when condition is not fulfilled, exception is raised and action is repeated
            Globals.Logger.LogWarning(timeoutMessage ?? $"Condition is not fulfilled, it's necessary to try execute action '{action.Method.Name}' again.");
            return false;
        }
    }
}

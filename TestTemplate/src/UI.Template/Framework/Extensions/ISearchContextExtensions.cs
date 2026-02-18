using System.Collections.ObjectModel;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using UI.Template.Components;
using UI.Template.Framework.Configurations;

namespace UI.Template.Framework.Extensions;

/// <summary>
/// Holds extension methods for <see cref="ISearchContext"/> object.
/// </summary>
public static class ISearchContextExtensions
{
    /// <summary>
    /// Resolves search context and returns it as global or element level.
    /// </summary>
    /// <param name="searchContext"><see cref="ISearchContext"/></param>
    /// <returns>IWebDriver resolved search context</returns>
    public static IWebDriver? GetWebDriver(this ISearchContext searchContext)
    {
        ISearchContext context = searchContext;
        if (context is BaseComponent component)
        {
            context = component.Element;
        }
#pragma warning disable IDE0038
        return context is IWebElement ? ((IWebElement)context).GetWebDriver() : context as IWebDriver;
#pragma warning restore IDE0038
    }

    /// <summary>
    /// Waits until we find IWebElement.
    /// </summary>
    /// <param name="searchContext"><see cref="ISearchContext"/></param>
    /// <param name="by"><see cref="By"/></param>
    private static void WaitForElement(ISearchContext searchContext, By by)
    {
        Globals.Logger.LogVerbose($"Waiting for presence of the element with by '{by}'");
        IWebDriver driver = searchContext.GetWebDriver() ?? throw new InvalidOperationException("Unable to resolve IWebDriver");
        TimeSpan timeout = TimeSpan.FromSeconds(TestConfiguration.PageElementTimeout);
        Globals.Logger.LogVerbose($"Wait call '{driver}' from '{searchContext}' up to {timeout}s");
        var wait = new WebDriverWait(driver, timeout);
        wait.Until(_ =>
        {
            return searchContext.FindElement(by);
        });
        Globals.Logger.LogVerbose($"Element with by '{by}' was found");
    }

    /// <summary>
    /// Finds the first <see cref="IWebElement"/> using the given method.
    /// </summary>
    /// <param name="searchContext"><see cref="ISearchContext"/></param>
    /// <param name="by"><see cref="By"/></param>
    /// <returns>The first matching <see cref="IWebElement"/> on the current context.</returns>
    public static IWebElement FindWebElement(this ISearchContext searchContext, By by)
    {
        WaitForElement(searchContext, by);
        return searchContext.FindElement(by);
    }

    /// <summary>
    /// Finds the first <see cref="IWebElement"/> using the given id.
    /// </summary>
    /// <param name="searchContext"><see cref="ISearchContext"/></param>
    /// <param name="id">Id of the element</param>
    /// <returns>The first matching <see cref="IWebElement"/> on the current context.</returns>
    public static IWebElement FindWebElement(this ISearchContext searchContext, string id)
    {
        WaitForElement(searchContext, Globals.GetCustomLocator(id));
        return searchContext.FindElement(Globals.GetCustomLocator(id));
    }

    /// <summary>
    /// Finds all IWebElements within the current context using the given mechanism.
    /// </summary>
    /// <param name="searchContext"><see cref="ISearchContext"/></param>
    /// <param name="by"><see cref="By"/></param>
    /// <returns>A <see cref="ReadOnlyCollection{T}"/> of all WebElements matching the current criteria, or an empty list if nothing matches.</returns>
    public static ReadOnlyCollection<IWebElement> FindWebElements(this ISearchContext searchContext, By by)
    {
        WaitForElement(searchContext, by);
        return searchContext.FindElements(by);
    }

    /// <summary>
    /// Finds all IWebElements within the current context using the given id.
    /// </summary>
    /// <param name="searchContext"><see cref="ISearchContext"/></param>
    /// <param name="id">Id of the element</param>
    /// <returns>A <see cref="ReadOnlyCollection{T}"/> of all WebElements matching the current criteria, or an empty list if nothing matches.</returns>
    public static ReadOnlyCollection<IWebElement> FindWebElements(this ISearchContext searchContext, string id)
    {
        By by = Globals.GetCustomLocator(id);
        return searchContext.FindWebElements(by);
    }
}

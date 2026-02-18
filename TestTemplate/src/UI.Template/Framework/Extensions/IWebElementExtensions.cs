using OpenQA.Selenium;
using UI.Template.Framework.Helpers;

namespace UI.Template.Framework.Extensions;

/// <summary>
/// Holds extension methods for <see cref="IWebElement"/> objects.
/// </summary>
public static class IWebElementExtensions
{
    /// <summary>
    /// Clicks on element even when it is overlapped
    /// </summary>
    /// <param name="element">Element to click on</param>
    public static void ClickJS(this IWebElement element)
    {
        var js = (IJavaScriptExecutor)element.GetWebDriver();
        js.ExecuteScript("arguments[0].click()", element);
    }

    /// <summary>
    /// Set element value attribute using IJavaScriptExecutor
    /// </summary>
    /// <param name="element">Element to be set</param>
    /// <param name="text">Text to be set</param>
    public static void SetTextJS(this IWebElement element, string text)
    {
        var js = (IJavaScriptExecutor)element.GetWebDriver();
        js.ExecuteScript($"arguments[0].value = '{text}';", element);
    }

    /// <summary>
    /// Get element value attribute using IJavaScriptExecutor
    /// </summary>
    /// <param name="element">Element to get text from</param>
    /// <returns>Text contained in IWebElement</returns>
    public static string GetTextJS(this IWebElement element)
    {
        var js = (IJavaScriptExecutor)element.GetWebDriver();
        return (string)(js.ExecuteScript("return arguments[0].value;", element) ?? string.Empty);
    }

    /// <summary>
    /// Gets <see cref="IWebDriver"/> from the current <see cref="IWebElement"/>.
    /// </summary>
    /// <param name="element"><see cref="IWebElement"/></param>
    /// <returns><see cref="IWebDriver"/> the <paramref name="element"/> belong to</returns>
    public static IWebDriver GetWebDriver(this IWebElement element)
    {
        return ((IWrapsDriver)element).WrappedDriver;
    }

    /// <summary>
    /// Scrolls using JS so that element is centered and visible in viewport. Handy for click intercepts.
    /// </summary>
    /// <param name="element"><see cref="IWebElement"/></param>
    public static void ScrollToJS(this IWebElement element)
    {
        var js = (IJavaScriptExecutor)element.GetWebDriver();
        js.ExecuteScript("arguments[0].scrollIntoView({block: \"center\", inline: \"center\"});", element);
    }

    /// <summary>
    /// Function returns inner text of the node without text of all children
    /// </summary>
    /// <param name="element"><see cref="IWebElement"/></param>
    /// <returns>Inner text of the node</returns>
    public static string GetInnerText(this IWebElement element)
    {
        return Utils.GetTextValueFromHtml(element.GetDomProperty("outerHTML") ?? string.Empty);
    }
}

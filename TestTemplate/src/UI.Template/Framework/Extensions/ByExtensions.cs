using OpenQA.Selenium;
using UI.Template.Framework.Helpers;

namespace UI.Template.Framework.Extensions;

/// <summary>
/// Holds extension methods for <see cref="By"/> locators.
/// </summary>
public static class ByExtensions
{
    /// <summary>
    /// Returns locator as string.
    /// </summary>
    /// <param name="by"><see cref="By"/></param>
    /// <returns>string</returns>
    public static string ToSelector(this By by)
    {
        var s = by.ToString();
        return GeneratedRegex.SelectorRegex().Replace(s, "$1");
    }
}

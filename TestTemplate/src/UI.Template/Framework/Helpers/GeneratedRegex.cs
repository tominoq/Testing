using System.Text.RegularExpressions;
using OpenQA.Selenium;

namespace UI.Template.Framework.Helpers;

/// <summary>
/// Represents a class that contains generated regular expressions.
/// </summary>
public static partial class GeneratedRegex
{
    /// <summary>
    /// Regex for extracting the status code from the page title and the site's browser logs (example: "Error - 503" or "... 503 ()").
    /// </summary>
    /// <returns></returns>
    [GeneratedRegex(@"(?<statusCode500x>(5\d{2}){1})(\s*\(\))?$")]
    public static partial Regex StatusCodeRegex();

    /// <summary>
    /// Regex for extracting the SMS code from the message.
    /// </summary>
    /// <returns></returns>
    [GeneratedRegex(@"\d{4}")]
    public static partial Regex SmsCodeRegex();

    /// <summary>
    /// Regex for searching prefix used when calling <see cref="By.ToString()"/> method.
    /// </summary>
    /// <returns></returns>
    [GeneratedRegex(@"By\..+?:\s(.+)")]
    public static partial Regex SelectorRegex();

    /// <summary>
    /// Regex for searching all white characters in the string.
    /// </summary>
    /// <returns></returns>
    [GeneratedRegex(@"\s+")]
    public static partial Regex WhiteCharactersRegex();

    /// <summary>
    /// Regex for searching the date in the string.
    /// </summary>
    /// <returns></returns>
    [GeneratedRegex(@"(\d{1,2}(\/|\.)\s{0,1}\d{1,2}(\/|\.)\s{0,1}\d{4})|(\d{4}\.\s{0,1}\d{1,2}\.\s{0,1}\d{1,2}\.)")]
    public static partial Regex DateRegex();
}

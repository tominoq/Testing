using System.Reflection;
using UI.Template.Framework.Attributes;

namespace UI.Template.Framework.Enums;

/// <summary>
/// Enum to hold the different types of WebDriver logs.
/// </summary>
public enum WebDriverLog
{
    #region General Log Types
    /// <summary>
    /// WebDriver server logs.
    /// </summary>
    [Name("server")]
    Server,

    /// <summary>
    /// Browser server logs.
    /// </summary>
    [Name("browser")]
    Browser,
    #endregion General Log Types
}

/// <summary>
/// Extension class for <see cref="WebDriverLog"/>.
/// </summary>
public static class WebDriverLogExtensions
{
    /// <summary>
    /// Gets the translation to be used for api calling.
    /// </summary>
    /// <param name="webDriverLog"><see cref="WebDriverLog"/></param>
    /// <returns>Valid api translation</returns>
    public static string GetName(this WebDriverLog webDriverLog)
    {
        FieldInfo? field = webDriverLog.GetType().GetField(webDriverLog.ToString());
        NameAttribute? attribute = field?.GetCustomAttribute<NameAttribute>(false);
        return attribute != null ? attribute.Name : webDriverLog.ToString();
    }
}

namespace UI.Template.Framework.Configurations;

/// <summary>
/// The configuration for the tests.
/// </summary>
public static class WebConfiguration
{
    static WebConfiguration()
    {
        var section = Globals.Configuration.GetSection(nameof(WebConfiguration));
        BaseUrl = section[nameof(BaseUrl)] ?? string.Empty;
        UserName = section[nameof(UserName)] ?? string.Empty;
        UserPassword = section[nameof(UserPassword)] ?? string.Empty;

        if (string.IsNullOrEmpty(UserName) || string.IsNullOrEmpty(UserPassword))
        {
            throw new ArgumentException("UserName or UserPassword is not set in the configuration.");
        }
    }

    /// <summary>
    /// Base URL of the application under test. For example: https://www.test.cz
    /// </summary>
    public static string BaseUrl { get; private set; }

    /// <summary>
    /// User name for authentication.
    /// </summary>
    public static string UserName { get; private set; }

    /// <summary>
    /// User password for authentication.
    /// </summary>
    public static string UserPassword { get; private set; }


}

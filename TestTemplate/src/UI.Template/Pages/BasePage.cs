using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using UI.Template.Framework.Configurations;
using UI.Template.Framework.Extensions;
using UI.Template.Framework.Interfaces;
using UI.Template.Framework.Logging;

namespace UI.Template.Pages;

/// <summary>
/// Common logic for working with Page objects.
/// </summary>
public abstract class BasePage : IPage
{
    /// <summary>
    /// BasePage parameterless constructor. Uses current WebDriver URL.
    /// </summary>
    protected BasePage()
    {
        SetUrl(WebDriver.UrlPathAndQuery());
    }

    /// <summary>
    /// BasePage constructor with url parameter.
    /// </summary>
    /// <param name="path">valid path string to be set as path and url</param>
    protected BasePage(string path)
    {
        SetUrl(path);
    }

    /// <summary>
    /// Full resolved URL to a page.
    /// </summary>
    /// <value>Uri</value>
    public Uri? Url { get; private set; }

    /// <summary>
    /// Reference of IPage.WebDriver for easier usage.
    /// </summary>
    protected IWebDriver WebDriver => Page.WebDriver;

    /// <summary>
    /// Reference of IPage.Logger for easier usage.
    /// </summary>
    protected ILogger Logger => Page.Logger;

    /// <summary>
    /// Reference of IPage.Wait for easier usage.
    /// </summary>
    protected WebDriverWait Wait => Page.Wait;

    /// <summary>
    /// Base URL that is set via <see cref="WebConfiguration.BaseUrl"/>.
    /// Value depends on the each project and can contain for example
    /// https://test.cz
    /// </summary>
    private static string BaseUrl { get; } = WebConfiguration.BaseUrl;

    /// <summary>
    /// URL path to a page. Used as parameter in constructor.
    /// </summary>
    /// <value>string</value>
    private string? Path { get; set; }

    private IPage Page => this;

    /// <inheritdoc/>
    public void SetUrl(string? path)
    {
        if (path is null)
        {
            throw new ArgumentException("Parameter path cannot be null");
        }

        if (!path.StartsWith('/'))
        {
            throw new ArgumentException($"Given path '{path}' has to start with /");
        }

        Path = path;
        Url = new Uri(BaseUrl + Path);
    }

    #region Open and navigation helper methods
    /// <inheritdoc/>
    public void Open()
    {
        Open(Url);
    }

    /// <summary>
    /// Navigates to given URL.
    /// </summary>
    /// <param name="url">Uri with full resolved URL</param>
    public void Open(Uri? url)
    {
        ArgumentNullException.ThrowIfNull(url);

        if (!Globals.IsLogged)
        {
            Logger.LogVerbose($"Opening page {url} with user authorization.");
            var uriWithCred = new UriBuilder(url) { UserName = WebConfiguration.UserName, Password = WebConfiguration.UserPassword }.Uri;
            WebDriver.Navigate().GoToUrl(uriWithCred);
            WaitForReady();
            Globals.IsLogged = true;
        }
        else
        {
            WebDriver.Navigate().GoToUrl(url);
        }
    }

    /// <summary>
    /// Navigates to given URL path.
    /// </summary>
    /// <param name="path">URL Path like /Order1.htm</param>
    public void OpenPath(string path)
    {
        if (!path.StartsWith('/'))
        {
            throw new ArgumentException($"Given URL {path} has to start with /");
        }

        var uri = new Uri(Url?.GetLeftPart(UriPartial.Authority) + path);

        if (!Globals.IsLogged)
        {
            Logger.LogVerbose($"Opening path {path} with user authorization.");
            var uriWithCred = new UriBuilder(uri) { UserName = WebConfiguration.UserName, Password = WebConfiguration.UserPassword }.Uri;
            WebDriver.Navigate().GoToUrl(uriWithCred.ToString());
            WaitForReady();
            Globals.IsLogged = true;
        }
        else
        {
            WebDriver.Navigate().GoToUrl(uri.ToString());
            WaitForReady();
        }
    }

    /// <inheritdoc/>
    public void Back()
    {
        WebDriver.WaitForUrlChanged(() => WebDriver.Navigate().Back());
        WaitForReady();
    }

    /// <inheritdoc/>
    public void Refresh()
    {
        Logger.LogVerbose("Page will be refreshed");
        Globals.Refresh();
        WaitForReady();
    }

    /// <inheritdoc/>
    public void Close()
    {
        Logger.LogVerbose("Window/tab will be closed");

        if (WebDriver.CurrentWindowHandle == Globals.MainWindowName)
        {
            Logger.LogWarning("Closing of the main window is forbidden. Use Close method only on the page opened from main window.");
            return;
        }

        WebDriver.Close();
        WebDriver.SwitchToTab(Globals.MainWindowName);
        WaitForReady();
    }
    #endregion Open and navigation helper methods

    #region Other helper methods
    /// <inheritdoc/>
    public virtual bool IsReady()
    {
        return WebDriver.IsReady();
    }

    /// <inheritdoc/>
    public virtual void WaitForReady()
    {
        WebDriver.WaitForReady();
    }
    #endregion Other helper methods
}

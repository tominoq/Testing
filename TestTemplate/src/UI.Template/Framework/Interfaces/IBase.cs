using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using UI.Template.Framework.Logging;

namespace UI.Template.Framework.Interfaces;

/// <summary>
/// Holds common properties and methods that can be used in all components and pages.
/// </summary>
public interface IBase //It's like BaseObject
{
    /// <summary>
    /// Reference of Globals.webDriver for easier usage.
    /// </summary>
    IWebDriver WebDriver => Globals.WebDriver;

    /// <summary>
    /// Reference of Globals.Logger for easier usage.
    /// </summary>
    ILogger Logger => Globals.Logger;

    /// <summary>
    /// Reference of Globals.Action for easier usage.
    /// </summary>
    Actions Action => Globals.Action;

    /// <summary>
    /// Reference of Globals.Wait for easier usage.
    /// </summary>
    WebDriverWait Wait => Globals.Wait;

    /// <summary>
    /// Reference of Globals.JavaScriptExecutor for easier usage.
    /// </summary>
    IJavaScriptExecutor JavaScriptExecutor => Globals.JavaScriptExecutor;

    /// <summary>
    /// Reference of Globals.Random for easier usage.
    /// </summary>
    Random Random => Globals.Random;
}

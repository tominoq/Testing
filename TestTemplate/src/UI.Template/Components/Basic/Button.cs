using OpenQA.Selenium;

namespace UI.Template.Components.Basic;

/// <summary>
/// Wraps IWebElement and acts as a button or input[type=text] or whatever we want to be a button.
/// </summary>
public class Button : BaseComponent
{
    public Button() { }

    public Button(By locator) : base(locator) { }

    public Button(By locator, ISearchContext searchContext) : base(locator, searchContext) { }

    /// <summary>
    /// Clicks and synchronizes the code after.
    /// </summary>
    public override void Click()
    {
        WaitForEnabled();
        base.Click();
        WaitForReady();
    }

    /// <summary>
    /// Clicks using JS and synchronizes the code after.
    /// </summary>
    public override void ClickJS()
    {
        WaitForEnabled();
        base.ClickJS();
        WaitForReady();
    }
}

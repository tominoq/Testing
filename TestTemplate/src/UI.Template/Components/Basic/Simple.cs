using OpenQA.Selenium;

namespace UI.Template.Components.Basic;

/// <summary>
/// Wraps IWebElement so that it behaves like any other BaseComponent.
/// </summary>
///
/// We can use methods like:
/// - WaitForDisplayed
/// - WaitForPresent
/// ... see BaseComponent for more
public class Simple : BaseComponent
{
    public Simple(By locator) : base(locator) { }

    public Simple(By locator, ISearchContext searchContext) : base(locator, searchContext) { }

    /// <inheritdoc/>
    public override void ScrollToAndClick()
    {
        ScrollTo();
        Click();
    }
}

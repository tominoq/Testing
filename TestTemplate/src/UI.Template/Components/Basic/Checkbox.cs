using OpenQA.Selenium;
using UI.Template.Framework.Extensions;

namespace UI.Template.Components.Basic;

/// <summary>
/// Wraps IWebElement and acts as an input[type=checkbox].
/// </summary>
public class Checkbox : BaseComponent
{
    public Checkbox() { }

    public Checkbox(By locator) : base(locator) { }

    public Checkbox(By locator, ISearchContext searchContext) : base(locator, searchContext) { }

    /// <summary>
    /// Returns true/false value if element is checked/unchecked.
    /// </summary>
    /// <returns>boolean</returns>
    public bool IsChecked()
    {
        return EvaluateElementBoolFunction((element) => element.Selected ? element : null, null);
    }

    /// <summary>
    /// Returns true/false value if element is checked/unchecked.
    /// </summary>
    /// <returns>boolean</returns>
    public bool IsNotChecked()
    {
        return EvaluateElementBoolFunction((element) => !element.Selected ? element : null, null);
    }

    /// <summary>
    /// Makes selected property of checkbox true.
    /// </summary>
    public void Check()
    {
        if (IsNotChecked())
        {
            Wait.SetTimeoutMessage($"'{GetType().Name}' with locator '{Locator}' wasn't checked during the timeout.")
                .Until(_ =>
            {
                Click();
                return IsChecked();
            });
        }
    }

    /// <summary>
    /// Makes selected property of checkbox false.
    /// </summary>
    public void UnCheck()
    {
        if (IsChecked())
        {
            Wait.SetTimeoutMessage($"'{GetType().Name}' with locator '{Locator}' wasn't unchecked during the timeout.")
                .Until(_ =>
            {
                Click();
                return IsNotChecked();
            });
        }
    }
}

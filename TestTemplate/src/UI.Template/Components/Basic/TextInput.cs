using OpenQA.Selenium;
using UI.Template.Framework.Extensions;

namespace UI.Template.Components.Basic;

/// <summary>
/// Wraps IWebElement and acts as TextInput component.
/// </summary>
public class TextInput : BaseComponent
{
    public TextInput(By locator) : base(locator) { }

    public TextInput(By locator, ISearchContext searchContext) : base(locator, searchContext) { }

    /// <summary>
    /// Gets the innerText of TextInput.
    /// Without any leading or trailing whitespace and with other whitespace collapsed.
    /// </summary>
    /// <returns>string</returns>
    public string GetTextJS()
    {
        string text = Element.GetTextJS();
        Logger.LogVerbose($"Text of the '{GetType().Name}' with locator '{Locator}' is '{text}'.");
        return text;
    }

    /// <summary>
    /// Move mouse to element and click on it.
    /// </summary>
    public override void HoverAndClick()
    {
        WaitForEnabled();
        Action.MoveToElement(Element).Click().Perform();
    }

    /// <summary>
    /// Clears text inside TextInput using standard Selenium API.
    /// </summary>
    public void Clear()
    {
        Wait.SetTimeoutMessage($"'{GetType().Name}' with locator '{Locator}' couldn't cleared during the timeout.")
            .Until(_ => IsCleared());
    }

    /// <summary>
    /// Returns true/false if Element is cleared
    /// </summary>
    /// <returns>boolean</returns>
    public bool IsCleared()
    {
        return EvaluateElementBoolFunction((element) => element.Displayed && element.Enabled ? element : null, (element) => element.Clear());
    }

    /// <summary>
    /// Clears text inside TextInput using JS.
    /// </summary>
    public void ClearJS()
    {
        WaitForEnabled();
        Element.SetTextJS("");
        Wait.SetTimeoutMessage($"'{GetType().Name}' with locator '{Locator}' couldn't cleared during the timeout.")
            .Until(_ => string.IsNullOrEmpty(Element.Text));
    }

    /// <summary>
    /// Clears text inside TextInput by sending backspace keystroke until the field is empty.
    /// </summary>
    public void ClearWithBackspace()
    {
        WaitForEnabled();
        // sometimes the cursor is not at the end of the text so the whole text is not cleared
        Element.SendKeys(Keys.End);
        while (Element.GetTextJS().Length > 0)
        {
            Element.SendKeys(Keys.Backspace);
        }
    }

    /// <summary>
    /// Simulates typing text into TextInput using standard Selenium API.
    /// </summary>
    public void SendKeys(string text)
    {
        ArgumentNullException.ThrowIfNull(text);
        Logger.LogVerbose($"Text to be filled into '{GetType().Name}' with locator '{Locator}' is '{text}'.");
        Wait.SetTimeoutMessage($"'{GetType().Name}' with locator '{Locator}' couldn't be set during the timeout.")
            .Until(_ => IsSendKeys(text));
    }

    /// <summary>
    /// Returns true/false if Element is cleared
    /// </summary>
    /// <returns>boolean</returns>
    public bool IsSendKeys(string text)
    {
        return EvaluateElementBoolFunction(
            (element) => element != null && element.Displayed && element.Enabled ? element : throw new InvalidOperationException("Element is not displayed or not enabled."),
            (element, insertedText) => element.SendKeys(insertedText),
            text);
    }

    /// <summary>
    /// Simulates typing text into TextInput using JS.
    /// </summary>
    /// <param name="text">Text to be sent to element</param>
    public void SendKeysJS(string text)
    {
        ArgumentNullException.ThrowIfNull(text);
        Logger.LogVerbose($"Text to be filled into '{GetType().Name}' with locator '{Locator}' is '{text}'.");
        WaitForEnabled();
        Element.SetTextJS(text);
    }

    /// <summary>
    /// Sends enter key into TextInput.
    /// </summary>
    public void SendEnter()
    {
        SendKeys(Keys.Enter);
    }

    /// <summary>
    /// Sends space key into TextInput.
    /// </summary>
    public void SendSpace()
    {
        SendKeys(Keys.Space);
    }

    /// <summary>
    /// Sends backspace key.
    /// </summary>
    public void SendBackspace()
    {
        SendKeys(Keys.Backspace);
    }

    /// <summary>
    /// Gets the value of the attribute value of TextInput.
    /// </summary>
    /// <returns>string</returns>
    public string GetValue()
    {
        string value = GetDomProperty("value");
        Logger.LogVerbose($"Value of the '{GetType().Name}' with locator '{Locator}' is '{value}'.");
        return value;
    }

    /// <summary>
    /// Sends CTRL+A shortcut to select all text inside the input.
    /// </summary>
    public void SelectAll()
    {
        SendKeys(Keys.Control + "a");
    }
}

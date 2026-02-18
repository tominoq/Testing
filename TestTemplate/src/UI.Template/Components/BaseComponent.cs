using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using UI.Template.Framework.Extensions;
using UI.Template.Framework.Helpers;
using UI.Template.Framework.Interfaces;
using UI.Template.Framework.Logging;

namespace UI.Template.Components;

/// <summary>
/// Represents wrapper of IWebElement having handy properties and methods.
/// </summary>
public abstract class BaseComponent : IComponent, ISearchContext
{
    private ISearchContext? _context;

    #region Constructors
    /// <summary>
    /// Parameterless constructor of <see cref="BaseComponent"/> object.
    /// The search context will be <see cref="IWebDriver"/> and locator is the main (generic) body tag of the page.
    /// In the case the component is not based on HTML, custom constructor should be created.
    /// </summary>
    protected BaseComponent()
    {
        Locator = By.TagName("body");
        Context = WebDriver;
    }

    /// <summary>
    /// Constructor of <see cref="BaseComponent"/> object with specific <see cref="By"/> locator parameter.
    /// The search context will be <see cref="IWebDriver"/>.
    /// </summary>
    /// <param name="locator"><see cref="By"/></param>
    protected BaseComponent(By locator)
    {
        Locator = locator;
        Context = WebDriver;
    }

    /// <summary>
    /// Constructor of <see cref="BaseComponent"/> object with specific <see cref="By"/> locator and <see cref="ISearchContext"/> parameters.
    /// </summary>
    /// <param name="locator"><see cref="By"/></param>
    /// <param name="searchContext"><see cref="ISearchContext"/></param>
    protected BaseComponent(By locator, ISearchContext searchContext)
    {
        Locator = locator;
        Context = searchContext;
    }
    #endregion Constructors

    #region Properties
    /// <summary>
    /// <see cref="By"/> locator of the current component.
    /// </summary>
    public By Locator { get; init; }

    /// <summary>
    /// Current component as <see cref="IWebElement"/> on the page.
    /// </summary>
    public virtual IWebElement Element => (Context ?? throw new InvalidOperationException("Search context is null.")).FindWebElement(Locator);

#pragma warning disable RCS1085
    /// <summary>
    /// Current <see cref="ISearchContext"/>.
    /// </summary>
    protected ISearchContext? Context { get => _context; init => _context = value; }
#pragma warning restore RCS1085

    /// <summary>
    /// Reference of IBase.WebDriver for easier usage.
    /// </summary>
    protected IWebDriver WebDriver => Component.WebDriver;

    /// <summary>
    /// Reference of IBase.Logger for easier usage.
    /// </summary>
    protected ILogger Logger => Component.Logger;

    /// <summary>
    /// Reference of IBase.Action for easier usage.
    /// </summary>
    protected Actions Action => Component.Action;

    /// <summary>
    /// Reference of IBase.Wait for easier usage.
    /// </summary>
    protected WebDriverWait Wait => Component.Wait;

    /// <summary>
    /// Reference of IBase.JavaScriptExecutor for easier usage.
    /// </summary>
    protected IJavaScriptExecutor JavaScriptExecutor => Component.JavaScriptExecutor;

    /// <summary>
    /// Handel of the current object.
    /// </summary>
    protected virtual IBase Component => this;
    #endregion Properties

    // Methods
    #region Methods to get Properties/States of the Component
    /// <summary>
    /// Sets new <see cref="ISearchContext"/> for the current component.
    /// </summary>
    /// <param name="searchContext"><see cref="ISearchContext"/></param>
    public void SetSearchContext(ISearchContext searchContext) => _context = searchContext;

    /// <summary>
    /// Returns true/false if Element is present in DOM
    /// </summary>
    /// <returns>boolean</returns>
    public virtual bool IsPresent() => EvaluateElementBoolFunction((element) => element, null);

    /// <summary>
    /// Returns true/false if Element is not present in DOM
    /// </summary>
    /// <returns>boolean</returns>
    public virtual bool IsNotPresent() => !EvaluateElementBoolFunction((element) => element, null, webDriverExc: true);

    /// <summary>
    /// Returns true/false if Element is displayed
    /// </summary>
    /// <returns>boolean</returns>
    public virtual bool IsDisplayed() => EvaluateElementBoolFunction((element) => element.Displayed ? element : null, null);

    /// <summary>
    /// Returns true/false if Element is displayed using JS.
    /// https://stackoverflow.com/questions/19669786/check-if-element-is-visible-in-dom
    /// </summary>
    /// <returns>boolean</returns>
    public virtual bool IsDisplayedJS()
    {
        const string script = "elem = arguments[0];" +
                              "const style = getComputedStyle(elem);" +
                              "if (style.display === 'none') return false;" +
                              "if (style.visibility !== 'visible') return false;" +
                              "if (style.opacity < 0.1) return false;" +
                              "if (elem.offsetWidth + elem.offsetHeight + elem.getBoundingClientRect().height +" +
                              "    elem.getBoundingClientRect().width === 0) {" +
                              "    return false;" +
                              "}" +
                              "const elemCenter = {" +
                              "    x: elem.getBoundingClientRect().left + elem.offsetWidth / 2," +
                              "    y: elem.getBoundingClientRect().top + elem.offsetHeight / 2" +
                              "};" +
                              "if (elemCenter.x < 0) return false;" +
                              "if (elemCenter.x > (document.documentElement.clientWidth || window.innerWidth)) return false;" +
                              "if (elemCenter.y < 0) return false;" +
                              "if (elemCenter.y > (document.documentElement.clientHeight || window.innerHeight)) return false;" +
                              "let pointContainer = document.elementFromPoint(elemCenter.x, elemCenter.y);" +
                              "do {" +
                              "    if (pointContainer === elem) return true;" +
                              "} while (pointContainer = pointContainer.parentNode);" +
                              "return false;";
        return (bool)(JavaScriptExecutor.ExecuteScript(script, Element) ?? false);
    }

    /// <summary>
    /// Returns true/false if Element is not displayed
    /// </summary>
    /// <returns>boolean</returns>
    public virtual bool IsNotDisplayed() => EvaluateElementBoolFunction((element) => !element.Displayed ? element : null, null, noElement: true);

    /// <summary>
    /// Returns true/false if Element is enabled
    /// </summary>
    /// <returns>boolean</returns>
    public virtual bool IsEnabled() => EvaluateElementBoolFunction((element) => element.Displayed && element.Enabled ? element : null, null);

    /// <summary>
    /// Returns true/false if Element is enabled regardless the element is displayed or not.
    /// </summary>
    /// <returns>boolean</returns>
    public virtual bool IsEnabledOnly() => EvaluateElementBoolFunction((element) => element.Enabled ? element : null, null);

    /// <summary>
    /// Returns true/false if Element is disabled
    /// </summary>
    /// <returns>boolean</returns>
    public virtual bool IsDisabled() => EvaluateElementBoolFunction((element) => element.Displayed && !element.Enabled ? element : null, null);

    /// <summary>
    /// Method returns result of the comparing before and after specified time
    /// </summary>
    /// <returns>True if height is still changing, otherwise false</returns>
    public virtual bool IsHeightChanging(int sleepTimeout = 100)
    {
        string before = GetCssValue("height");
        Utils.PauseThread(sleepTimeout);
        string after = GetCssValue("height");
        return before != after;
    }

    /// <summary>
    /// Method checks if two components are overlapping
    /// </summary>
    /// <param name="component1"><see cref="BaseComponent"/></param>
    /// <param name="component2"><see cref="BaseComponent"/></param>
    /// <returns>True if components are overlapping, false if not</returns>
    public static bool ComponentsAreOverlapping(BaseComponent component1, BaseComponent component2)
    {
        IWebElement element1 = component1.Element;
        IWebElement element2 = component2.Element;

        return !(element1.Location.Y > element2.Location.Y + element2.Size.Height ||
                 element1.Location.X + element1.Size.Width < element2.Location.X ||
                 element1.Location.Y + element1.Size.Height < element2.Location.Y ||
                 element1.Location.X > element2.Location.X + element2.Size.Width);
    }

    /// <summary>
    /// Method checks if two components are vertically overlapping
    /// </summary>
    /// <param name="component"><see cref="BaseComponent"/></param>
    /// <returns>True if components are vertically overlapping, false if not</returns>
    public virtual bool ComponentsAreVerticallyOverlapping(BaseComponent component)
    {
        IWebElement element1 = Element;
        IWebElement element2 = component.Element;

        return !(element1.Location.Y > element2.Location.Y + element2.Size.Height ||
                 element1.Location.Y + element1.Size.Height < element2.Location.Y);
    }

    /// <summary>
    /// Returns value of the IWebElement attribute.
    /// </summary>
    /// <param name="attributeName">Attribute name</param>
    /// <param name="timeout">How long the method will be wait until any value is get</param>
    /// <returns>string</returns>
    public virtual string GetDomAttribute(string attributeName, uint timeout = 0)
    {
        string value = Wait.GetCustomWait(timeout: timeout)
                           .SetTimeoutMessage($"Couldn't get value of the DOM attribute '{attributeName}' of the '{GetType().Name}' with locator '{Locator}' during the timeout.")
                           .Until(_ => EvaluateElementStringFunction((element) => element, (element) => element.GetDomAttribute(attributeName)));
        Logger.LogVerbose($"Value of the DOM attribute '{attributeName}' of the '{GetType().Name}' with locator '{Locator}' is '{value}'.");
        return value;
    }

    /// <summary>
    /// Returns value of the IWebElement property.
    /// </summary>
    /// <param name="propertyName">Property name</param>
    /// <param name="timeout">How long the method will be wait until any value is get</param>
    /// <returns>string</returns>
    public virtual string GetDomProperty(string propertyName, uint timeout = 0)
    {
        string value = Wait.GetCustomWait(timeout: timeout)
                           .SetTimeoutMessage($"Couldn't get value of the DOM property '{propertyName}' of the '{GetType().Name}' with locator '{Locator}' during the timeout.")
                           .Until(_ => EvaluateElementStringFunction((element) => element, (element) => element.GetDomProperty(propertyName)));
        Logger.LogVerbose($"Value of the DOM property '{propertyName}' of the '{GetType().Name}' with locator '{Locator}' is '{value}'.");
        return value;
    }

    /// <summary>
    /// Returns value of the IWebElement attribute.
    /// </summary>
    /// <param name="attributeName">Attribute to be searched</param>
    /// <param name="value">Value of the attribute</param>
    /// <param name="timeout">How long the method will be wait until any value is get</param>
    /// <returns>string</returns>
    public bool TryGetDomAttribute(string attributeName, out string? value, uint timeout = 2)
    {
        value = null;
        try
        {
            value = GetDomAttribute(attributeName, timeout);
            Logger.LogVerbose($"Value of the DOM attribute '{attributeName}' of the '{GetType().Name}' with locator '{Locator}' is '{value}'.");
            return value is not null;
        }
        catch (WebDriverTimeoutException)
        {
            Logger.LogWarning($"Cannot find DOM attribute '{attributeName}' of the '{GetType().Name}' with locator '{Locator}'.");
            return false;
        }
    }

    /// <summary>
    /// Returns value of the innerText of the IWebElement.
    /// Warning: The WebElement.Text method returns text with normalized whitespace ('a   b' => 'a b').
    /// To keep these whitespace characters, use GetDomProperty("innerHTML") instead.
    /// </summary>
    /// <returns>string</returns>
    public virtual string GetText()
    {
        string text = Wait.SetTimeoutMessage($"Couldn't get text of the '{GetType().Name}' with locator '{Locator}' during the timeout.")
                          .Until(_ => EvaluateElementStringFunction((element) => element.Displayed ? element : null, (element) => element.Text));
        Logger.LogVerbose($"Text of the '{GetType().Name}' with locator '{Locator}' is '{text}'.");

        return text;
    }

    /// <summary>
    /// Method returns inner text of the node without text of all children
    /// </summary>
    /// <returns>Inner text of the node</returns>
    public virtual string GetInnerText()
    {
        string text = Wait.SetTimeoutMessage($"Couldn't get inner text (text of the '{GetType().Name}' without texts of descendants) of the '{GetType().Name}' with locator '{Locator}' during the timeout.")
                          .Until(_ => EvaluateElementStringFunction((element) => element.Displayed ? element : null, (element) => element.GetInnerText()));
        Logger.LogVerbose($"Text of the '{GetType().Name}' with locator '{Locator}' is '{text}'.");
        return text;
    }

    /// <summary>
    /// Method returns inner text of the node without text of all children no matter element is hidden or not.
    /// </summary>
    /// <returns>Inner text of the node</returns>
    public virtual string GetInnerTextOnly()
    {
        string text = Wait.SetTimeoutMessage($"Couldn't get inner text (text of the '{GetType().Name}' without texts of descendants) of the '{GetType().Name}' with locator '{Locator}' during the timeout.")
                          .Until(_ => EvaluateElementStringFunction((element) => element, (element) => element.GetInnerText()));
        Logger.LogVerbose($"Text of the '{GetType().Name}' with locator '{Locator}' is '{text}'.");
        return text;
    }

    /// <summary>
    /// Returns value of the css attribute of the IWebElement style attribute.
    /// </summary>
    /// <param name="cssAttribute">Name of the css attribute</param>
    /// <returns>string</returns>
    public virtual string GetCssValue(string cssAttribute)
    {
        string cssValue = Wait.SetTimeoutMessage($"Couldn't get value of the css attribute '{cssAttribute}' of the '{GetType().Name}' with locator '{Locator}' during the timeout.")
                              .Until(_ => EvaluateElementStringFunction((element) => element, (element) => element.GetCssValue(cssAttribute)));
        Logger.LogVerbose($"CSS value of the attribute '{cssAttribute}' of the '{GetType().Name}' with locator '{Locator}' is '{cssValue}'.");
        return cssValue;
    }

    /// <summary>
    /// Returns css attribute value of the pseudo element whose parent is <see cref="BaseComponent"/> using JS.
    /// For more info see https://www.lambdatest.com/blog/handling-pseudo-elements-in-css-with-selenium/.
    /// </summary>
    /// <param name="cssAttribute">Name of the css attribute</param>
    /// <param name="pseudoElementName">before, after, ...</param>
    /// <returns>string</returns>
    public virtual string GetCssValueOfPseudoElementJS(string cssAttribute, string pseudoElementName)
    {
        string? cssValue = JavaScriptExecutor.ExecuteScript($"return window.getComputedStyle(arguments[0],'::{pseudoElementName}').getPropertyValue('{cssAttribute}')", Element)?.ToString();
        Logger.LogVerbose($"CSS value of the attribute '{cssAttribute}' of the '{GetType().Name}' with locator '{Locator}' is '{cssValue}'.");
        return cssValue ?? string.Empty;
    }

    /// <summary>
    /// Methods returns location of the element on the page.
    /// </summary>
    /// <returns><see cref="Point"/></returns>
    public Point GetLocation() => Element.Location;

    /// <summary>
    /// Methods returns size of the element on the page.
    /// </summary>
    /// <returns><see cref="Size"/></returns>
    public Size GetSize() => Element.Size;

    /// <summary>
    /// Method returns the innerHTML of the element.
    /// </summary>
    /// <returns><see cref="IHtmlDocument"/></returns>
    public IHtmlDocument GetHtmlDocument()
    {
        HtmlParser html = new();
        return html.ParseDocument(GetDomProperty("innerHTML"));
    }

    /// <summary>
    /// Method returns the index of the first child element found by class name.
    /// </summary>
    /// <param name="className">Class name of the child element</param>
    /// <returns>Index of the child element</returns>
    public int GetIndexOfElementFoundByClassName(string className)
    {
        IElement? specificElement = GetHtmlDocument()?.GetElementsByClassName(className).FirstOrDefault();
        return specificElement?.Index() ?? -1;
    }
    #endregion Methods to get Properties/States of the Component

    #region Wait For Some Property of the Component Is Fulfilled Methods
    /// <summary>
    /// Waits for Element to be present in DOM
    /// </summary>
    public virtual void WaitForPresent() =>
        Wait.SetTimeoutMessage($"'{GetType().Name}' with locator '{Locator}' should be present on the page during the timeout.")
            .Until(_ => IsPresent());

    /// <summary>
    /// Waits for Element to be not present in DOM
    /// </summary>
    public virtual void WaitForNotPresent() =>
        Wait.SetTimeoutMessage($"'{GetType().Name}' with locator '{Locator}' shouldn't be present on the page during the timeout.")
            .Until(_ => IsNotPresent());

    /// <summary>
    /// Waits for Element to be displayed on page.
    /// </summary>
    public virtual void WaitForDisplayed() =>
        Wait.SetTimeoutMessage($"'{GetType().Name}' with locator '{Locator}' should be displayed on the page during the timeout.")
            .Until(_ => IsDisplayed());

    /// <summary>
    /// Waits for Element to be not displayed on page.
    /// </summary>
    public virtual void WaitForNotDisplayed() =>
        Wait.SetTimeoutMessage($"'{GetType().Name}' with locator '{Locator}' shouldn't be displayed on the page during the timeout.")
            .Until(_ => IsNotDisplayed());

    /// <summary>
    /// Wait for element to be enabled.
    /// </summary>
    public virtual void WaitForEnabled() =>
        Wait.SetTimeoutMessage($"'{GetType().Name}' with locator '{Locator}' should be enabled on the page during the timeout.")
            .Until(_ => IsEnabled());

    /// <summary>
    /// Wait for element to be disabled.
    /// </summary>
    public virtual void WaitForDisabled() =>
        Wait.SetTimeoutMessage($"'{GetType().Name}' with locator '{Locator}' should be disabled on the page during the timeout.")
            .Until(_ => IsDisabled());

    /// <summary>
    /// Method scrolls to the element so that it appears in center of viewport.
    /// </summary>
    public virtual void ScrollTo()
    {
        Logger.LogVerbose($"Scrolling to '{GetType().Name}' with locator '{Locator}'.");
        EvaluateElementBoolFunction((element) => element, (element) => element.ScrollToJS());
        WaitForReady();
    }

    /// <summary>
    /// Method scrolls to the parent element first, then it scrolls to child element so that it appears in the center of the parent
    /// viewport.
    /// For example: react component (this), whose part (childComponent) is lazy loaded, it's necessary to scroll
    /// to the this component first, childComponent is loaded and after this it can be possible to scroll to childComponent
    /// </summary>
    /// <param name="childComponent"><see cref="BaseComponent"/> placed inside of this component</param>
    public virtual void ScrollTo(BaseComponent childComponent)
    {
        WaitForPresent();
        ScrollTo();
        WaitForReady();

        childComponent.WaitForPresent();
        childComponent.ScrollTo();
    }
    #endregion Wait For Some Property of the Component Is Fulfilled Methods

    #region Methods to Interact With the Component
    /// <summary>
    /// Scrolls to an element and clicks on it.
    /// </summary>
    public virtual void ScrollToAndClick()
    {
        ScrollTo();
        Click();
        WaitForReady(); // needed for the price to update in order1
    }

    /// <summary>
    /// Scrolls to an element and clicks on it using JS only for Click method.
    /// </summary>
    public virtual void ScrollToAndClickJS()
    {
        ScrollTo();
        ClickJS();
        WaitForReady();
    }

    /// <summary>
    /// Clicks on element.
    /// </summary>
    public virtual void Click() =>
        Wait.SetTimeoutMessage($"'{GetType().Name}' with locator '{Locator}' wasn't clicked during the timeout.")
            .Until(_ => IsClicked());

    /// <summary>
    /// Returns true/false if Element is clicked
    /// </summary>
    /// <returns>boolean</returns>
    public virtual bool IsClicked() =>
        EvaluateElementBoolFunction((element) => element.Displayed ? element : null, (element) => element.Click(), webDriverExc: true);

    /// <summary>
    /// Clicks on element using JS.
    /// </summary>
    public virtual void ClickJS() => JavaScriptExecutor.ExecuteScript("arguments[0].click();", Element);

    /// <summary>
    /// Clicks on Element only if it is present in DOM
    /// </summary>
    /// <returns>boolean</returns>
    public virtual bool ClickIfDisplayed() => ClickIfDisplayed(Click);

    /// <summary>
    /// Helper method for click methods.
    /// </summary>
    /// <returns>boolean</returns>
    public virtual bool ClickIfDisplayed(Action action)
    {
        if (IsDisplayed())
        {
            action();
            return true;
        }
        return false;
    }

    /// <summary>
    /// Clicks using JS on Element only if it is present in DOM
    /// </summary>
    /// <returns>boolean</returns>
    public virtual bool ClickJSIfDisplayed() => ClickIfDisplayed(ClickJS);

    /// <summary>
    /// Move mouse to element and click on it.
    /// </summary>
    public virtual void HoverAndClick()
    {
        Action.MoveToElement(Element).Click().Perform();
        WaitForReady();
    }

    /// <summary>
    /// Move mouse to element.
    /// </summary>
    public virtual void Hover()
    {
        Action.MoveToElement(Element).Perform();
        WaitForReady();
    }

    /// <summary>
    /// Method performs double click on the element.
    /// </summary>
    public virtual void DoubleClick()
    {
        Action.DoubleClick(Element).Perform();
        WaitForReady();
    }
    #endregion Methods to Interact With the Component

    #region Find Element(s) Methods
    /// <summary>
    /// Finds the first IWebElement using the given method.
    /// </summary>
    /// <param name="by"><see cref="By"/></param>
    /// <returns>IWebElement</returns>
    public IWebElement FindElement(By by)
    {
        Logger.LogVerbose($"Trying to find '{GetType().Name}' with locator '{by}'.");
        return Element.FindElement(by);
    }

    /// <summary>
    /// Finds the first IWebElement using the given method.
    /// </summary>
    /// <param name="by"><see cref="By"/></param>
    /// <returns>IWebElement</returns>
    public IWebElement FindWebElement(By by)
    {
        Logger.LogVerbose($"Trying to find '{GetType().Name}' with locator '{by}'.");
        return Element.FindWebElement(by);
    }

    /// <summary>
    /// Finds all IWebElements within the current context using the given mechanism.
    /// </summary>
    /// <param name="by"><see cref="By"/></param>
    /// <returns>A <see cref="ReadOnlyCollection{T}"/> of all WebElements matching the current criteria, or an empty list if nothing matches.</returns>
    public virtual ReadOnlyCollection<IWebElement> FindElements(By by)
    {
        return Element.FindElements(by);
    }

    /// <summary>
    /// Finds all IWebElements within the current context using the given mechanism.
    /// </summary>
    /// <param name="by"><see cref="By"/></param>
    /// <returns>A <see cref="ReadOnlyCollection{T}"/> of all WebElements matching the current criteria, or an empty list if nothing matches.</returns>
    public virtual ReadOnlyCollection<IWebElement> FindWebElements(By by)
    {
        try
        {
            return Element.FindWebElements(by);
        }
        catch (StaleElementReferenceException)
        {
            Logger.LogVerbose($"Catching '{nameof(StaleElementReferenceException)}'. Trying to find elements again.");
            try
            {
                return Element.FindWebElements(by);
            }
            catch (StaleElementReferenceException)
            {
                Logger.LogFatal($"Catching '{nameof(StaleElementReferenceException)}' again. Please, investigate if there is missing some wait-for step to avoid this exception.");
                throw;
            }
        }
    }

    /// <summary>
    /// Finds all components {T} within the current context using the xpath.
    /// </summary>
    /// <param name="by">Only <see cref="By.XPath(string)"/> can be used in this method</param>
    /// <typeparam name="T">Any component that extends <see cref="BaseComponent"/></typeparam>
    /// <returns>Collection of {T} with filled locator</returns>
    public ReadOnlyCollection<T> FindWebElementsByXPath<T>(By by) where T : BaseComponent, new()
    {
        if (by.Mechanism != "xpath")
        {
            throw new UnsupportedOperationException($"Used mechanism '{by.Mechanism}' isn't supported; please use only 'By.XPath' mechanism.");
        }

        var components = new List<T>();
        var count = Element.FindWebElements(by).Count;
        for (int i = 1; i <= count; i++)
        {
            By childLocator = By.XPath($"{by.ToSelector()}[{i}]");
            T component = new() { Locator = childLocator };
            components.Add(component);
        }
        return components.AsReadOnly();
    }
    #endregion Find Element(s) Methods

    #region Helper Methods
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

    /// <summary>
    /// Method returns result of getElementCondition and elementAction if present.
    /// If <see cref="StaleElementReferenceException"/> is raised, exception is caught and Evaluate method is called again.
    /// Example: Trying to evaluate IsPresent method. If <see cref="StaleElementReferenceException"/> was thrown, we don't know if element is really in the DOM,
    /// so we can try to run new evaluation of the getElementFunction/getElementAction.
    /// </summary>
    /// <param name="getElementCondition">Method returns IWebElement represents that getElementCondition was fulfilled, and elementAction can be performed.
    ///                                   If the method returns null, getElementCondition is not fulfilled and method returns false.</param>
    /// <param name="elementAction">Action can be performed when getElementCondition is fulfilled (returns not null).</param>
    /// <param name="noElement">If <see cref="NoSuchElementException"/> was thrown, element wasn't found.
    ///                         Example: Trying to evaluate IsPresent method. If <see cref="NoSuchElementException"/> was thrown, element is not present in the DOM.
    ///                         If the page/data is rendered/loaded slowly, and we are sure that element is on the page, we can try to evaluate this function repeatedly.</param>
    /// <param name="webDriverExc">We can influence what must happen when <see cref="WebDriverException"/> is raised. This is unexpected exception, and we can catch it,
    ///                            so the test does not fail. It depends on behavior of the test logic.</param>
    /// <returns>True/false if the condition is fulfilled and action performed.</returns>
    protected bool EvaluateElementBoolFunction(Func<IWebElement, IWebElement?> getElementCondition, Action<IWebElement>? elementAction, bool noElement = false,
        bool webDriverExc = false)
    {
        IWebElement? element = null;
        try
        {
            Logger.LogVerbose($"Trying to evaluate method '{getElementCondition.Method.Name}' on '{GetType().Name}' with locator '{Locator}'.");
            element = getElementCondition((Context ?? throw new InvalidOperationException("Search context is null.")).FindElement(Locator));
            if (element != null)
            {
                if (elementAction != null)
                {
                    Logger.LogVerbose($"Trying to evaluate action '{elementAction.Method.Name}' on '{GetType().Name}' with locator '{Locator}'.");
                    elementAction.Invoke(element);
                }
                return true;
            }
            else
            {
                Logger.LogVerbose($"'{GetType().Name}' with locator '{Locator}' matching the condition not found.");
                return false;
            }
        }
        catch (StaleElementReferenceException)
        {
            Logger.LogVerbose("Catching stale exception");
            if (new StackTrace().GetFrames()?[1].GetMethod()?.Name == nameof(EvaluateElementBoolFunction))
                throw;
            return EvaluateElementBoolFunction(getElementCondition, elementAction, noElement, webDriverExc);
        }
        catch (NoSuchElementException)
        {
            Logger.LogVerbose("Catching no such element exception.");
            return noElement;
        }
        catch (WebDriverException wde)
        {
            Logger.LogVerbose(wde, "Catching WebDriverException.");
            return webDriverExc;
        }
    }

    /// <summary>
    /// Method returns result of getElementCondition and elementAction if present. Used for action with IWebElement and string arguments (e.g. SendKeys).
    /// If <see cref="StaleElementReferenceException"/> is raised, exception is caught and Evaluate method is called again.
    /// Example: Trying to evaluate IsPresent method. If <see cref="StaleElementReferenceException"/> was thrown, we don't know if element is really in the DOM,
    /// so we can try to run new evaluation of the getElementFunction/getElementAction.
    /// </summary>
    /// <param name="getElementCondition">Method returns IWebElement represents that getElementCondition was fulfilled, and elementAction can be performed.
    ///                                   If returns null, getElementCondition is not fulfilled and method returns false.</param>
    /// <param name="elementAction">Action can be performed when getElementCondition is fulfilled (returns not null).</param>
    /// <param name="text">Input parameter of the action method.</param>
    /// <param name="noElement">If <see cref="NoSuchElementException"/> was thrown, element wasn't found.
    ///                         Example: Trying to evaluate IsPresent method. If <see cref="NoSuchElementException"/> was thrown, element is not present in the DOM.
    ///                         If the page/data is rendered/loaded slowly, and we are sure that element is on the page, we can try to evaluate this function repeatedly.</param>
    /// <param name="webDriverExc">We can influence what must happen when <see cref="WebDriverException"/> is raised. This is unexpected exception, and we can catching it,
    ///                            so the test does not fail. It depends on behavior of the test logic.</param>
    /// <returns>True/false if the condition is fulfilled and action performed.</returns>
    protected bool EvaluateElementBoolFunction(Func<IWebElement, IWebElement> getElementCondition, Action<IWebElement, string> elementAction, string text,
        bool noElement = false, bool webDriverExc = false)
    {
        try
        {
            Logger.LogVerbose($"Trying to evaluate method '{getElementCondition.Method.Name}' on '{GetType().Name}' with locator '{Locator}'.");
            IWebElement element = getElementCondition((Context ?? throw new InvalidOperationException("Search context is null.")).FindElement(Locator));
            if (element != null)
            {
                if (elementAction != null)
                {
                    Logger.LogVerbose($"Trying to evaluate action '{elementAction.Method.Name}' on '{GetType().Name}' with locator '{Locator}'.");
                    elementAction.Invoke(element, text);
                }
                return true;
            }
            else
            {
                Logger.LogVerbose($"'{GetType().Name}' with locator '{Locator}' matching the condition not found.");
                return false;
            }
        }
        catch (StaleElementReferenceException)
        {
            Logger.LogVerbose("Catching stale exception");
            if (new StackTrace().GetFrames()?[1].GetMethod()?.Name == nameof(EvaluateElementBoolFunction))
                throw;
            return EvaluateElementBoolFunction(getElementCondition, elementAction, text, noElement, webDriverExc);
        }
        catch (NoSuchElementException)
        {
            Logger.LogVerbose("Catching no such element exception.");
            return noElement;
        }
        catch (WebDriverException wde)
        {
            Logger.LogVerbose(wde, "Catching WebDriverException.");
            return webDriverExc;
        }
    }

    /// <summary>
    /// Method returns string that represents value of the IWebElement property.
    /// </summary>
    /// <param name="getElementCondition">Method returns IWebElement represents that getElementCondition was fulfilled, and elementAction can be performed.
    ///                                   If returns null, getElementCondition is not fulfilled and method returns false</param>
    /// <param name="elementAction">Action can be performed when getElementCondition is fulfilled (returns not null).</param>
    /// <returns>String that represents value of the IWebElement property.</returns>
    protected string? EvaluateElementStringFunction(Func<IWebElement, IWebElement?> getElementCondition, Func<IWebElement, string?> elementAction)
    {
        try
        {
            Logger.LogVerbose($"Trying to evaluate method '{getElementCondition.Method.Name}' on '{GetType().Name}' with locator '{Locator}'.");
            IWebElement? element = getElementCondition((Context ?? throw new InvalidOperationException("Search context is null.")).FindElement(Locator));
            if (element != null)
            {
                return elementAction(element);
            }
            else
            {
                Logger.LogVerbose($"'{GetType().Name}' with locator '{Locator}' matching the condition not found.");
                return null;
            }
        }
        catch (StaleElementReferenceException)
        {
            Logger.LogVerbose("Catching stale exception.");
            return null;
        }
        catch (NoSuchElementException)
        {
            Logger.LogVerbose("Catching no such element exception.");
            return null;
        }
    }
    #endregion Helper Methods
}

using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace UI.Template.Components.Basic;

/// <summary>
/// Wraps IWebElement and acts as a select
/// </summary>
public class DropDownList : BaseComponent
{
    public DropDownList(By locator) : base(locator) { }

    public DropDownList(By locator, ISearchContext searchContext) : base(locator, searchContext) { }

    /// <summary>
    /// Select first option of the drop-down list component.
    /// </summary>
    public void SelectFirst()
    {
        SelectElement selectElement = new(Element);
        selectElement.SelectByIndex(0);
    }

    /// <summary>
    /// Select option by index in the drop-down list component.
    /// </summary>
    public void SelectByIndex(int index)
    {
        SelectElement selectElement = new(Element);
        selectElement.SelectByIndex(index);
    }

    /// <summary>
    /// Select last option of the drop-down list component.
    /// </summary>
    public void SelectLast()
    {
        SelectElement selectElement = new(Element);
        selectElement.SelectByIndex(selectElement.Options.Count - 1);
    }

    /// <summary>
    /// Select next option of the drop-down list component.
    /// </summary>
    /// <returns>True if position was set, false if index of the new set option is out of range.</returns>
    public bool SelectNext()
    {
        SelectElement selectElement = new(Element);
        int currentPosition = selectElement.Options.IndexOf(selectElement.SelectedOption);
        int nextPosition = currentPosition + 1;
        if (nextPosition >= selectElement.Options.Count)
        {
            Logger.LogWarning("Selected value is out of range.");
            return false;
        }
        selectElement.SelectByIndex(nextPosition);
        return true;
    }

    /// <summary>
    /// Select option of the drop-down list component by its value attribute.
    /// </summary>
    /// <param name="value">string representation of the value to be selected</param>
    public void SelectByValue(string value)
    {
        SelectElement selectElement = new(Element);
        selectElement.SelectByValue(value);
    }

    /// <summary>
    /// Select option of the drop-down list component by its text value.
    /// </summary>
    /// <param name="text">the text value to be selected</param>
    public void SelectByText(string text)
    {
        SelectElement selectElement = new(Element);
        selectElement.SelectByText(text);
    }

    /// <summary>
    /// Function returns dictionary of the options.
    /// </summary>
    /// <returns><see cref="Dictionary{int, string}"/> where key is the index of the option and value is text of the option</returns>
    public Dictionary<int, string> GetOptions()
    {
        Dictionary<int, string> options = [];

        SelectElement selectElement = new(Element);
        int optionsCount = selectElement.Options.Count;

        for (int i = 0; i < optionsCount; i++)
            options.Add(i, selectElement.Options[i].Text);

        return options;
    }

    /// <summary>
    /// Function returns value of the selected option.
    /// </summary>
    /// <returns>Value of the selected option</returns>
    public string GetSelectedOptionValue()
    {
        SelectElement selectElement = new(Element);
        return selectElement.SelectedOption.GetDomProperty("value") ?? string.Empty;
    }

    /// <summary>
    /// Function returns text of the selected option.
    /// </summary>
    /// <returns>Text of the selected option</returns>
    public string GetSelectedOptionText()
    {
        SelectElement selectElement = new(Element);
        return selectElement.SelectedOption.Text;
    }
}

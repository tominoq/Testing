using OpenQA.Selenium;
using UI.Template.Components.Basic;
using UI.Template.Framework.Extensions;

namespace UI.Template.Components.Containers;

public class CategoryContainer(By locator) : BaseComponent(locator)
{
    private readonly Simple _categoryList = new(By.XPath("//ul[@class='category-list']"));
    private readonly Simple CurrentCategory = new(By.XPath("//li[@class='active']"));

    public void Back() => WebDriver.WaitForUrlChanged(WebDriver.Navigate().Back);

    public override bool IsDisplayed() => Wait.TryWaitWithCondition(() => base.IsDisplayed() && _categoryList.IsDisplayed());

    /// <inheritdoc/>
    public override void WaitForReady()
    {
        Wait.Until(_ => IsDisplayed());
    }

    /// <summary>
    /// Select a first category from the list
    /// </summary>
    /// <param name="categoryName">The category name to select</param>
    public void SelectCategory(string categoryName)
    {
        Simple Category = new(By.XPath($"//li[contains (text(),'{categoryName}')]"));
        Category.Click();
        WaitForReady();
    }

    /// <summary>
    /// Get the current selected category name
    /// </summary>
    /// <returns>The current category name</returns>
    public string GetCurrentCategory() => CurrentCategory.GetText();
}

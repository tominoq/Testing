using System.Globalization;
using OpenQA.Selenium;
using UI.Template.Components.Basic;
using UI.Template.Framework.Extensions;

namespace UI.Template.Components.Containers;

public class EditProductContainer(By locator) : BaseComponent(locator)
{
    public TextInput Name => new(By.XPath($"{Locator.ToSelector()}//*[@id='name']"));
    public DropDownList Category => new(By.XPath($"{Locator.ToSelector()}//*[@id='category']"));
    public TextInput Price => new(By.XPath($"{Locator.ToSelector()}//*[@id='price']"));
    public TextInput Stock => new(By.XPath($"{Locator.ToSelector()}//*[@id='stock']"));
    public DropDownList Image => new(By.XPath($"{Locator.ToSelector()}//*[@id='imageUrl']"));
    public TextInput Description => new(By.XPath($"{Locator.ToSelector()}//*[@id='description']"));
    public Button SaveButton => new(By.XPath($"{Locator.ToSelector()}//button[@class='save-button']"));

    /// <summary>
    /// Sets the name of the product.
    /// </summary>
    /// <param name="value">The name to set.</param>
    /// <returns>True if the name was set correctly, false otherwise.</returns>
    public bool SetName(string value)
    {
        Name.Clear();
        Name.SendKeys(value);
        return Name.GetValue().Equals(value, StringComparison.Ordinal);
    }

    /// <summary>
    /// Selects category from the dropdown.
    /// </summary>
    /// <param name="categoryName">The category name for selection.</param>
    /// <returns>True if the category was selected correctly, false otherwise.</returns>
    public bool SelectCategory(string categoryName)
    {
        Category.SelectByText(categoryName);
        return Category.GetSelectedOptionText() == categoryName;
    }

    /// <summary>
    /// Sets the price of the product.
    /// </summary>
    /// <param name="value">The price to set.</param>
    /// <returns>True if the price was set correctly, false otherwise.</returns>
    public bool SetPrice(int value)
    {
        Price.Clear();
        Price.SendKeys(value.ToString(CultureInfo.InvariantCulture));
        return int.Parse(Price.GetValue(), CultureInfo.InvariantCulture) == value;
    }

    /// <summary>
    /// Opens the basket container.
    /// </summary>
    public bool SetStock(int value)
    {
        Stock.Clear();
        Stock.SendKeys(value.ToString(CultureInfo.InvariantCulture));
        return int.Parse(Stock.GetValue(), CultureInfo.InvariantCulture) == value;
    }

    /// <summary>
    /// Selects image from the dropdown.
    /// </summary>
    public bool SelectImage(string imageName)
    {
        Image.SelectByText(imageName);
        return Image.GetSelectedOptionText() == imageName;
    }

    /// <summary>
    /// Sets the description of the product.
    /// </summary>
    /// <param name="value">The description to set.</param>
    /// <returns>True if the description was set correctly, false otherwise.</returns>
    public bool SetDescription(string value)
    {
        Description.Clear();
        Description.SendKeys(value);
        return Description.GetValue().Equals(value, StringComparison.Ordinal);
    }

    /// <summary>
    /// Clicks the UpdateProductButton button.
    /// </summary>
    public void SaveChanges()
    {
        SaveButton.Click();
        WaitForNotDisplayed();
    }

}

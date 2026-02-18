using OpenQA.Selenium;
using UI.Template.Components.Basic;
using UI.Template.Components.Containers;
using UI.Template.Framework.Extensions;

namespace UI.Template.Components;

public class AdminProductCard(By locator) : BaseProductInfo(locator)
{
    public override Simple Name => new(By.XPath($"{Locator.ToSelector()}//h3[@class='product-name']"));
    public override Simple Price => new(By.XPath($"{Locator.ToSelector()}//span[text()='Price:']/parent::div"));
    public override Simple StockStatus => new(By.XPath($"{Locator.ToSelector()}//span[text()='Stock:']/parent::div"));
    private Simple Image => new(By.XPath($"{Locator.ToSelector()}//img[@class='product-image']"));
    private Button EditCartButton => new(By.XPath($"{Locator.ToSelector()}//button[@class='edit-button']"));

    /// <inheritdoc/>
    public override bool IsReady()
    {
        return base.IsReady() && IsDisplayed();
    }

    /// <summary>
    /// Opens the edit product container.
    /// </summary>
    public EditProductContainer EditProduct()
    {
        EditCartButton.ScrollToAndClick();
        EditProductContainer editProductContainer = new(By.XPath("//div[@class='modal-overlay']"));
        editProductContainer.WaitForDisplayed();
        return editProductContainer;
    }

    /// <summary>
    /// Gets the product image URL.
    /// </summary>
    /// <returns>The product image URL.</returns>
    public string GetImageUrl() => Image.GetDomProperty("src");
}

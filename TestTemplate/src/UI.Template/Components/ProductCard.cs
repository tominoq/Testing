using OpenQA.Selenium;
using UI.Template.Components.Basic;
using UI.Template.Framework.Extensions;
using UI.Template.Pages;

namespace UI.Template.Components;

public class ProductCard(By locator) : BaseProductInfo(locator)
{
    private const string _productInfoLocator = "//a[@class='product-link']";
    public override Simple Name => new(By.XPath($"{Locator.ToSelector()}{_productInfoLocator}//h3"));
    public override Simple Price => new(By.XPath($"{Locator.ToSelector()}{_productInfoLocator}//p[@class='price']"));
    public override Simple StockStatus => new(By.XPath($"{Locator.ToSelector()}{_productInfoLocator}//p[contains(@class, 'stock-status')]"));
    private Button AddToCartButton => new(By.XPath($"{Locator.ToSelector()}//button[@class='add-to-cart']"));

    /// <inheritdoc/>
    public override bool IsReady()
    {
        return base.IsReady() && IsDisplayed();
    }

    /// <summary>
    /// Add the product to the basket. You must check the basket count in a test/page.
    /// </summary>
    public void AddToBasket() => AddToCartButton.Click();

    /// <summary>
    /// Opens the product detail page.
    /// </summary>
    /// <returns>A new instance of the <see cref="ProductDetailPage"/>.</returns>
    public ProductDetailPage OpenProductDetail()
    {
        Name.Click();
        ProductDetailPage productDetailPage = new();
        productDetailPage.WaitForReady();
        return productDetailPage;
    }
}

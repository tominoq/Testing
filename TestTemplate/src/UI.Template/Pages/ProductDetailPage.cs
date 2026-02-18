using OpenQA.Selenium;
using UI.Template.Components;

namespace UI.Template.Pages;

public class ProductDetailPage(string url = "/product/") : BaseEshopPage(url)
{
    public ProductInfo ProductInfoForm { get; private set; } = new(By.XPath("//div[@class='product-info']"));

    /// <inheritdoc/>
    public override bool IsReady()
    {
        return base.IsReady() && ProductInfoForm.IsDisplayed();
    }
}

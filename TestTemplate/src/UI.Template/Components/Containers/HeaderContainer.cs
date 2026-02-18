using OpenQA.Selenium;
using UI.Template.Components.Basic;
using UI.Template.Framework.Extensions;
using UI.Template.Pages;

namespace UI.Template.Components.Containers;

public class HeaderContainer : BaseComponent
{
    private readonly Simple OpenBasketButton = new(By.XPath("//*[@class='cart-widget']"));
    private readonly Simple BasketCount = new(By.XPath("//*[@class='cart-count']"));
    private readonly Simple BasketContainer = new(By.XPath("//div[@class='cart-view']"));
    private readonly Button ClearBasketButton = new(By.XPath("//button[@class='clear-cart']"));
    private readonly Button CloseBasketButton = new(By.XPath("//button[@class='close-cart']"));
    private readonly Simple Title = new(By.XPath("//h1[@class='shop-title']"));

    /// <summary>
    /// Opens the basket container.
    /// </summary>
    public void OpenBasketContainer()
    {
        if (BasketContainer.IsNotDisplayed())
        {
            OpenBasketButton.Click();
            BasketContainer.WaitForDisplayed();
        }
    }

    /// <summary>
    /// Clears all items from the basket.
    /// </summary>
    /// <returns>
    /// The current basket count.
    /// </returns>
    public int GetBasketCount()
    {
        if (BasketCount.IsNotDisplayed())
        {
            return 0;
        }

        return int.TryParse(BasketCount.GetText(), out int count) ?
                            count :
                            throw new InvalidOperationException("Failed to parse basket count.");
    }

    /// <summary>
    /// Hide the basket container.
    /// </summary>
    public void CloseBasketContainer()
    {
        if (BasketContainer.IsDisplayed())
        {
            CloseBasketButton.Click();
            BasketContainer.WaitForNotDisplayed();
        }
    }

    /// <summary>
    /// Gets a list of product names currently in the basket.
    /// </summary>
    /// <returns>A list of product names.</returns>
    public List<string> GetProductNamesInBasket()
    {
        if (BasketContainer.IsNotDisplayed())
        {
            throw new InvalidOperationException("Basket container is not open. Please open the basket before retrieving product names.");
        }

        List<string> productNames = new List<string>();
        By productPathLocator = By.XPath("//*[@class='cart-item-details']");

        int productCardsCount = WebDriver.FindElements(productPathLocator).Count;
        for (int i = 1; i <= productCardsCount; i++)
        {
            Simple productCard = new(By.XPath($"({productPathLocator.ToSelector()}/h3)[{i}]"));
            productCard.ScrollTo();
            productNames.Add(productCard.GetText());
        }

        return productNames;
    }

    /// <summary>
    /// Gets the nth product in the basket.
    /// </summary>
    /// <param name="n">The index of the product to get (1-based).</param>
    /// <param name="productName">The name of the product.</param>
    /// <param name="productDetail">The detail of the product.</param>
    /// <returns>True if the product was found, false otherwise.</returns>
    public bool GetNthProduct(int n, out string productName, out string productDetail)
    {
        productName = string.Empty;
        productDetail = string.Empty;

        string nthProductXPathSelector = $"(//*[@class='cart-item-details'])[{n}]";

        Simple nthProduct = new(By.XPath(nthProductXPathSelector));
        Simple nthProductName = new(By.XPath($"{nthProductXPathSelector}/h3"));
        Simple nthProductDetail = new(By.XPath($"{nthProductXPathSelector}/p"));

        if (BasketContainer.IsDisplayed() && nthProduct.IsDisplayed())
        {
            productName = nthProductName.GetText();
            productDetail = nthProductDetail.GetText();
            return true;
        }

        return false;
    }

    /// <summary>
    /// Open the admin page
    /// </summary>
    /// <returns>The Admin page</returns>
    public AdminPage OpenAdminPage()
    {
        WebDriver.WaitForUrlChanged(() => Title.Click());
        return new AdminPage();
    }
}

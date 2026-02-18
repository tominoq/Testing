using OpenQA.Selenium;
using UI.Template.Components;
using UI.Template.Components.Basic;
using UI.Template.Components.Containers;
using UI.Template.Framework.Extensions;

namespace UI.Template.Pages;

public class AdminPage() : BasePage("/admin")
{
    private readonly Button _backToEshopButton = new(By.XPath("//button[@class='back-button']"));
    private readonly Button _addProductButton = new(By.XPath("//button[@class='add-button']"));
    private readonly AdminProductGridContainer _productsGrid = new AdminProductGridContainer(By.XPath("//*[@class='product-grid']"));

    /// <summary>
    /// Tries to get a admin product card by product name.
    /// </summary>
    /// <param name="productName">The name of the product.</param>
    /// <param name="productCard">The found product card or null.</param>
    /// <returns>True if product card was found, false otherwise.</returns>
    public bool TryGetProductCardByName(string productName, out AdminProductCard? productCard)
    {
        Dictionary<string, AdminProductCard> productCards = _productsGrid.GetProductCards();
        return productCards.TryGetValue(productName, out productCard);
    }

    /// <summary>
    /// Opens the Add New Product container.
    /// </summary>
    /// <returns>The adding new product container</returns>
    public EditProductContainer OpenAddNewProductContainer()
    {
        _addProductButton.ScrollToAndClick();
        EditProductContainer editProductContainer = new(By.XPath("//div[@class='modal-overlay']"));
        editProductContainer.WaitForDisplayed();
        return editProductContainer;
    }

    /// <summary>
    /// Navigates to the e-shop home page.
    /// </summary>
    /// <returns>The Home page</returns>
    public HomePage GoToEshopHome()
    {
        WebDriver.WaitForUrlChanged(() => _backToEshopButton.Click());
        return new HomePage();
    }
}

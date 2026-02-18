using OpenQA.Selenium;
using UI.Template.Components;
using UI.Template.Components.Containers;

namespace UI.Template.Pages;

public class HomePage() : BaseEshopPage("/")
{
    private readonly CategoryContainer _categories = new CategoryContainer(By.XPath("//*[@class='categories-sidebar']"));
    private readonly ProductGridContainer _productsGrid = new ProductGridContainer(By.XPath("//*[@class='products-grid']"));

    /// <summary>
    /// Adds the first product from a specific category to the basket.
    /// </summary>
    /// <param name="category">The name of the category.</param>
    /// <returns>True if the product was added to the basket, false otherwise.</returns>
    public bool AddToBasketFirstProductFromCategory(string category)
    {
        int basketCountBefore = Header.GetBasketCount();

        _categories.SelectCategory(category);
        Dictionary<string, ProductCard> productCards = _productsGrid.GetProductCards();
        if (productCards.Count == 0)
        {
            return false;
        }

        productCards.First().Value.AddToBasket();

        return Header.GetBasketCount() == basketCountBefore + 1;
    }

    /// <summary>
    /// Opens a product detail page by its name from a specific category.
    /// </summary>
    /// <param name="category">The name of the category.</param>
    /// <param name="product">The name of the product.</param>
    /// <returns></returns>
    public ProductDetailPage OpenProductByNameFromCategory(string category, string product)
    {
        _categories.SelectCategory(category);
        Dictionary<string, ProductCard> productCards = _productsGrid.GetProductCards();

        if (!productCards.TryGetValue(product, out ProductCard? value))
        {
            throw new NoSuchElementException("Product \"" + product + "\" not found in category \"" + category + "\".");
        }

        return value.OpenProductDetail();
    }

    /// <summary>
    /// Tries to find a product card by its name on the current category.
    /// </summary>
    /// <param name="productName"></param>
    /// <param name="productCard"></param>
    /// <returns></returns>
    public bool TryGetProductCardByName(string productName, out ProductCard productCard)
    {
        Dictionary<string, ProductCard> productCards = _productsGrid.GetProductCards();

        if (!productCards.TryGetValue(productName, out ProductCard? value))
        {
            productCard = null!;
            return false;
        }

        productCard = value;
        return true;
    }

    /// <summary>
    /// Returns the name of the currently selected category.
    /// </summary>
    /// <returns></returns>
    public string GetCurrentCategory()
    {
        return _categories.GetCurrentCategory();
    }
}

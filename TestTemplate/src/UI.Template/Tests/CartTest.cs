using UI.Template.Data;
using UI.Template.Pages;

namespace UI.Template.Tests;

[TestFixture]
public class CartTest : BaseTest
{
    [Test]
    public void AddProductToCartTest()
    {
        //** STEP 1 ***/
        HomePage homePage = new HomePage();
        homePage.Open();

        //** STEP 2 ***/
        Assert.That(homePage.GetCurrentCategory(), Is.EqualTo("All"), "The current category is not 'All'");

        //** STEP 3 ***/
        ProductDetailPage productDetail = homePage.OpenProductByNameFromCategory(TestData.CardTestProduct.ProductCategory, TestData.CardTestProduct.ProductName);

        //** STEP 4 ***/
        productDetail.ProductInfoForm.AddToCart();
        Assert.That(productDetail.Header.GetBasketCount(), Is.EqualTo(1), "Basket count is not 1. Check Add to Cart functionality.");

        //** STEP 5 ***/
        productDetail.Header.OpenBasketContainer();
        Assert.That(productDetail.Header.GetNthProduct(1, out string productName, out _), Is.True, "The first product in the basket was not found");
        Assert.That(productName, Is.EqualTo(TestData.CardTestProduct.ProductName), "The name of product in the basket is not same as in test data");

        //** STEP 6 ***/
        productDetail.Header.CloseBasketContainer();

        //** STEP 7 ***/
        productDetail.ProductInfoForm.BackToShop();
        Assert.That(homePage.GetCurrentCategory(), Is.EqualTo("All"), "The current category is not 'All'");
    }
}

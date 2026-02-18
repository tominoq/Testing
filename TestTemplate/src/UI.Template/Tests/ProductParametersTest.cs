using UI.Template.Components;
using UI.Template.Data;
using UI.Template.Framework.Extensions;
using UI.Template.Models;
using UI.Template.Pages;

namespace UI.Template.Tests;

[TestFixture]
public class ProductParametersTest : BaseTest
{
    [Test]
    public void CheckProductParametersTest()
    {
        /*** STEP 1 ***/
        HomePage homePage = new HomePage();
        homePage.Open();

        homePage.WaitForReady();

        Assert.Multiple(() =>
        {
            Assert.That(WebDriver.UrlPath(), Is.EqualTo("/"), "Homepage is not opened");
            Assert.That(homePage.GetCurrentCategory(), Is.EqualTo("All"), "Active category does not match expected 'All'");
        });


        /*** STEP 2 ***/
        if (!homePage.TryGetProductCardByName(TestData.ParametersTestProduct.ProductName, out ProductCard productCard))
        {
            Assert.Fail($"Product '{TestData.ParametersTestProduct.ProductName}' not found on the home page.");
        }

        productCard.WaitForReady();
        Product productModelFromCard = productCard.ToProductModel();

        Assert.Multiple(() =>
        {
            Assert.That(productModelFromCard.Name, Is.Not.Null.And.Not.Empty, "Failed to find product name");
            Assert.That(productModelFromCard.Price, Is.GreaterThan(0), "Failed to find product price");
            Assert.That(productModelFromCard.Stock, Is.GreaterThan(0), "Failed to find product availability");
        });


        /*** STEP 3 ***/
        ProductDetailPage productDetail = homePage.OpenProductByNameFromCategory(TestData.ParametersTestProduct.ProductCategory,
                                                                                 TestData.ParametersTestProduct.ProductName);

        productDetail.WaitForReady();
        Product productModelFromDetail = productDetail.ProductInfoForm.ToProductModel();

        Assert.That(WebDriver.UrlPath(), Is.EqualTo(TestData.ParametersTestProduct.ProductUrl), "Product URL does not match expected URL");


        /*** STEP 4 ***/
        Assert.Multiple(() =>
        {
            Assert.That(productModelFromDetail.Name, Is.Not.Null.And.Not.Empty, "Failed to find product name");
            Assert.That(productModelFromDetail.Price, Is.GreaterThan(0), "Failed to find product price");
            Assert.That(productModelFromDetail.Stock, Is.GreaterThan(0), "Failed to find product availability");
        });

        List<string> diffs = productModelFromDetail.GetDifferences(productModelFromCard).ToList();
        Assert.That(diffs, Is.Empty, $"The product info on the card and detail page are different: {string.Join("; ", diffs)}");
    }
}

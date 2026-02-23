using NUnit.Framework.Internal;
using OpenQA.Selenium.Support.UI;
using UI.Template.Data;
using UI.Template.Models;
using UI.Template.Pages;

namespace UI.Template.Tests;

[TestFixture]
public class AddProductTest : BaseTest
{
    [Test]
    public void AddNewProductToCart()
    {
        //** STEP 1 ***/ - Open admin page
        AdminPage adminPage = new AdminPage();
        adminPage.Open();
        adminPage.WaitForReady();

        //** STEP 2 ***/ Fill in and save new product form
        var form = adminPage.OpenAddNewProductContainer();
        Assert.Multiple(() =>
        {
            Assert.That(form.SetName(TestData.NewProduct.ProductName), Is.True, "Failed to set product name");
            Assert.That(form.SelectCategory(TestData.NewProduct.ProductCategory), Is.True, "Failed to select product category");
            Assert.That(form.SetPrice(TestData.NewProduct.Price), Is.True, "Failed to set product price");
            Assert.That(form.SetStock(TestData.NewProduct.Stock), Is.True, "Failed to set product stock");
            Assert.That(form.SelectImage(TestData.NewProduct.Image), Is.True, "Failed to select product image");
            Assert.That(form.SetDescription(TestData.NewProduct.Description), Is.True, "Failed to set product description");
        });

        form.SaveChanges();

        //** STEP 3 ***/ - Verify that hte product appears in admin page
        Assert.That(
            adminPage.TryGetProductCardByName(TestData.NewProduct.ProductName, out _), Is.True, 
            $"The new product '{TestData.NewProduct.ProductName}' was not found in admin page after saving changes.");

        //** STEP 4 ***/ - Open eshop home page
        HomePage homePage = adminPage.GoToEshopHome();
        Thread.Sleep(500); // Wait for the home page to load

        //** STEP 5 ***/ - Open the new product from the cameras category and verify the product details
        ProductDetailPage productDetail = homePage.OpenProductByNameFromCategory(TestData.NewProduct.ProductCategory, TestData.NewProduct.ProductName);     
        productDetail.WaitForReady();
        Product productFromDetail = productDetail.ProductInfoForm.ToProductModel();

       Assert.Multiple(() =>
        {
            Assert.That(productFromDetail.Name,  Is.EqualTo(TestData.NewProduct.ProductName), "Product name on detail page does not match");
            Assert.That(productFromDetail.Price, Is.EqualTo(TestData.NewProduct.Price), "Product price on detail page does not match");
            Assert.That(productFromDetail.Stock, Is.EqualTo(TestData.NewProduct.Stock), "Product stock on detail page does not match");
        });
    }
}

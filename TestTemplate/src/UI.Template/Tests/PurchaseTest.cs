using UI.Template.Data;
using UI.Template.Pages;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace UI.Template.Tests;

[TestFixture]
public class PurchaseTest : BaseTest
{
    [Test]
    public void CartAndPurchaseTest()
    {
        //** STEP 1 ***/ - Open home page
        HomePage homePage = new HomePage();
        homePage.Open();

        homePage.Header.OpenBasketContainer();

        if (homePage.Header.GetBasketCount() > 0)
        {
            homePage.Header.ClearBasket();
        }
        homePage.Header.CloseBasketContainer();

        homePage.WaitForReady();

        //** STEP 2 ***/ - Open the new product from the cameras category and verify the product details
        ProductDetailPage productDetail = homePage.OpenProductByNameFromCategory(TestData.PurchaseProduct.ProductCategory, TestData.PurchaseProduct.ProductName);
        productDetail.WaitForReady();

        //** STEP 3 ***/ - Get the price of the product to verify it later
        decimal productPrice = productDetail.ProductInfoForm.GetPrice();

        //** STEP 4 ***/ Add the product to the cart and verify that it is shown in the cart
        productDetail.ProductInfoForm.AddToCart();
        WebDriverWait wait = new WebDriverWait(WebDriver, TimeSpan.FromSeconds(10)); // Wait for the cart to update
        wait.Until(driver => productDetail.IsReady());
        Assert.That(productDetail.Header.GetBasketCount(), Is.EqualTo(1), "Basket count is not 1. Check Add to Cart functionality.");

        //** STEP 5 ***/ Open the cart and verify that the correct product with correct data is shown in the cart
        productDetail.Header.OpenBasketContainer();
        Assert.That(productDetail.Header.GetNthProduct(1, out string PurchaseproductName, out string PurchaseproductDetail), Is.True, "The first product in the basket was not found");
        
        Assert.Multiple(() =>
        {
            Assert.That(PurchaseproductName, Is.EqualTo(TestData.PurchaseProduct.ProductName), "The name of product in the basket is not same as in test data");
            Assert.That(PurchaseproductDetail, Does.Contain("1"), "Product quantity in basket does not show 1 piece.");
            Assert.That(PurchaseproductDetail, Does.Contain("700.00"), "Product price in basket does not match the price from product detail page.");
        });

        //** STEP 6 ***/ - Proceed to checkout
        CheckoutPage checkoutPage = productDetail.Header.GoToCheckout();
        checkoutPage.WaitForReady();

        //** STEP 7 ***/ - Fill in the checkout form, check the price and place the order
        checkoutPage.FillForm(TestData.Checkout);
        decimal totalBeforePay = checkoutPage.GetTotalPrice();
        checkoutPage.Pay();

        //** STEP 8 ***/ - Verify the order confirmation
        Assert.Multiple(() =>
        {
            Assert.That(totalBeforePay, Is.EqualTo(productPrice), "Total price before payment does not match the product price.");
            Assert.That(checkoutPage.GetSelectedPaymentMethod(), Does.Contain("PayPal"), "Selected payment method does not match the expected payment method.");
        });

    }
}

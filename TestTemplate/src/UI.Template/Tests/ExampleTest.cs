using UI.Template.Pages;

namespace UI.Template.Tests;

[TestFixture]
public class ExampleTest : BaseTest
{
    [Test]
    public void HomePageTest()
    {
        HomePage homePage = new HomePage();
        homePage.Open();
        bool addedToBasket = homePage.AddToBasketFirstProductFromCategory("Electronics");

        Assert.That(addedToBasket, Is.True, "Product was not added to the basket.");
    }
}

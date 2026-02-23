using UI.Template.Models;

namespace UI.Template.Data;

public static class TestData
{
    public static TestProduct ParametersTestProduct { get; } = new TestProduct
    {
        ProductCategory = "Accessories",
        ProductName = "Wireless Mouse",
        ProductUrl = "/product/2"
    };

    public static TestProduct CardTestProduct { get; } = new TestProduct
    {
        ProductCategory = "Accessories",
        ProductName = "Gaming Keyboard RGB"
    };

    public static TestProduct NewProduct { get; } = new TestProduct
    {
        ProductCategory = "Cameras",
        ProductName = "Camera M25",
        Price = 50.5m,
        Stock = 5,
        Image = "Camera 2",
        Description = "Camera"
    };
}

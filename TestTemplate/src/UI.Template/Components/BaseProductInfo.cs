using System.Globalization;
using System.Text.RegularExpressions;
using OpenQA.Selenium;
using UI.Template.Components.Basic;
using UI.Template.Models;

namespace UI.Template.Components;

public abstract class BaseProductInfo(By locator) : BaseComponent(locator)
{
    public abstract Simple Name { get; }
    public abstract Simple Price { get; }
    public abstract Simple StockStatus { get; }

    /// <inheritdoc/>
    public override void WaitForReady()
    {
        Wait.Until(_ => IsReady() && IsDisplayed());
    }

    /// <summary>
    /// Returns the product name as displayed in the UI.
    /// Default implementation reads text from the <see cref="Name"/> element.
    /// </summary>
    public string GetName() => Name.GetText();

    /// <summary>
    /// Returns the product price as parsed from the UI.
    /// Default implementation reads text from the <see cref="Price"/> element, removes common currency
    /// characters and parses the value using invariant culture.
    /// </summary>
    public decimal GetPrice()
    {
        string text = Price.GetText();
        string cleaned = text.Replace("$", "").Replace(",-", "").Trim();
        return decimal.Parse(cleaned, CultureInfo.InvariantCulture);
    }

    /// <summary>
    /// Returns the product stock status/quantity as parsed from the UI.
    /// Default implementation reads text from the <see cref="StockStatus"/> element and extracts
    /// the first integer found. Returns -1 if a numeric value cannot be parsed.
    /// </summary>
    public int GetStockStatus()
    {
        string text = StockStatus.GetText();
        Match match = Regex.Match(text, @"\d+");
        if (match.Success && int.TryParse(match.Value, out int value))
        {
            return value;
        }
        return -1;
    }

    /// <summary>
    /// Converts UI values into a <see cref="Product"/> instance using the other accessors.
    /// Default implementation delegates to <see cref="GetName"/>, <see cref="GetPrice"/> and <see cref="GetStockStatus"/>.
    /// </summary>
    /// <returns>A new <see cref="Product"/> instance populated from UI values.</returns>
    public Product ToProductModel()
    {
        return new Product(GetName(), GetPrice(), GetStockStatus());
    }
}

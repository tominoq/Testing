using OpenQA.Selenium;
using UI.Template.Components.Basic;

namespace UI.Template.Pages;

public class CheckoutPage() : BasePage("/checkout")
{
    // --- Personal data ---
    private readonly TextInput _firstName = new(By.XPath("//input[@placeholder='Enter your first name']"));
    private readonly TextInput _lastName = new(By.XPath("//input[@placeholder='Enter your last name']"));
    private readonly TextInput _street = new(By.XPath("//input[@placeholder='Enter your street']"));
    private readonly TextInput _city = new(By.XPath("//input[@placeholder='Enter your city']"));
    private readonly TextInput _zipCode = new(By.XPath("//input[@placeholder='Enter your ZIP code']"));
    private readonly TextInput _dateOfBirth = new(By.XPath("//input[@type='date']"));
    private readonly TextInput _email = new(By.XPath("//input[@placeholder='Enter your email']"));

    // --- Phone number (country dropdown + number input) ---
    private readonly DropDownList _phoneCountry = new(By.XPath("//select[contains(@class,'country') or contains(@class,'phone-country') or contains(@class,'flag')]"));
    private readonly TextInput _phoneNumber = new(By.XPath("//input[@placeholder='Enter your phone number']"));

    // --- Delivery and payment ---
    private readonly DropDownList _deliveryMethod = new(By.XPath("//select[contains(@class,'delivery')]"));
    private readonly DropDownList _paymentMethod = new(By.XPath("//select[contains(@class,'payment')]"));

    // --- Optional fields ---
    private readonly TextInput _specialOfferCode = new(By.XPath("//input[@placeholder='Enter your code']"));
    private readonly Checkbox  _isStudent = new(By.XPath("//input[@type='checkbox']"));

    // --- Order summary ---
    private readonly Simple _totalPrice = new(By.XPath("//*[contains(@class,'total-price') or contains(@class,'total-amount') or (contains(@class,'total') and not(contains(@class,'subtotal')))]//span[last()] | //span[contains(@class,'total-value')]"));

    // --- Buttons ---
    private readonly Button _payButton = new(By.XPath("//button[normalize-space()='Pay']"));
    private readonly Button _backToShopButton = new(By.XPath("//button[normalize-space()='Back to Shop']"));

    public override bool IsReady()
    {
        return base.IsReady() && _firstName.IsDisplayed();
    }

    public void FillForm(Checkout data)
    {
        _firstName.SendKeys(data.FirstName);
        _lastName.SendKeys(data.LastName);
        _street.SendKeys(data.Street);
        _city.SendKeys(data.City);
        _zipCode.SendKeys(data.ZipCode);
        _email.SendKeys(data.Email);

        if (!string.IsNullOrEmpty(data.DateOfBirth))
            _dateOfBirth.SendKeys(data.DateOfBirth);

        if (!string.IsNullOrEmpty(data.PhoneCountry))
            _phoneCountry.SelectByText(data.PhoneCountry);

        _phoneNumber.SendKeys(data.PhoneNumber);

        if (!string.IsNullOrEmpty(data.DeliveryMethod))
            _deliveryMethod.SelectByText(data.DeliveryMethod);

        if (!string.IsNullOrEmpty(data.PaymentMethod))
            _paymentMethod.SelectByText(data.PaymentMethod);

        if (!string.IsNullOrEmpty(data.SpecialOfferCode))
            _specialOfferCode.SendKeys(data.SpecialOfferCode);

        if (data.IsStudent)
            _isStudent.Check();
    }
    public void Pay()
    {
        _payButton.ScrollToAndClick();
    }

    public void BackToShop()
    {
        _backToShopButton.ScrollToAndClick();
    }

    public string GetSelectedPaymentMethod()
    {
        return _paymentMethod.GetSelectedOptionText();
    }

    public decimal GetTotalPrice()
    {
        string raw = _totalPrice.GetText()
                                .Replace("$", "")
                                .Replace(",", "")
                                .Trim();
        return decimal.TryParse(raw, System.Globalization.NumberStyles.Any,
                                System.Globalization.CultureInfo.InvariantCulture, out decimal result)
            ? result
            : throw new InvalidOperationException($"Could not parse total price from text: '{raw}'");
    }
}

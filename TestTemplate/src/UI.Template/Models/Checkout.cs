namespace UI.Template.Models;
public class Checkoutdata
{
    // Required fields
    public string FirstName   { get; set; } = string.Empty;
    public string LastName    { get; set; } = string.Empty;
    public string Street      { get; set; } = string.Empty;
    public string City        { get; set; } = string.Empty;
    public string ZipCode     { get; set; } = string.Empty;
    public string Email       { get; set; } = string.Empty;
    public string PhoneCountry { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string DeliveryMethod { get; set; } = string.Empty;
    public string PaymentMethod { get; set; } = string.Empty;

    // Optional fields
    public string DateOfBirth { get; set; } = string.Empty;
    public string SpecialOfferCode { get; set; } = string.Empty;
    public bool IsStudent { get; set; }
}













namespace SavingsBook.Domain.Common;

public class Address
{
    public string? Country { get; set; }
    public string? City { get; set; }
    public string? Street { get; set; }
    public string? Zipcode { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
}
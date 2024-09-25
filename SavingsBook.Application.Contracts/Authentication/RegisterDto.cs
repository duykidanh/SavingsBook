using System.ComponentModel.DataAnnotations;

namespace SavingsBook.Application.Contracts.Authentication;

public class RegisterDto
{
    [Required, EmailAddress]
    public string Email { get; set; }
    [Required, MinLength(6), MaxLength(20)]
    public string Username { get; set; }
    [Required, DataType(DataType.Password)]
    public string Password { get; set; }
    // [Required]
    // public string FullName { get; set; }
    // [Required]
    // public string Address { get; set; }
    // [Required, MinLength(9), MaxLength(12)]
    // public string IdCardNumber { get; set; }
}
using System.ComponentModel.DataAnnotations;

namespace SavingsBook.Application.Contracts.Authentication;

public class LoginDto
{
    [Required]
    public string Username { get; set; }
    [Required, DataType(DataType.Password)]
    public string Password { get; set; }
}

public class AuthenticationResponseDto
{
    public string? Message { get; set; }
    public bool Success { get; set; }
    public TokenType? AccessToken { get; set; }
    public TokenType? RefreshToken { get; set; }

    public class TokenType
    {
        public string? Token { get; set; }
        public DateTime ExpiresTime { get; set; }
        
    }
}

using System.ComponentModel.DataAnnotations;
using Domain.Constants;

namespace Domain.DTOs;

public class RegisterDto
{
    [Required(ErrorMessage = "Invalid_Name_Input")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "Invalid_Name_Input")]
    public string Name { get; set; }

    [Required(ErrorMessage = "Invalid_Email_Input")]
    [EmailAddress(ErrorMessage = "Invalid_Email_Input")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Invalid_Role_Input")]
    [RegularExpression("User|Seller", ErrorMessage = "Invalid_Role_Input")]
    public string Role { get; set; } = Roles.User;

    [Required(ErrorMessage = "Invalid_Password_Input")]
    [MinLength(6, ErrorMessage = "Invalid_Password_Input")]
    public string Password { get; set; }
}

public class LoginDto
{
    [Required(ErrorMessage = "Invalid_Email_Input")]
    [EmailAddress(ErrorMessage = "Invalid_Email_Input")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Invalid_Password_Input")]
    [MinLength(6, ErrorMessage = "Invalid_Password_Input")]
    public string Password { get; set; }
}

public class TokenDto
{
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
    public DateTime AccessTokenExpiration { get; set; }
}

public record RefreshTokenRequest(string RefreshToken);
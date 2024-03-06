using System.ComponentModel.DataAnnotations;

namespace Business.DTOs
{
  public class UserCredentialsDTO
  {
    [Required]
    public string UserName { get; set; } = null!;
    [Required]
    public string Password { get; set; } = null!;
  }
  public class AuthenticationResponseDTO
  {
    public string Token { get; set; } = null!;
    public DateTime Expire { get; set; }
  }
  public class RegisterModelDTO
  {
    [Required]
    public string UserName { get; set; } = null!;
    [Required]
    [EmailAddress]
    public string Email { get; set; } = null!;
    [Required]
    public string Password { get; set; } = null!;
    [Required]
    public string PhoneNumber { get; set; } = null!;
  }
}
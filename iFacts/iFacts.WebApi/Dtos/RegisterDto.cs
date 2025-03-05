using System.ComponentModel.DataAnnotations;

namespace iFacts.WebApi.Dtos;

public class RegisterDto
{
    [Required, StringLength(100, MinimumLength = 1)]
    public string FirstName { get; set; }
    [StringLength(100, MinimumLength = 1)]
    public string MiddleName { get; set; }
    [Required, StringLength(100, MinimumLength = 1)]
    public string LastName { get; set; }
    [Required, EmailAddress, StringLength(200, MinimumLength = 5)]
    public string Login { get; set; }
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[#$^+=!*()@%&]).{8,}$")]
    public string Password { get; set; }
}

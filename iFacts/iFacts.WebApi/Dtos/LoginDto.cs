using Extensions.DeviceDetector.Models;
using iFacts.Shared.Auth;
using System.ComponentModel.DataAnnotations;

namespace iFacts.WebApi.Dtos;

public class LoginDto
{
    [Required, EmailAddress, StringLength(200, MinimumLength = 5)]
    public string Login { get; set; }
    [Required]
    [RegularExpression(RegexTemplate.Password.Regex, ErrorMessage = RegexTemplate.Password.ErrorMessage)]
    public string Password { get; set; }
    [Required]
    public string Lang { get; set; }
    public string PushFcmToken { get; set; }
    public ClientInfo Client { get; set; }
    public AppLoginCreateModel App { get; set; }
}

public class AppLoginCreateModel
{
    [Required, StringLength(30)]
    public string Id { get; set; }
    [Required, StringLength(70)]
    public string Secret { get; set; }
    [Required, StringLength(50, MinimumLength = 1)]
    public string Version { get; set; }
}
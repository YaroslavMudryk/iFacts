using iFacts.Data.Entities;

namespace iFacts.WebApi.Dtos;

public class UserTokenModel
{
    public int UserId { get; set; }
    public User User { get; set; }
    public Guid SessionId { get; set; }
    public string AuthType { get; set; }
    public string Lang { get; set; }
}

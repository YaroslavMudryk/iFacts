namespace iFacts.Shared.Auth;

public class JwtToken
{
    public string AccessToken { get; set; }
    public string SessionId { get; set; }
    public string RefreshToken { get; set; }
    public DateTime ExpiredAt { get; set; }
}

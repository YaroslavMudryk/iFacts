using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace iFacts.Shared.Auth;

public class TokenOptions
{
    public const string Issuer = "System ID";
    public const string Audience = "System Client";
    const string SecretKey = "34i7gh3489gh30894gjh0834ghj03r8gh993ri0"; // temp
    public const int LifeTimeInMinutes = 60;
    public static SymmetricSecurityKey GetSymmetricSecurityKey() =>
        new SymmetricSecurityKey(Encoding.ASCII.GetBytes(SecretKey));
}

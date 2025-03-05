using Microsoft.AspNetCore.Http;
using iFacts.Shared.Exceptions;

namespace iFacts.Shared.Auth;

public abstract record CurrentUser;

public record UninitializedUser : CurrentUser
{
    public static UninitializedUser Instance { get; } = new();
}

public record UnauthenticatedUser : CurrentUser
{
    public static UnauthenticatedUser Instance { get; } = new();
}

public record BasicAuthenticatedUser(int UserId, string SessionId, string Login, IEnumerable<ClaimPermissionDto> Claims) : CurrentUser
{
    public void EnsureUserHasPermissions(string type, string value)
    {
        if (Claims.Where(s => s.Type == type).Any(s => s.Value == value))
            return;

        throw new UnauthorizedException();
    }

    public bool UserIsInRoles(IReadOnlyCollection<string> rolesToCheck)
    {
        var roles = Claims.Where(s => s.Type == AppClaims.Types.Role).ToList();

        return roles.Any(s => rolesToCheck.Any(rtc => rtc.Contains(s.Value)));
    }
}

public static class CurrentUserHelper
{
    public static CurrentUser GetCurrentUser(this HttpContext httpContext)
    {
        ArgumentNullException.ThrowIfNull(httpContext);

        var userIdClaim = httpContext.User.Claims.FirstOrDefault(s => s.Type == AppClaims.Types.UserId);
        var sessionIdClaim = httpContext.User.Claims.FirstOrDefault(s => s.Type == AppClaims.Types.SessionId);
        var loginClaim = httpContext.User.Claims.FirstOrDefault(s => s.Type == AppClaims.Types.Login);
        var otherClaims = httpContext.User.Claims.Where(s => s.Type != AppClaims.Types.UserId && s.Type != AppClaims.Types.SessionId && s.Type != AppClaims.Types.Login).Select(s => new ClaimPermissionDto(s.Type, s.Value));

        if (userIdClaim is null || sessionIdClaim is null)
            return UnauthenticatedUser.Instance;

        return new BasicAuthenticatedUser(Convert.ToInt32(userIdClaim.Value), sessionIdClaim.Value, loginClaim.Value, otherClaims);
    }
}
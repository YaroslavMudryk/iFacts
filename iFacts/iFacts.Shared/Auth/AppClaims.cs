using System.Security.Claims;

namespace iFacts.Shared.Auth;

public static class AppClaims
{
    public static class Types
    {
        public const string UserId = ClaimTypes.NameIdentifier;
        public const string SessionId = "sessionId";
        public const string Login = "login";
        public const string Role = "role";

        public const string Facts = nameof(Facts);
    }

    public static class Values
    {
        public const string Create = nameof(Create);
        public const string Update = nameof(Update);
        public const string Delete = nameof(Delete);
        public const string SoftDelete = nameof(SoftDelete);
        public const string ViewAll = nameof(ViewAll);
        public const string View = nameof(View);
        public const string ChangeSecret = nameof(ChangeSecret);
    }
}

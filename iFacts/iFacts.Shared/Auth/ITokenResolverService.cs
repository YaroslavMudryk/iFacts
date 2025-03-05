namespace iFacts.Shared.Auth;

public interface ITokenResolverService
{
    Task<CurrentUser> GetUserAsync(string token);
}

public class MockTokenResolverService : ITokenResolverService
{
    public Task<CurrentUser> GetUserAsync(string token)
    {
        if (token == "1")
            return Task.FromResult<CurrentUser>(UnauthenticatedUser.Instance);

        return Task.FromResult<CurrentUser>(new BasicAuthenticatedUser(1, "1F6F5702-B817-4B2B-B2FA-BFA5C7B6E05D", "testUser@email.com", []));
    }
}
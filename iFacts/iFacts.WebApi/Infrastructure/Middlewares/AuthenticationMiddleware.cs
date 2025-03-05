using iFacts.Shared.Auth;
using Microsoft.AspNetCore.Authorization;

namespace iFacts.WebApi.Infrastructure.Middlewares;

public class AuthenticationMiddleware(IUserContext userContext, ITokenResolverService tokenResolver) : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(next);

        var endpoint = context.GetEndpoint();
        var allowAnonymous = endpoint?.Metadata.GetMetadata<AllowAnonymousAttribute>() != null;

        if (!allowAnonymous)
        {
            var user = await tokenResolver.GetUserAsync(context.Request.Headers.Authorization);
            if (user is UnauthenticatedUser)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Response.ContentType = "application/json";

                return;
            }

            ((UserContext)userContext).CurrentUser = user;
        }

        await next(context);
    }
}

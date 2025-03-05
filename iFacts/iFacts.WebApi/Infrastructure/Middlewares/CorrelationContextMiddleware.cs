using iFacts.Shared.Auth;
using iFacts.WebApi.Infrastructure.Helpers;

namespace iFacts.WebApi.Infrastructure.Middlewares;

public class CorrelationContextMiddleware : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(next);

        var correlationId = context.GetOrAssignCorrelationId();
        ((UserContext)context.RequestServices.GetService<IUserContext>()).CorrelationId = correlationId;
        context.Response.OnStarting(() =>
        {
            context.Response.Headers.Append(HttpHeadersConstants.CorrelationId, correlationId);
            return Task.CompletedTask;
        });

        await next(context);
    }
}

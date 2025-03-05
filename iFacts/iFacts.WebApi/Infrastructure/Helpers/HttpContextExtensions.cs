using iFacts.Shared.Auth;

namespace iFacts.WebApi.Infrastructure.Helpers;

public static class HttpContextExtensions
{
    public static string GetCorreletionId(this HttpContext httpContext)
    {
        ArgumentNullException.ThrowIfNull(httpContext);
        return httpContext.Request.Headers[HttpHeadersConstants.CorrelationId];
    }

    public static string GetOrAssignCorrelationId(this HttpContext httpContext)
    {
        ArgumentNullException.ThrowIfNull(httpContext);

        var correlationId = httpContext.GetCorreletionId();

        if (string.IsNullOrEmpty(correlationId))
        {
            correlationId = Guid.NewGuid().ToString("N");
            httpContext.Request.Headers.Append(HttpHeadersConstants.CorrelationId, correlationId);
        }

        return correlationId;
    }
}
